using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Server._RMC14.Roles.LarvaQueue;
/// <summary>
/// Indicates that a specific mind is within the larva queue and what it's priority is.
/// Lowest priority value gets served first.
/// </summary>
public sealed partial class InLarvaQueueComponent : Component
{
    public int Priority = 0;
}
