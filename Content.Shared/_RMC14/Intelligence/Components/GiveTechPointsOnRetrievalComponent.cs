

namespace Content.Shared._RMC14.Intelligence;

/// <summary>
/// Placing this item in an appropriate location provides Tech Points
/// </summary>
public sealed partial class GiveIntelOnRetrievalComponent : Component
{
    [DataField]
    public decimal PointReward = 0.1M;
    [DataField]
    public RewardSchedule RewardSchedule = RewardSchedule.SINGLE;
}

public enum RewardSchedule
{
    /// <summary>
    /// Will no longer reward points when dropped off, used when a retrieval item that provides a one time reward has
    /// already been placed in the zone.
    /// </summary>
    NEVER,
    /// <summary>
    /// Will reward points one time when placed in the zone.
    /// </summary>
    SINGLE,
    /// <summary>
    /// Will reward points every update while in the zone.
    /// </summary>
    CONTINUOUS
}
