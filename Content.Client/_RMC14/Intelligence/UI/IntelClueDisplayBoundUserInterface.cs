using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Content.Client._RMC14.Intelligence.UI;

[UsedImplicitly]
public sealed partial class IntelClueDisplayBoundUserInterface : BoundUserInterface
{
    private IntelClueDisplayWindow? _window;
    public IntelClueDisplayBoundUserInterface(EntityUid owner, Enum key) : base(owner, key)
    {
    }

    protected override void Open()
    {
        base.Open();
        _window = new IntelClueDisplayWindow(this);

        _window.OnClose += Close;
        _window.OpenCentered();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing) return;
        _window?.Dispose();
    }
}
