using System.Collections.Generic;
using Godot;

public class Burn : DotStatusEffect
{
    private VfxBurnExplosion _burnExplosion;
    public Burn(
        Node2D source,
        StatusEffectData statusEffectData
    )
    {
        Source = source;
        StatusEffectId = statusEffectData.StatusEffectId;
        StatusEffectName = statusEffectData.StatusEffectName;
        StatusEffectDesc = statusEffectData.StatusEffectDesc;
        VisualEffectName = statusEffectData.VisualEffectName;
        IsStackable = statusEffectData.IsStackable;
        NumberOfStacks = statusEffectData.NumberOfStacks;
        Duration = statusEffectData.Duration;
        Damage = statusEffectData.Damage;
		DamageType = statusEffectData.DamageType;
		TickPerEverySecond = statusEffectData.TickPerEverySecond;

        // Initialize Explosion
        _burnExplosion = (VfxBurnExplosion) Scenes.VfxBurnExplosion.Instantiate();
        _burnExplosion.ReportEnemies += AffectSideTargets;
    }

    public override void StartStatusEffect()
    {
        base.StartStatusEffect();
    }

    public override void HandleStatusEffect()
	{
        base.HandleStatusEffect();
	}

    public override void OnStatusEffectEnd()
    {
        base.OnStatusEffectEnd();

    }

    public override void OnTargetDied()
    {
        base.OnTargetDied();
        // When the target died, spread to other closeby targets
        //Add to tree
        Target.GetTree().Root.GetNode("Node2D").GetNode("VFXParentNode").AddChild(_burnExplosion);
        _burnExplosion.Position = Target.Position;
        
        var explosionAnimatedSprite = _burnExplosion.AnimatedSprite2D;
        explosionAnimatedSprite.AnimationFinished += Target.DestroyCharacter;
        explosionAnimatedSprite.AnimationFinished += CleanUpBurnExplosion;
        explosionAnimatedSprite.SpriteFrames.SetAnimationLoop("vfx_burn_explosion", false);
        explosionAnimatedSprite.Play();

        // Scan for nearby targets
        _burnExplosion.ScanForEnemies(Target);        
        // Should theoretically clean up status effect when the target dies
    }

    public void AffectSideTargets(List<Enemy> enemies)
    {
        if (enemies.Count > 0)
        {
            foreach (var enemy in enemies)
            {
                // Create new Burn instance
                var burn = new Burn(
                    enemy,
                    StatusEffectParsedData.GetData("Status_DOT_Burn")
		        );

                // Deals damage to nearby targets
                // Apply debuffs to nearby targets
                enemy.StatusEffectComponent.ApplyEffectToCharacter(burn);
                enemy.DealDamageToCharacter(burn.Damage, DamageTypes.Normal);
            }
        }

    }

    public void CleanUpBurnExplosion()
    {
        _burnExplosion.QueueFree();
    }

    ~Burn()
    {
         
    }
}