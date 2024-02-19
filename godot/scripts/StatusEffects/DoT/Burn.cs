using Godot;

public class Burn : DotStatusEffect
{
    public Burn(
        Node2D source,
        string statusEffectId,
        string statusEffectName,
        string statusEffectDesc,
        bool isStackable,
        int numberOfStacks,
        float duration,
        float damage,
        DamageTypes damageType,
        float tickPerSec
    ) : base (
        source,
        statusEffectId,
        statusEffectName,
        statusEffectDesc,
        isStackable,
        numberOfStacks,
        duration,
        damage,
        damageType,
        tickPerSec
    )
    {    }

    public override void OnTargetDied()
    {
        base.OnTargetDied();
        // When the target died, spread to other closeby targets
    }

}