
namespace Content.Server._RMC14.Objectives.Components;

/// <summary>
/// Notes down the clues that you have found
/// </summary>
[RegisterComponent]
public sealed partial class FindIntelTargetConditionComponent : Component
{
    [DataField(required: true)]
    public LocId ObjectiveText;

    [DataField(required: true)]
    public LocId DescriptionText;
}
