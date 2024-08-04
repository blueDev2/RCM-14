using Content.Shared._RMC14.Intelligence.Components;
using Content.Shared.DoAfter;


namespace Content.Shared._RMC14.Intelligence;

public sealed partial class DoAfterAnalyzeIntelEvent : SimpleDoAfterEvent
{
    public IntelComponent Intel;

    public override DoAfterEvent Clone()
    {
        DoAfterAnalyzeIntelEvent newDoAfterAnalyzeIntelEvent = (DoAfterAnalyzeIntelEvent) base.Clone();
        newDoAfterAnalyzeIntelEvent.Intel = this.Intel;
        return newDoAfterAnalyzeIntelEvent;
    }
    public DoAfterAnalyzeIntelEvent(IntelComponent intel)
    {
        Intel = intel;
    }
}
