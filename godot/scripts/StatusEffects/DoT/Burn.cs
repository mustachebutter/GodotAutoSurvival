using System.Collections.Generic;
using Godot;

public class Burn : DotStatusEffect
{
    private VfxBurnExplosion _burnExplosion;
    public Burn(
        Node2D source,
        string statusEffectId,
        string statusEffectName,
        string statusEffectDesc,
        string visualEffectName,
        bool isStackable,
        int numberOfStacks,
        float duration,
        float damage,
        DamageTypes damageType,
        float tickPerSec
    )
    {
        Source = source;
        StatusEffectId = statusEffectId;
        StatusEffectName = statusEffectName;
        StatusEffectDesc = statusEffectDesc;
        VisualEffectName = visualEffectName;
        IsStackable = isStackable;
        NumberOfStacks = numberOfStacks;
        Duration = duration;
        Damage = damage;
		DamageType = damageType;
		TickPerEverySecond = tickPerSec;

        // Initialize Explosion
        _burnExplosion = (VfxBurnExplosion) Scenes.VfxBurnExplosion.Instantiate();
        _burnExplosion.ReportEnemies += AffectSideTargets;
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
                    "Status_DOT_Burn", "Burn", "Burns the target, ticks damage every x seconds", "vfx_burn",
                    false, 0, 3.0f,
                    30.0f, DamageTypes.Fire, 0.3f
		        );

                // Deals damage to nearby targets
                // Apply debuffs to nearby targets
                enemy.StatusEffectComponent.ApplyEffectToCharacter(burn);
                enemy.DealDamageToCharacter(Damage, DamageTypes.Normal);
            }
        }

    }
    public override void OnStatusEffectEnd()
    {
        base.OnStatusEffectEnd();

    }

    public void CleanUpBurnExplosion()
    {
        _burnExplosion.QueueFree();
    }

    ~Burn()
    {
         
    }
}