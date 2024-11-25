using System;
using Godot;

public class DotStatusEffect : StatusEffect
{
	protected Timer _tickTimer;

	public override void StartStatusEffect()
	{
		base.StartStatusEffect();
		
		_tickTimer = Utils.CreateTimer(Target, HandleStatusEffect, StatusEffectData.TickPerEverySecond, false);
		_tickTimer?.Start();
		
		Target.OnCharacterDeadEvent -= OnTargetDied;
		Target.OnCharacterDeadEvent += OnTargetDied;
	}

	public override void HandleStatusEffect()
	{
		// Tick damage
		Target.DealDamageToCharacter(CalculateTotalDamage(), StatusEffectData.DamageType);
	}

	public override void OnStatusEffectEnd()
	{
		base.OnStatusEffectEnd();
		if (Target != null)
		{
			Target.OnCharacterDeadEvent -= OnTargetDied;
			Utils.DestroyTimer(_tickTimer);
		}
		
		//!!! This should be at the bottom of inheritance
		//!!! since this is called last.
		Target = null;
		SourceCharacter = null;
	}
}
