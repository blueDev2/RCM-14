
using Content.Shared.Damage;
using Content.Shared.Tag;
using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;

namespace Content.Server._RMC14.Construction;

/// <summary>
/// Heals damage to an entity by using up another entity.
/// <see cref="Repairable.RepairableComponent"/> for healing via tool.
/// </summary>
public sealed partial class RepairableByReplacementComponent : Component
{

    [DataField]
    public TagPrototype RepairTag = new();

    [DataField]
    public DamageSpecifier? Damage;

    [DataField]
    public int MaterialCost = 5;

    [DataField]
    public int DoAfterDelay = 1;

    /// <summary>
    /// A multiplier that will be applied to the above if an entity is repairing themselves.
    /// </summary>
    [DataField]
    public float SelfRepairPenalty = 3f;

    /// <summary>
    /// Whether or not an entity is allowed to repair itself.
    /// </summary>
    [DataField]
    public bool AllowSelfRepair = true;
}
