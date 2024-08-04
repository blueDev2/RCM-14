
using Content.Shared._RMC14.Intelligence.Components;
using Content.Shared._RMC14.Marines.Skills;
using Content.Shared._RMC14.Requisitions;
using Content.Shared.CharacterInfo;
using Content.Shared.DoAfter;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Objectives.Components;
using Content.Shared.Objectives.Systems;
using Content.Shared.Paper;
using Content.Shared.Popups;
using Content.Shared.Standing;
using Content.Shared.UserInterface;
using Robust.Shared.Prototypes;

namespace Content.Shared._RMC14.Intelligence;

public abstract partial class SharedIntelSystem : EntitySystem
{
    //[Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    //[Dependency] private readonly SkillsSystem _skills = default!;
    //[Dependency] private readonly SharedPopupSystem _popupSystem = default!;
    //[Dependency] private readonly SharedUserInterfaceSystem _uiSystem = default!;
    //[Dependency] private readonly SharedObjectivesSystem _objectivesSystem = default!;

    public enum IntelClueDisplayUIKey : byte
    {
        Key
    }

    [Dependency] private readonly SharedTransformSystem _transform = default!;


    public Skills RequiredIntelSkills = new() { Intel = 2 };
    protected const decimal IntelToReqMultiplier = 1000M;


    public override void Initialize()
    {
        base.Initialize();

    }



    public List<Entity<GiveIntelOnRetrievalComponent>> GetAllRetrievalTargetsInDropZones()
    {
        List<Entity<GiveIntelOnRetrievalComponent>> res = new();

        var dropZones = new EntityQueryEnumerator<IntelDropZoneComponent>();
        var retrievalTargets = new EntityQueryEnumerator<GiveIntelOnRetrievalComponent>();

        while (dropZones.MoveNext(out var dropZoneEnt, out var dropZoneComp))
        {
            if (!TryComp(dropZoneEnt, out TransformComponent? dropZoneTransform))
            {
                continue;
            }
            var (minX, minY) = _transform.GetMapCoordinates(dropZoneTransform);
            var maxX = minX + dropZoneComp.Width;
            var maxY = minY + dropZoneComp.Height;

            while (retrievalTargets.MoveNext(out var retrievalEnt, out var retrievalComp))
            {
                if (!TryComp(retrievalEnt, out TransformComponent? retrievalTransform))
                {
                    continue;
                }
                var (targetX, targetY) = _transform.GetMapCoordinates(retrievalTransform);

                if ((targetX >= minX && targetX <= maxX) &&
                    (targetY >= minY && targetY <= maxY))
                {
                    res.Add((retrievalEnt, retrievalComp));
                }
            }
        }

        return res;
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
}
