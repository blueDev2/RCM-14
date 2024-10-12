using Content.Shared._RMC14.Xenonids.Egg.EggGenerator;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Mobs;
using Robust.Shared.Prototypes;
using Robust.Shared.Spawners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Shared._RMC14.Xenonids.Egg.FragileEggGenerator;

public sealed partial class XenoFragileEggGeneratorSystem : EntitySystem
{
    [Dependency] private readonly EntityManager _entities = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<XenoFragileEggGeneratorComponent, XenoGenerateEggActionEvent>(OnGenerateEgg);
        SubscribeLocalEvent<XenoFragileEggGeneratorComponent, XenoGenerateEggDoAfter>(OnGenerateEggDoAfter);

        SubscribeLocalEvent<XenoFragileEggComponent, XenoEggPlacedEvent>(OnPlaceFragileEgg);

        SubscribeLocalEvent<XenoFragileEggGeneratorComponent, MobStateChangedEvent>(OnDeath);
    }

    private void OnGenerateEgg(EntityUid ent, XenoFragileEggGeneratorComponent comp, XenoGenerateEggActionEvent args)
    {

    }

    private void OnGenerateEggDoAfter(EntityUid ent, XenoFragileEggGeneratorComponent comp, XenoGenerateEggDoAfter args)
    {
        var newEgg = Spawn(comp.FragileEggPrototype);

        var eggRetrieverComp = EnsureComp<XenoEggRetrieverComponent>(ent);
    }

    private void OnPlaceFragileEgg(EntityUid ent, XenoFragileEggComponent comp, XenoEggPlacedEvent args)
    {
        var localEgg = (ent, comp);
        var localPlacer = _entities.GetEntity(args.Placer);

        // Once hiveweeds are added, check if it's planted on hive weeds here

        if (TryComp(localPlacer, out XenoFragileEggGeneratorComponent? fragileEggGenComp))
        {
            SustainFragileEgg(localPlacer, fragileEggGenComp, localEgg);
        }
        else
        {
            DecayEgg(ent, comp.UnsustainedEggDecay);
        }
    }

    private void OnDeath(EntityUid ent, XenoFragileEggGeneratorComponent comp, MobStateChangedEvent args)
    {

    }

    private void SustainFragileEgg(EntityUid ent, XenoFragileEggGeneratorComponent comp, Entity<XenoFragileEggComponent> fragileEgg)
    {
        var sustainedEggs = comp.SustainedEggs;
        sustainedEggs.Enqueue(fragileEgg);
        DecayEgg(fragileEgg, comp.SustainedEggDecay);

        if (comp.MaxSustainableEggCount < sustainedEggs.Count)
        {
            var unsustainedEgg = sustainedEggs.Dequeue();
            DecayEgg(unsustainedEgg, fragileEgg.Comp.UnsustainedEggDecay);
        }
    }


    private void DecayEgg(EntityUid fragileEgg, float lifeTime)
    {
        var despawnTimerComp = new TimedDespawnComponent();
        despawnTimerComp.Lifetime = lifeTime;

        AddComp(fragileEgg, despawnTimerComp);
    }
}

public sealed partial class XenoGenerateEggDoAfter : SimpleDoAfterEvent;
