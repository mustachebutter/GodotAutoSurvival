using Godot;

public class DotStatusEffect : StatusEffect
{
    protected Timer _tickTimer;
    public float Damage { get; private set; } = 0.0f;
    public float TickPerEverySecond { get; private set; } = 0.0f;
    public DotStatusEffect(
        Node2D source,
        string statusEffectId,
        string statusEffectName,
        string statusEffectDesc,
        bool isStackable,
        int numberOfStacks,
        float duration,
        float damage,
        float tickPerSec
    ) : base (
        source,
        statusEffectId,
        statusEffectName,
        statusEffectDesc,
        isStackable,
        numberOfStacks,
        duration
    )
    {
        Damage = damage;
        TickPerEverySecond = tickPerSec;
    }

    public virtual void OnTargetDied() { }
    public override void StartStatusEffect(BaseCharacter target)
    {
        base.StartStatusEffect(target);
        if (target.IsDead) return;
        
        _tickTimer = Utils.CreateTimer(target, HandleStatusEffect, TickPerEverySecond, false);
        _tickTimer?.Start();
    }

    public override void HandleStatusEffect()
    {
        // Tick damage
        _target.DealDamageToCharacter(Damage);
    }

    public override void OnStatusEffectEnd()
    {
        base.OnStatusEffectEnd();
        Utils.DestroyTimer(_mainTimer);
        Utils.DestroyTimer(_tickTimer);
    }
}