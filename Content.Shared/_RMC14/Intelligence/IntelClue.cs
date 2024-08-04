using Content.Shared._RMC14.Intelligence.Components;

namespace Content.Shared._RMC14.Intelligence;

public struct IntelClue
{
    /// <summary>
    /// The target of an Intel Clue.
    /// Usually has an intel component.
    /// Usually have a GiveIntelOnRetrieval Component.
    /// </summary>
    public EntityUid IntelTarget;

    /// <summary>
    /// The closest landmark entity to the intel target (at spawn).
    /// </summary>
    /// <remarks>
    /// If rooms are created, peform a check on which room contains the intel target (at spawn)
    /// and store that name.
    /// </remarks>
    public Entity<IntelLandmarkComponent> ClosestLandmark;
}
