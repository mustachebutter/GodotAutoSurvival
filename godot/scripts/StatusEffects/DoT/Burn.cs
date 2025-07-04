using System;
using System.Collections.Generic;
using Godot;

public class Burn : DotStatusEffect
{
	private VfxBurnExplosion _burnExplosion;

	// ######################################################
	// CONSTRUCTOR
	// ######################################################
	#region CONSTRUCTOR

	public Burn(StatusEffectData statusEffectData)
	{
		try
		{
			ArgumentNullException.ThrowIfNull(statusEffectData);
		}
		catch (ArgumentNullException e)
		{
			LoggingUtils.Error($"{nameof(Burn)} Status effect data is null");
			throw;
		}

		StatusEffectData = statusEffectData;
		// Initialize Explosion
		_burnExplosion = (VfxBurnExplosion) Scenes.VfxBurnExplosion.Instantiate();
		_burnExplosion.ReportEnemies += AffectSideTargets;
	}
	#endregion

	// ######################################################
	// STATUS EFFECT TIMELINE
	// ######################################################
	#region STATUS EFFECT TIMELINE
	public override void StartStatusEffect()
	{
		base.StartStatusEffect();
	}

	public override void HandleStatusEffect()
	{
		base.HandleStatusEffect();
	}

	public override void EndStatusEffect()
	{
		base.EndStatusEffect();

	}
	#endregion

	// ######################################################
	// EVENT HANDLING
	// ######################################################
	#region EVENT HANDLING
	public override void OnTargetDied()
	{
		base.OnTargetDied();
		if (Target == null) LoggingUtils.Debug("TARGET NULL");
		// When the target died, spread to other closeby targets
		//Add to tree
		UtilGetter.GetVfxParentNode().AddChild(_burnExplosion);
		_burnExplosion.Position = Target.Position;
		
		var explosionAnimatedSprite = _burnExplosion.AnimatedSprite2D;
		explosionAnimatedSprite.AnimationFinished += Target.Perish;
		explosionAnimatedSprite.AnimationFinished += CleanUpBurnExplosion;
		explosionAnimatedSprite.SpriteFrames.SetAnimationLoop("vfx_burn_explosion", false);
		explosionAnimatedSprite.Play();

		// Scan for nearby targets
		_burnExplosion.ScanForEnemies(Target);        
		// Should theoretically clean up status effect when the target dies
	}
	#endregion

	// ######################################################
	// HELPER METHODS
	// ######################################################
	#region HELPER METHODS
	public void AffectSideTargets(List<Enemy> enemies)
	{
		if (enemies.Count > 0)
		{
			foreach (var enemy in enemies)
			{
				// Create new Burn instance
				var burn = new Burn(
					DataParser.GetStatusEffectData("Status_DOT_Burn")
				);

				// Deals damage to nearby targets
				// Apply debuffs to nearby targets
				enemy.StatusEffectComponent.ApplyEffectToCharacter(burn, SourceCharacter, enemy);
				// TODO: This explosion spread deals base damage of status effect
				enemy.DealDamageToCharacter(burn.StatusEffectData.Damage, burn.StatusEffectData.DamageType);
			}
		}

	}
	#endregion

	// ######################################################
	// DECONSTRUCTOR/ CLEAN UP
	// ######################################################
	#region DECONSTRUCTOR/ CLEAN UP
	public void CleanUpBurnExplosion()
	{
		_burnExplosion.QueueFree();
	}

	~Burn()
	{
		 
	}
	#endregion
}
