using Content.Shared.DoAfter;


namespace Content.Shared._RMC14.Intelligence;

public sealed partial class DoAfterAnalyzeIntelEvent : SimpleDoAfterEvent
{
    public EntityUid? AnalyzingEntity;

    public override DoAfterEvent Clone()
    {
        DoAfterAnalyzeIntelEvent newDoAfterAnalyzeIntelEvent = (DoAfterAnalyzeIntelEvent) base.Clone();
        newDoAfterAnalyzeIntelEvent.AnalyzingEntity = this.AnalyzingEntity;
        return newDoAfterAnalyzeIntelEvent;
    }
    public DoAfterAnalyzeIntelEvent(EntityUid analyzingEntity)
    {
        AnalyzingEntity = analyzingEntity;
    }
}
