using Robust.Shared.GameStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Shared._RMC14.Marines.Skills;

/// <summary>
/// Indicates that Activatable UIs on the entity require a certain level of skill by the user
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ActivatableUIRequiresSkillsComponent : Component
{
    /// <summary>
    /// Message sent when someone without the required skills attempts to activate a UI
    /// </summary>
    [DataField]
    public LocId PopupMessage = "default-insufficient-skill";

    [DataField]
    public Skills RequiredSkills = new();
}


