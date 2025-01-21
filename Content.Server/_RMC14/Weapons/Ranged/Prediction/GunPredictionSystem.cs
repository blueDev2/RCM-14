using Content.Server._RMC14.CollisionPrediction;
using Content.Server.Movement.Components;
using Content.Server.Weapons.Ranged.Systems;
using Content.Shared._RMC14.CCVar;
using Content.Shared._RMC14.Weapons.Ranged.Prediction;
using Content.Shared.GameTicking;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Server.GameObjects;
using Robust.Shared.Configuration;
using Robust.Shared.Map;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Timing;

namespace Content.Server._RMC14.Weapons.Ranged.Prediction;

public sealed class GunPredictionSystem : SharedGunPredictionSystem
{
    [Dependency] private readonly IConfigurationManager _config = default!;
    [Dependency] private readonly GunSystem _gun = default!;
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;
    [Dependency] private readonly SharedProjectileSystem _projectile = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly TransformSystem _transform = default!;
    [Dependency] private readonly CollisionPredictionSystem _collisionPredict = default!;

    private readonly Dictionary<(Guid, int), EntityUid> _predicted = new();
    private readonly List<(PredictedProjectileHitEvent Event, ICommonSession Player)> _predictedHits = new();
    private bool _preventCollision;
    private bool _logHits;
    private float _coordinateDeviation;
    private float _lowestCoordinateDeviation;
    private float _aabbEnlargement;

    private EntityQuery<FixturesComponent> _fixturesQuery;
    private EntityQuery<LagCompensationComponent> _lagCompensationQuery;
    private EntityQuery<PhysicsComponent> _physicsQuery;
    private EntityQuery<ProjectileComponent> _projectileQuery;
    private EntityQuery<PredictedProjectileServerComponent> _predictedProjectileServerQuery;
    private EntityQuery<TransformComponent> _transformQuery;

    public override void Initialize()
    {
        base.Initialize();

        _fixturesQuery = GetEntityQuery<FixturesComponent>();
        _lagCompensationQuery = GetEntityQuery<LagCompensationComponent>();
        _physicsQuery = GetEntityQuery<PhysicsComponent>();
        _projectileQuery = GetEntityQuery<ProjectileComponent>();
        _predictedProjectileServerQuery = GetEntityQuery<PredictedProjectileServerComponent>();
        _transformQuery = GetEntityQuery<TransformComponent>();

        SubscribeLocalEvent<RoundRestartCleanupEvent>(OnRoundRestartCleanup);
        SubscribeNetworkEvent<RequestShootEvent>(OnShootRequest);
        SubscribeNetworkEvent<PredictedProjectileHitEvent>(OnPredictedProjectileHit);

        SubscribeLocalEvent<PredictedProjectileServerComponent, MapInitEvent>(OnPredictedMapInit);
        SubscribeLocalEvent<PredictedProjectileServerComponent, ComponentRemove>(OnPredictedRemove);
        SubscribeLocalEvent<PredictedProjectileServerComponent, EntityTerminatingEvent>(OnPredictedRemove);
        SubscribeLocalEvent<PredictedProjectileServerComponent, PreventCollideEvent>(OnPredictedPreventCollide);

        Subs.CVar(_config, RMCCVars.RMCGunPredictionPreventCollision, v => _preventCollision = v, true);
        Subs.CVar(_config, RMCCVars.RMCGunPredictionLogHits, v => _logHits = v, true);
        Subs.CVar(_config, RMCCVars.RMCGunPredictionCoordinateDeviation, v => _coordinateDeviation = v, true);
        Subs.CVar(_config, RMCCVars.RMCGunPredictionLowestCoordinateDeviation, v => _lowestCoordinateDeviation = v, true);
        Subs.CVar(_config, RMCCVars.RMCGunPredictionAabbEnlargement, v => _aabbEnlargement = v, true);

    }

    private void OnRoundRestartCleanup(RoundRestartCleanupEvent ev)
    {
        _predicted.Clear();
    }

    private void OnShootRequest(RequestShootEvent ev, EntitySessionEventArgs args)
    {
        _gun.ShootRequested(ev.Gun, ev.Coordinates, ev.Target, ev.Shot, args.SenderSession);
    }

