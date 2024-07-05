using Content.Shared._RMC14.Damage;
using Content.Server._RMC14.Medical.Pain;
using Content.Shared.Damage;

namespace Content.Server._RMC14.Medical.Pain;

public abstract class PainSystem : EntitySystem
{
    [Dependency] private CMDamageableSystem _damageableSystem = default!;

    private const float BRUTE_PAIN_MULTIPLIER = 1.0F;
    private const float BURN_PAIN_MULTIPLIER = 1.2F;
    private const float TOX_PAIN_MULTIPLIER = 1.5F;
    private const float OXY_PAIN_MULTIPLIER = 0F;


    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<PainRecipientComponent>();

        while (query.MoveNext(out var uid, out var comp))
        {
            var total_pain = 0;
            total_pain += GetPainFromStandardDamage(uid);
            total_pain += GetPainFromOrganDamage(uid);
            total_pain += GetPainFromLimbs(uid);
        }
    }

    private int GetPainFromStandardDamage(EntityUid entity)
    {
        if (TryComp(entity, out DamageableComponent? comp))
        {
            var total_pain = 0;
            if (comp.DamagePerGroup.TryGetValue("Brute", out var bruteDamage))
            {
                total_pain += (int) (bruteDamage * BRUTE_PAIN_MULTIPLIER);
            }

            if (comp.DamagePerGroup.TryGetValue("Burn", out var burnDamage))
            {
                total_pain += (int) (burnDamage * BURN_PAIN_MULTIPLIER);
            }

            if (comp.DamagePerGroup.TryGetValue("Toxin", out var toxDamage))
            {
                total_pain += (int) (toxDamage * TOX_PAIN_MULTIPLIER);
            }

            if (comp.DamagePerGroup.TryGetValue("Airloss", out var airDamage))
            {
                total_pain += (int) (airDamage * OXY_PAIN_MULTIPLIER);
            }
            return total_pain;
        }
        return 0;
    }

    // Add logic when organ damage are added
    private int GetPainFromOrganDamage(EntityUid entity)
    {
        return 0;
    }

    // Add logic when bone breaks are added
    private int GetPainFromLimbs(EntityUid entity)
    {
        return 0;
    }
}
