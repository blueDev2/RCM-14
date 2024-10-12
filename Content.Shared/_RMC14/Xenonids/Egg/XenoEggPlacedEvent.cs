using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Shared._RMC14.Xenonids.Egg;

/// <summary>
/// Event raised on a xeno egg when placed by a xeno
/// </summary>
[Serializable, NetSerializable]
public sealed partial class XenoEggPlacedEvent : EntityEventArgs
{
    public NetEntity Placer;

    public XenoEggPlacedEvent(NetEntity placer)
    {
        Placer = placer;
    }
}
