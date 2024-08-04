
namespace Content.Shared._RMC14.Intelligence.Components;

/// <summary>
/// The Component that stores all Intel Clues within an entity,
/// Components like the IntelClueDisplay and IntelClueUpload must reference this component,
/// so this component is necessary for them to work.
/// </summary>
public sealed partial class IntelClueStorageComponent : Component
{
    public HashSet<IntelClue> Clues = new();

    /// <summary>
    /// Whether or not that the server should recieve hints from this component.
    /// </summary>
    [DataField]
    public bool SendToServer = false;

    /// <summary>
    /// Whether or not that the server should send hints to this component.
    /// </summary>
    [DataField]
    public bool RecieveFromServer = true;
}
