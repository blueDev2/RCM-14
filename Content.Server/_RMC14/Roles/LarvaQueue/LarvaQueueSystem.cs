using Content.Shared._RMC14.Mobs;
using Content.Shared.Ghost;
using Content.Shared.Mind;
using Content.Shared.Mind.Components;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Server._RMC14.Roles.LarvaQueue;

public sealed partial class LarvaQueueSystem : EntitySystem
{
    private List<EntityUid> _larvaQueue = new();
    private TimeSpan _nextUpdate = new();
    private TimeSpan _stepLength = new(0, 1, 0);

    // Whenever a player is added to queue, their priority is set to _newPriority
    private int _newPriority = 0;

    [Dependency] private readonly IGameTiming _time = default!;
    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (_larvaQueue.Count == 0)
        {
            _newPriority = 0;
        }

        if (_time.CurTime >= _nextUpdate)
        {
            _newPriority++;
        }
    }
    /// <summary>
    /// Offers larva to the first awake, ghosted player with lowest priority.
    /// </summary>
    /// <param name="larva"></param>
    /// <returns>Returns false if no eligible player is found to offer the larva to</returns>
    private bool OfferLarvaToPriorityPlayer(EntityUid larva)
    {
        foreach (var curMind in _larvaQueue)
        {
            if (!TryComp(curMind, out MindComponent? mindComp))
            {
                // This should never happen
                continue;
            }

            // SSD players will not be provided offer
            if (mindComp.Session is null)
            {
                continue;
            }

            var playerEnt = mindComp.OwnedEntity;
            if (!HasComp<CMGhostComponent>(playerEnt))
            {
                continue;
            }
            OfferLarva(larva, curMind);
            return true;
        }
        return false;
    }

    private void OfferLarva(EntityUid larva, EntityUid playerMind)
    {

    }
    public bool UpdatePriority(EntityUid mindEnt, int newPriority)
    {
        var index = _larvaQueue.IndexOf(mindEnt);
        if (!(HasComp<MindComponent>(mindEnt)) || index == -1)
        {
            return false;
        }

        return UpdatePriority(index, newPriority);
    }
    public bool UpdatePriority(int index, int newPriority)
    {
        if (_larvaQueue.Count <= index)
        {
            return false;
        }
        var mindEnt = _larvaQueue[index];
        if (!TryComp(mindEnt, out MindComponent? mindComp))
        {
            return false;
        }
        var newLarvaQueueComponent = EnsureComp<InLarvaQueueComponent>(mindEnt);
        newLarvaQueueComponent.Priority = newPriority;
        _larvaQueue.RemoveAt(index);
        AddMindToQueue((mindEnt, mindComp), null);
        return true;
    }

    /// <summary>
    /// Add "ent" to the larva queue in such a way that the queue maintains order from smallest to largest priority
    /// </summary>
    /// <param name="ent">Mind entity being added to the larva queue</param>
    public bool AddMindToQueue(Entity<MindComponent> ent)
    {
        return AddMindToQueue(ent, _newPriority);
    }
    /// <summary>
    /// Add "ent" to the larva queue in such a way that the queue maintains order from smallest to largest priority
    /// </summary>
    /// <param name="ent">Mind entity being added to the larva queue</param>
    /// <param name="defaultPriority"> This is set to the priority of the mind
    /// IF the mind does not currently have an <see cref="InLarvaQueueComponent"/> component on.</param>
    public bool AddMindToQueue(Entity<MindComponent> ent, int? defaultPriority)
    {
        InLarvaQueueComponent? newLarvaQueueComp = null;
        if (!HasComp<InLarvaQueueComponent>(ent.Owner))
        {
            if (defaultPriority is null)
            {
                return false;
            }
            newLarvaQueueComp = AddComp<InLarvaQueueComponent>(ent.Owner);
            newLarvaQueueComp.Priority = (int)defaultPriority;
        }
        else
        {
            newLarvaQueueComp = Comp<InLarvaQueueComponent>(ent.Owner);
        }
        for (var i = 0; i < _larvaQueue.Count; i++)
        {
            var curMind = _larvaQueue[i];
            if (!TryComp(curMind, out InLarvaQueueComponent? curLarvaQueueComp))
            {
                // This should never happen
                continue;
            }
            if (newLarvaQueueComp.Priority < curLarvaQueueComp.Priority)
            {
                _larvaQueue.Insert(i, ent.Owner);
                return true;
            }

        }
        _larvaQueue.Append(ent.Owner);
        return true;
    }
}
