

namespace Content.Shared._RMC14.Intelligence;

/// <summary>
/// Reading this item provides hints of locations for other items
/// </summary>
public sealed partial class IntelComponent : Component
{
    public int AnalyzeDuration = 2;
    public Dictionary<EntityUid, string> IntelHints = new();
}
