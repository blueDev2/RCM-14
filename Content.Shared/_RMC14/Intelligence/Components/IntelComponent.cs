

namespace Content.Shared._RMC14.Intelligence;

/// <summary>
/// Reading this item provides hints of locations for other items
/// </summary>
public sealed partial class IntelComponent : Component
{
    /// <summary>
    /// How long, in seconds, it takes a person to decrypt the Intel Hints
    /// </summary>
    public int AnalyzeDuration = 2;

    public double PointsAward = 0.2;

    /// <summary>
    /// Hints that will be included in the paper component of the intel.
    /// Should never be higher than 4.
    /// </summary>
    public List<string> IntelHints = new();

    /// <summary>
    /// The current highest intel skill entity that has analyzed this particular entity.
    /// Each level reveals 1 clue. 
    /// </summary>
    //public int HighestLevelAnalyzed = 0;
}
