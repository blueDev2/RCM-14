using Content.Shared._RMC14.Prototypes;
using Content.Shared.Body.Prototypes;
using Content.Shared.Chemistry.Reaction;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Prototypes;
using System.Collections.Frozen;

namespace Content.Shared._RMC14.Chemistry.Components;

/// <summary>
///
/// </summary>
[Prototype("bundledReagentEffectsPrototype")]
[DataDefinition]
public sealed partial class BundledReagentEffectsPrototype : IPrototype
{
    [ViewVariables]
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField(required: true)]
    private LocId Name { get; set; }

    [DataField(serverOnly: true)]
    public FrozenDictionary<ProtoId<MetabolismGroupPrototype>, ReagentEffectsEntry>? Metabolisms;

    [DataField(serverOnly: true)]
    public Dictionary<ProtoId<ReactiveGroupPrototype>, Content.Shared.Chemistry.Reagent.ReactiveReagentEffectEntry>? ReactiveEffects;
}
