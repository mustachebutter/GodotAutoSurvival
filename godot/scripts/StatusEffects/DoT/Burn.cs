using Godot;

public class Burn : DotStatusEffect
{
    public Burn(
        Node2D source,
        string statusEffectId,
        string statusEffectName,
        string statusEffectDesc,
        string visualEffectName,
        bool isStackable,
        int numberOfStacks,
        float duration,
        float damage,
        DamageTypes damageType,
        float tickPerSec
    )
    {
        Source = source;
        StatusEffectId = statusEffectId;
        StatusEffectName = statusEffectName;
        StatusEffectDesc = statusEffectDesc;
        VisualEffectName = visualEffectName;
        IsStackable = isStackable;
        NumberOfStacks = numberOfStacks;
        Duration = duration;
        Damage = damage;
		DamageType = damageType;
		TickPerEverySecond = tickPerSec;

    }

    public override void OnTargetDied()
    {
        base.OnTargetDied();
        // When the target died, spread to other closeby targets
    }

}