using Godot;

public class DotStatusEffect : StatusEffect
{
	protected Timer _tickTimer;
	public float Damage { get; protected set; } = 0.0f;
	public DamageTypes DamageType { get; protected set;} = DamageTypes.Normal;
	public float TickPerEverySecond { get; protected set; } = 0.0f;

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
		Target.DealDamageToCharacter(Damage, DamageTypes.Fire);
	}

	public override void OnStatusEffectEnd()
	{
		GD.Print("This shouldn't be called if it should be refreshed");
		base.OnStatusEffectEnd();
		Target.StatusEffectComponent.ClearEffect(this);
		Target.VisualEffectComponent.ClearVisualEffect();
		Target = null;
		// IF oneshot then I don't think we need to do this
		// Utils.DestroyTimer(MainTimer);
		Utils.DestroyTimer(_tickTimer);
	}
}
