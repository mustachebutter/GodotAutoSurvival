using Godot;

public class Burn : DotStatusEffect
{
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

    }

    public override void OnTargetDied()
    {
        base.OnTargetDied();
        // When the target died, spread to other closeby targets
        //Add to tree
        VfxBurnExplosion vfxBurnExplosion = (VfxBurnExplosion) Scenes.VfxBurnExplosion.Instantiate();
        Target.AddChild(vfxBurnExplosion);
        vfxBurnExplosion.PlayVisualEffects();
        // Scan for nearby targets
        var enemies = vfxBurnExplosion.ScanForEnemies(Target);
        GD.Print(enemies);
        if (enemies.Count > 0)
        {
            foreach (var enemy in enemies)
            {
                // Deals damage to nearby targets
                // Apply debuffs to nearby targets
                enemy.StatusEffectComponent.ApplyEffectToCharacter(this);
                enemy.DealDamageToCharacter(Damage, DamageTypes.Normal);
            }
        }
    }

}