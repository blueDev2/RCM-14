
namespace Content.Shared._RMC14.Intelligence;

/// <summary>
/// Creates a rectangle where intel related items can be placed to gain intel points
/// </summary>
public sealed partial class IntelDropZoneComponent : Component
{
    [DataField(required: true)]
    public int Width = 0;
    [DataField(required: true)]
    public int Height = 0;
}