    private void OnPredictedMapInit(Entity<PredictedProjectileServerComponent> ent, ref MapInitEvent args)
    {
        if (ent.Comp.Shooter == null)
        {
            Log.Warning($"{nameof(PredictedProjectileServerComponent)} map initialized with a null shooter session!");
            return;
        }

        _predicted[(ent.Comp.Shooter.UserId, ent.Comp.ClientId)] = ent;
    }

    private void OnPredictedRemove<T>(Entity<PredictedProjectileServerComponent> ent, ref T args)
    {
        if (ent.Comp.Shooter == null)
            return;

        _predicted.Remove((ent.Comp.Shooter.UserId, ent.Comp.ClientId));
    }

    private void OnPredictedProjectileHit(PredictedProjectileHitEvent ev, EntitySessionEventArgs args)
    {
        _predictedHits.Add((ev, args.SenderSession));
    }

    private void OnPredictedPreventCollide(Entity<PredictedProjectileServerComponent> ent, ref PreventCollideEvent args)
    {
        if (!_preventCollision)
            return;

        if (args.Cancelled)
            return;

        var other = args.OtherEntity;
        if (!_lagCompensationQuery.TryComp(other, out var otherLagComp) ||
            !_fixturesQuery.TryComp(other, out var otherFixtures) ||
            !_transformQuery.TryComp(other, out var otherTransform))
        {
            return;
        }

        if (!_physicsQuery.TryComp(ent, out var entPhysics))
            return;

        if (!PredictCollision(
                (ent, ent, entPhysics),
                (other, otherLagComp, otherFixtures, args.OtherBody, otherTransform),
                null))
        {
            args.Cancelled = true;
        }
    }

    private bool PredictCollision(
        Entity<PredictedProjectileServerComponent, PhysicsComponent> projectile,
        Entity<LagCompensationComponent, FixturesComponent, PhysicsComponent, TransformComponent> other,
        MapCoordinates? clientCoordinates)
    {
        var ping = projectile.Comp1.Shooter?.Channel.Ping ?? 0;
        return _collisionPredict.PredictCollision((projectile.Owner, projectile.Comp2),
                                                    other,
                                                    clientCoordinates,
                                                    ping,
                                                    _coordinateDeviation,
                                                    _lowestCoordinateDeviation,
                                                    _aabbEnlargement);
    }

    private void ProcessPredictedHit(PredictedProjectileHitEvent ev, ICommonSession player)
    {
        if (!_predicted.TryGetValue((player.UserId, ev.Projectile), out var projectile))
            return;

        if (!_predictedProjectileServerQuery.TryComp(projectile, out var predictedProjectile) ||
            predictedProjectile.Hit)
        {
            return;
        }

        if (predictedProjectile.Shooter?.UserId != player.UserId.UserId)
            return;

        if (!_projectileQuery.TryComp(projectile, out var projectileComp) ||
            !_physicsQuery.TryComp(projectile, out var projectilePhysics))
        {
            return;
        }

        predictedProjectile.Hit = true;
        foreach (var (netEnt, clientPos) in ev.Hit)
        {
            if (GetEntity(netEnt) is not { Valid: true } hit)
                continue;

            if (!_lagCompensationQuery.TryComp(hit, out var otherLagComp) ||
                !_fixturesQuery.TryComp(hit, out var otherFixtures) ||
                !_physicsQuery.TryComp(hit, out var otherPhysics) ||
                !_transformQuery.TryComp(hit, out var otherTransform))
            {
                continue;
            }

            if (!PredictCollision(
                    (projectile, predictedProjectile, projectilePhysics),
                    (hit, otherLagComp, otherFixtures, otherPhysics, otherTransform),
                    clientPos))
            {
                if (_logHits)
                    Log.Info("missed");

                continue;
            }

            if (_logHits)
                Log.Info("hit");

            _projectile.ProjectileCollide((projectile, projectileComp, projectilePhysics), hit, true);
        }
    }

    public override void Update(float frameTime)
    {
        try
        {
            foreach (var ev in _predictedHits)
            {
                ProcessPredictedHit(ev.Event, ev.Player);
            }
        }
        finally
        {
            _predictedHits.Clear();
        }

        var predicted = EntityQueryEnumerator<PredictedProjectileHitComponent, TransformComponent>();
        while (predicted.MoveNext(out var uid, out var hit, out var xform))
        {
            var origin = hit.Origin;
            var coordinates = xform.Coordinates;
            if (!origin.TryDistance(EntityManager, _transform, coordinates, out var distance) ||
                distance >= hit.Distance)
            {
                QueueDel(uid);
            }
        }
    }
}
