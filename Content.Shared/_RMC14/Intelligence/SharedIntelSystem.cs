
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Requisitions;
using Content.Shared.CharacterInfo;
using Content.Shared.DoAfter;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Objectives.Components;
using Content.Shared.Objectives.Systems;
using Content.Shared.Popups;
using Content.Shared.Standing;

namespace Content.Shared._RMC14.Intelligence;

public abstract partial class SharedIntelSystem : EntitySystem
{
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly SkillsSystem _skills = default!;
    [Dependency] private readonly SharedPopupSystem _popupSystem = default!;
    //[Dependency] private readonly SharedObjectivesSystem _objectivesSystem = default!;


    public Skills RequiredIntelSkills = new() { Intel = 2 };
    protected const decimal IntelToReqMultiplier = 1000M;


    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<IntelComponent, InteractHandEvent>(OnInteractIntelInHand);
        SubscribeLocalEvent<IntelComponent, DoAfterAnalyzeIntelEvent>(OnIntelAnalysisCompletion);
    }


    public bool IsWithinDropZone(EntityUid ent)
    {
        if (!TryComp(ent, out TransformComponent? transformComp))
        {
            return false;
        }

        var dropZones = new EntityQueryEnumerator<IntelDropZoneComponent>();
        while (dropZones.MoveNext(out var uid, out var dropZone))
        {
            if (IsWithinDropZone((ent, transformComp), (uid, dropZone)))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsWithinDropZone(Entity<TransformComponent> ent, Entity<IntelDropZoneComponent> dropZone)
    {
        if (!TryComp(dropZone.Owner, out TransformComponent? dropZoneTransform))
        {
            return false;
        }



        return false;
    }

    public void OnIntelAnalysisCompletion(EntityUid ent, IntelComponent comp, DoAfterAnalyzeIntelEvent args)
    {

    }

    public void OnInteractIntelInHand(EntityUid ent, IntelComponent comp, InteractHandEvent args)
    {
        if (!_skills.HasSkills(ent, in RequiredIntelSkills))
        {
            _popupSystem.PopupClient(Loc.GetString("insufficient-intel-skill"), ent);
            return;
        }

        var ev = new DoAfterAnalyzeIntelEvent();
        var doAfter = new DoAfterArgs(EntityManager, ent, comp.AnalyzeDuration, ev, null)
        {
            BreakOnMove = true
        };
        _doAfter.TryStartDoAfter(doAfter);
        _popupSystem.PopupClient(Loc.GetString("start-analyzing-intel"), ent);

    }

    public bool AddIntelItems(int NumberOfItems)
    {
        return false;
    }
}
