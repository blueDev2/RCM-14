using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Shared._RMC14.Xenonids.Egg.EggGenerator;

/// <summary>
/// Allows a xeno to make fragile eggs using plasma
/// </summary>
[RegisterComponent]
public sealed partial class XenoFragileEggGeneratorComponent : Component
{
    public EntProtoId FragileEggPrototype = "XenoEggFragile";

    public Queue<EntityUid> SustainedEggs = new();

    [DataField]
    public int MaxSustainableEggCount = 4;

    [DataField]
    public int MaxSustainableEggDistance = 14;

    [DataField]
    public float SustainedEggDecay = 300f;
}
