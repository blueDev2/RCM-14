
using Robust.Shared.Map;

namespace Content.Shared._RMC14.Intelligence;

/// <summary>
/// Creates a rectangle where intel related items can be placed to gain intel points
/// The Entity's location is assumed to be the top-left point of the rectangle
/// </summary>
public sealed partial class IntelDropZoneComponent : Component
{
    [DataField]
    public int Width = 0;
    [DataField]
    public int Height = 0;

    //public MapCoordinates TopLeftPoint = new();
}
