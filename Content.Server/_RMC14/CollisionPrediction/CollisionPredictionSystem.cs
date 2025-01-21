using Content.Server.Movement.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Server._RMC14.CollisionPrediction;

public sealed partial class CollisionPredictionSystem : EntitySystem
{
    [Dependency] private readonly TransformSystem _transform = default!;
    [Dependency] private readonly PhysicsSystem _physics = default!;
    [Dependency] private readonly GameTiming _timing = default!;

    public bool PredictCollision(
        Entity<PhysicsComponent> projectile,
        Entity<LagCompensationComponent, FixturesComponent, PhysicsComponent, TransformComponent> other,
        MapCoordinates? clientCoordinates,
        float ping,
        float coordinateDeviation,
        float lowestCoordinateDeviation,
        float aabbEnlargement)
    {
        var projectileCoordinates = _transform.GetMapCoordinates(projectile);
        var projectilePosition = projectileCoordinates.Position;

        MapCoordinates lowestCoordinate = default;
        var otherCoordinates = EntityCoordinates.Invalid;
        // Use 1.5 due to the trip buffer.
        var sentTime = _timing.CurTime - TimeSpan.FromMilliseconds(ping * 1.5);
        var pingTime = TimeSpan.FromMilliseconds(ping);

        foreach (var pos in other.Comp1.Positions)
        {
            otherCoordinates = pos.Item2;
            if (pos.Item1 >= sentTime)
                break;
            else if (lowestCoordinate == default && pos.Item1 >= sentTime - pingTime)
                lowestCoordinate = _transform.ToMapCoordinates(pos.Item2);
        }

        var otherMapCoordinates = otherCoordinates == default
            ? _transform.GetMapCoordinates(other)
            : _transform.ToMapCoordinates(otherCoordinates);

        if (clientCoordinates != null &&
            (clientCoordinates.Value.InRange(otherMapCoordinates, coordinateDeviation) ||
             clientCoordinates.Value.InRange(lowestCoordinate, lowestCoordinateDeviation)))
        {
            otherMapCoordinates = clientCoordinates.Value;
        }

        var transform = new Transform(otherMapCoordinates.Position, 0);
        var bounds = new Box2(transform.Position, transform.Position);

        foreach (var fixture in other.Comp2.Fixtures.Values)
        {
            if ((fixture.CollisionLayer & projectile.Comp.CollisionMask) == 0)
                continue;

            for (var i = 0; i < fixture.Shape.ChildCount; i++)
            {
                var boundy = fixture.Shape.ComputeAABB(transform, i);
                bounds = bounds.Union(boundy);
            }
        }

        bounds = bounds.Enlarged(aabbEnlargement);
        if (bounds.Contains(projectilePosition))
            return true;

        var projectileVelocity = _physics.GetLinearVelocity(projectile, projectile.Comp.LocalCenter);
        projectilePosition = projectileCoordinates.Position + projectileVelocity / _timing.TickRate / 1.5f;
        if (bounds.Contains(projectilePosition))
            return true;

        return false;
    }
}
