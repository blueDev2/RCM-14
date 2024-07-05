namespace Content.Server._RMC14.Medical.Pain;

/// <summary>
/// Applies pain mechanisms to the target entity
/// </summary>
public sealed partial class PainRecipientComponent : Component
{
    public int PainLevel = 0;

    [DataField]
    public int ThresholdMild = 20;

    [DataField]
    public int ThresholdDiscomforting = 30;

    [DataField]
    public int ThresholdModerate = 40;

    [DataField]
    public int ThresholdDistressing = 60;

    [DataField]
    public int ThresholdSevere = 70;

    [DataField]
    public int ThresholdHorrible = 80;

    [DataField]
    public int MaxPain = 200;
}
