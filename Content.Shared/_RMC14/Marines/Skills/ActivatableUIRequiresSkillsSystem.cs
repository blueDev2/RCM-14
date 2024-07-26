using Content.Shared.Popups;
using Content.Shared.UserInterface;

namespace Content.Shared._RMC14.Marines.Skills;

public sealed partial class ActivatableUIRequiresSkillsSystem : EntitySystem
{
    [Dependency] private readonly SkillsSystem _skillsSystem = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ActivatableUIRequiresSkillsComponent, ActivatableUIOpenAttemptEvent>(OnActivate);
    }

    private void OnActivate(EntityUid ent, ActivatableUIRequiresSkillsComponent comp, ActivatableUIOpenAttemptEvent args)
    {
        if (args.Cancelled)
        {
            return;
        }

        if (TryComp(args.User, out SkillsComponent? skillsComp) &&
            _skillsSystem.HasSkills((ent, skillsComp), in comp.RequiredSkills))
        {
            return;
        }
        args.Cancel();
        _popup.PopupClient(Loc.GetString(comp.PopupMessage), args.User);
    }
}
