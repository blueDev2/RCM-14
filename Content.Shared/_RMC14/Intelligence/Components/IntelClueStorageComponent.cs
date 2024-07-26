
namespace Content.Shared._RMC14.Intelligence.Components;

/// <summary>
/// The Component that stores all Intel Clues within an entity,
/// Components like the IntelClueDisplay and IntelClueUpload must reference this component
/// </summary>
public sealed partial class IntelClueStorageComponent : Component
{
    public List<IntelClueStorageComponent> Clues = new();
}
