using System;
using Godot;

public class DotStatusEffect : StatusEffect
{
	public const float STATUS_EFFECT_DAMAGE_MULTIPLIER = 0.1f;
	protected Timer _tickTimer;

	public override void StartStatusEffect()
	{
		base.StartStatusEffect();
		
		_tickTimer = Utils.CreateTimer(Target, HandleStatusEffect, StatusEffectData.TickPerEverySecond, false);
		_tickTimer?.Start();
		
		Target.OnCharacterDeadEvent -= OnTargetDied;
		Target.OnCharacterDeadEvent += OnTargetDied;
	}

	public virtual float CalculateTotalDamage()
	{
		if (SourceCharacter == null)
		{
			LoggingUtils.Error("Source character for status effect is null, will have 0 character damage");
		}

		float characterDamage = SourceCharacter == null ? 
			0
			: SourceCharacter.CharacterStatComponent.GetCompleteStatFromName("Attack").totalValue;

		return (float) Math.Round(
			StatusEffectData.Damage + (characterDamage * STATUS_EFFECT_DAMAGE_MULTIPLIER),
			2
		);
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
