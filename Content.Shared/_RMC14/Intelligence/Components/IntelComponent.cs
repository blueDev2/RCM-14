

using Content.Shared.Tag;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Intelligence;

/// <summary>
/// Reading this item provides hints of locations for other items
/// </summary>
public sealed partial class IntelComponent : Component
{
    /// <summary>
    /// How long, in seconds, it takes a person to decrypt the Intel Hints
    /// </summary>
    [DataField]
    public int AnalyzeDuration = 2;

    /// <summary>
    /// Hints that will be included in the paper component of the intel.
    /// Should never be more than 4.
    /// </summary>
    public List<LocId> IntelHintLocIDs = new();

    /// <summary>
    /// The types of intel entities that can be targets of clues from this entity
    /// </summary>
    [DataField]
    public List<ProtoId<TagPrototype>> IntelWhitelist = new();
}
