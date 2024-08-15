using Godot;

public class DotStatusEffect : StatusEffect
{
	protected Timer _tickTimer;
	public virtual void OnTargetDied() { }

	public override void StartStatusEffect()
	{
		base.StartStatusEffect();
		if (Target.IsDead) return;
		
		_tickTimer = Utils.CreateTimer(Target, HandleStatusEffect, StatusEffectData.TickPerEverySecond, false);
		_tickTimer?.Start();
		
		Target.OnCharacterDeadEvent -= OnTargetDied;
		Target.OnCharacterDeadEvent += OnTargetDied;
	}

	public override void HandleStatusEffect()
	{
		// Tick damage
		Target.DealDamageToCharacter(StatusEffectData.Damage, StatusEffectData.DamageType);
	}

	public override void OnStatusEffectEnd()
	{
		base.OnStatusEffectEnd();
		Utils.DestroyTimer(_tickTimer);
		if (Target != null)
			Target.OnCharacterDeadEvent -= OnTargetDied;
	}
}
