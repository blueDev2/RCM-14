using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Shared._RMC14.Xenonids.Egg;

/// <summary>
/// Event raised on xeno placing an xeno egg
/// </summary>
[Serializable, NetSerializable]
public sealed partial class PlacedXenoEggEvent : EntityEventArgs
{
    public NetEntity PlacedEgg;

    public PlacedXenoEggEvent(NetEntity placedEgg)
    {
        PlacedEgg = placedEgg;
    }
}
