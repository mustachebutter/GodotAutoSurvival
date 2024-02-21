using Godot;

public class DotStatusEffect : StatusEffect
{
	protected Timer _tickTimer;
	public float Damage { get; private set; } = 0.0f;
	public DamageTypes DamageType { get; private set;} = DamageTypes.Normal;
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
		DamageTypes damageType,
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
		DamageType = damageType;
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
		Target.DealDamageToCharacter(Damage, DamageTypes.Fire);
		GD.PrintErr("Tick");
	}

	public override void OnStatusEffectEnd()
	{
		base.OnStatusEffectEnd();
		GD.PrintRich("[color=red] End Status effect [/color]");
		// IF oneshot then I don't think we need to do this
		// Utils.DestroyTimer(MainTimer);
		// Utils.DestroyTimer(_tickTimer);
	}
}
