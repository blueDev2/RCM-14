using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Shared._RMC14.Xenonids.Egg.FragileEggGenerator;

/// <summary>
/// Fragile Eggs check if the planter has a <see cref="XenoFragileEggGeneratorSystem"/> component
/// if not, decay in a short amount of time.
/// </summary>
public sealed partial class XenoFragileEggComponent : Component
{
    [DataField]
    public float UnsustainedEggDecay = 60f;
}
