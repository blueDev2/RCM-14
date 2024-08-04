using Content.Server._RMC14.Requisitions;
using Content.Server.Paper;
using Content.Shared._RMC14.Intelligence;
using Content.Shared._RMC14.Intelligence.Components;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Mind;
using Content.Shared.Paper;
using Content.Shared.Popups;
using Robust.Shared.Timing;
using System.Text;


namespace Content.Server._RMC14.Intelligence;

public sealed partial class IntelSystem : SharedIntelSystem
{
    [Dependency] private readonly RequisitionsSystem _requisitions = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedUserInterfaceSystem _uiSystem = default!;
    [Dependency] private readonly SharedPopupSystem _popupSystem = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly SkillsSystem _skills = default!;
    [Dependency] private readonly PaperSystem _paper = default!;
    [Dependency] private readonly SharedMindSystem _mind = default!;

    private TimeSpan _intelTickLength = new(0, 1, 0);
    private TimeSpan _nextUpdate = new(0, 0, 0);
    //private TimeSpan _intelLastTick = new(0, 0, 0);


    public override void Initialize()
    {
        base.Initialize();

        // Scan intel with a IntelClueScanner entity
        SubscribeLocalEvent<IntelClueScannerComponent, InteractUsingEvent>(OnInteractIntelWithScanner);
        SubscribeLocalEvent<IntelClueScannerComponent, DoAfterAnalyzeIntelEvent>(OnIntelAnalysisCompletionOnScanner);

        // Scan intel into a IntelClueUpload entity
        SubscribeLocalEvent<IntelComponent, InteractUsingEvent>(OnInteractIntelOnUpload);
        SubscribeLocalEvent<IntelClueUploadComponent, DoAfterAnalyzeIntelEvent>(OnIntelAnalysisCompletionOnUploader);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var curTime = _timing.CurTime;
        if (curTime > _nextUpdate)
        {
            _nextUpdate = curTime + _intelTickLength;
        }
        else
        {
            return;
        }

        var zonedRetrievalTargets = GetAllRetrievalTargetsInDropZones();

        foreach (var item in zonedRetrievalTargets)
        {
            RewardPoints(item);
        }

    }

    private void OnInteractIntelWithScanner(EntityUid ent, IntelClueScannerComponent scannerComp, ref InteractUsingEvent args)
    {
        if (!TryComp(args.Used, out IntelComponent? intelComp))
        {
            return;
        }

        if (!_skills.HasSkills(args.User, in RequiredIntelSkills))
        {
            _popupSystem.PopupClient(Loc.GetString("insufficient-intel-skill"), args.User);
            return;
        }

        args.Handled = true;
        var ev = new DoAfterAnalyzeIntelEvent(intelComp);
        var doAfter = new DoAfterArgs(EntityManager, ent, intelComp.AnalyzeDuration, ev, null)
        {
            BreakOnMove = true
        };
        if (_doAfter.TryStartDoAfter(doAfter))
        {
            _popupSystem.PopupClient(Loc.GetString("start-analyzing-intel"), ent);
        }
    }

    private void OnInteractIntelOnUpload(EntityUid ent, IntelComponent intelComp, ref InteractUsingEvent args)
    {
        if (!TryComp(args.Target, out IntelClueUploadComponent? intelUploadComp))
        {
            return;
        }

        if (!_skills.HasSkills(args.User, in RequiredIntelSkills))
        {
            _popupSystem.PopupClient(Loc.GetString("insufficient-intel-skill"), args.User);
            return;
        }

        args.Handled = true;
        var ev = new DoAfterAnalyzeIntelEvent(intelComp);
        var doAfter = new DoAfterArgs(EntityManager, args.Target, intelComp.AnalyzeDuration, ev, null)
        {
            BreakOnMove = true
        };
        if (_doAfter.TryStartDoAfter(doAfter))
        {
            _popupSystem.PopupClient(Loc.GetString("start-analyzing-intel"), ent);
        }
    }

    private void OnIntelAnalysisCompletionOnScanner(EntityUid ent, IntelClueScannerComponent comp, DoAfterAnalyzeIntelEvent args)
    {
        OnIntelAnalysisCompletion(ent, args.Intel);
    }

    private void OnIntelAnalysisCompletionOnUploader(EntityUid ent, IntelClueUploadComponent comp, DoAfterAnalyzeIntelEvent args)
    {
        OnIntelAnalysisCompletion(ent, args.Intel);
    }

    private void OnIntelAnalysisCompletion(EntityUid ent, IntelComponent intelComp)
    {
        var clueStorageComp = EnsureComp<IntelClueStorageComponent>(ent);
        AddCluesToStorage(intelComp, clueStorageComp);
        if (!intelComp.AwardedPoints)
        {
            RewardPoints(intelComp.PointsOnAnalyze);
            intelComp.AwardedPoints = true;
        }
    }

    private void AddCluesToStorage(IntelComponent intelComp, IntelClueStorageComponent intelClueStorageComp)
    {
        foreach (var clue in intelComp.IntelClues)
        {
            intelClueStorageComp.Clues.Add(clue);
        }
    }

    private bool RewardPoints(Entity<GiveIntelOnRetrievalComponent> ent)
    {
        if (ent.Comp.RewardSchedule == RewardSchedule.NEVER)
        {
            return false;
        }
        RewardPoints(ent.Comp.PointReward);
        if (ent.Comp.RewardSchedule == RewardSchedule.SINGLE)
        {
            ent.Comp.RewardSchedule = RewardSchedule.NEVER;
        }
        return true;
    }

    /// <summary>
    /// Once tech tree is added, points will be rewarded as tech points
    /// For now, they will be directly rewarded as req points
    /// </summary>
    private void RewardPoints(decimal points)
    {
        _requisitions.AddBalance((int) Math.Round(points * IntelToReqMultiplier));
    }
}
