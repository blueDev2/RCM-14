using Content.Server._RMC14.Requisitions;
using Content.Shared._RMC14.Intelligence;
using Robust.Shared.Timing;


namespace Content.Server._RMC14.Intelligence;

public sealed partial class IntelSystem : SharedIntelSystem
{
    [Dependency] private readonly RequisitionsSystem _requisitions = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    private TimeSpan _intelTickLength = new(0, 1, 0);
    private TimeSpan _nextUpdate = new(0, 0, 0);
    //private TimeSpan _intelLastTick = new(0, 0, 0);

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

        var retrievalTargets = new EntityQueryEnumerator<GiveIntelOnRetrievalComponent>();

        while (retrievalTargets.MoveNext(out var ent, out var comp))
        {
            RewardPoints((ent, comp));
        }

    }

    private bool RewardPoints(Entity<GiveIntelOnRetrievalComponent> ent)
    {
        if (ent.Comp.RewardSchedule == RewardSchedule.NEVER)
        {
            return false;
        }
        if (IsWithinDropZone(ent.Owner))
        {
            RewardPoints(ent.Comp.PointReward);
            if (ent.Comp.RewardSchedule == RewardSchedule.SINGLE)
            {
                ent.Comp.RewardSchedule = RewardSchedule.NEVER;
            }
            return true;
        }
        return false;
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
