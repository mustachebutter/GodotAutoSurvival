using Godot;

public class DotStatusEffect : StatusEffect
{
	protected Timer _tickTimer;
	public float Damage { get; protected set; } = 0.0f;
	public DamageTypes DamageType { get; protected set;} = DamageTypes.Normal;
	public float TickPerEverySecond { get; protected set; } = 0.0f;

	public virtual void OnTargetDied() { }

	public override void StartStatusEffect()
	{
		base.StartStatusEffect();
		if (Target.IsDead) return;
		
		_tickTimer = Utils.CreateTimer(Target, HandleStatusEffect, TickPerEverySecond, false);
		_tickTimer?.Start();
		
		Target.OnCharacterDeadEvent += OnTargetDied;
	}

	public override void HandleStatusEffect()
	{
		// Tick damage
		Target.DealDamageToCharacter(Damage, DamageType);
	}

	public override void OnStatusEffectEnd()
	{
		base.OnStatusEffectEnd();
		Target.StatusEffectComponent.ClearEffect(this);
		Target.VisualEffectComponent.ClearVisualEffect();
		Target = null;
		Utils.DestroyTimer(MainTimer);
		Utils.DestroyTimer(_tickTimer);
	}
}
