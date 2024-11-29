using System;
using Godot;

public class DotStatusEffect : StatusEffect
{
	protected Timer _tickTimer;

	// ######################################################
	// CONSTRUCTOR
	// ######################################################
	#region CONSTRUCTOR

	#endregion

	// ######################################################
	// STATUS EFFECT TIMELINE
	// ######################################################
	#region STATUS EFFECT TIMELINE
	public override void StartStatusEffect()
	{
		base.StartStatusEffect();
		
		_tickTimer = Utils.CreateTimer(Target, HandleStatusEffect, StatusEffectData.TickPerEverySecond, false);
		_tickTimer?.Start();		
	}

	public override void HandleStatusEffect()
	{
		// Tick damage
		Target.DealDamageToCharacter(CalculateTotalDamage(), StatusEffectData.DamageType);
	}

	public override void EndStatusEffect()
	{
		base.EndStatusEffect();
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
	#endregion

	// ######################################################
	// EVENT HANDLING
	// ######################################################
	#region EVENT HANDLING
	#endregion

	// ######################################################
	// HELPER METHODS
	// ######################################################
	#region HELPER METHODS
	#endregion

	// ######################################################
	// DECONSTRUCTOR/ CLEAN UP
	// ######################################################
	#region DECONSTRUCTOR/ CLEAN UP
	#endregion
}
