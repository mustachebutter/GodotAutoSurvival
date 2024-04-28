using System;
using System.Collections.Generic;
using Godot;

public class Static : StatusEffect
{
    private VfxChainLightning _chainLightning;

    private List<Node2D> _lightningStrikesToDispose = new List<Node2D>();
    public Static(
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

        _chainLightning = (VfxChainLightning) Scenes.VfxChainLightning.Instantiate();
        _chainLightning.ReportEnemies += LightningStrikeTargets;
    }

    public override void StartStatusEffect()
    {
        base.StartStatusEffect();
        
		if (NumberOfStacks != 0 && NumberOfStacks % 3 == 0)
		{
            _chainLightning.Position = Target.Position;
            if (_chainLightning.GetParent() == null)
                Target.GetTree().Root.GetNode("Node2D").GetNode("VFXParentNode").AddChild(_chainLightning);
			// Trigger the lightning strike
			_chainLightning.ScanForEnemies();
		}

    }

    public override void HandleStatusEffect()
    {
        base.HandleStatusEffect();
    }
    
    public override void OnStatusEffectEnd()
    {
        base.OnStatusEffectEnd();
    }

    public void LightningStrikeTargets(List<Enemy> enemies)
    {
        var vfxRootNode = Target.GetTree().Root.GetNode("Node2D").GetNode("VFXParentNode");
        for (int i = 0; i < enemies.Count - 1; i++)
        {
            var currentEnemy = enemies[i];
            var nextEnemy = enemies[i + 1];

            var line = new Line2D();
            line.Points = new Vector2[] { currentEnemy.Position, nextEnemy.Position };
            // line.DefaultColor = new Color(1, 1, 0, 0.5f);
            line.DefaultColor = new Color(1, 1, 0, 0.0f);
            vfxRootNode.AddChild(line);

            var animatedSprite = new AnimatedSprite2D();
            animatedSprite.Position = (currentEnemy.Position + nextEnemy.Position) / 2;
            animatedSprite.AnimationFinished += CleanUpLightningStrike;
            animatedSprite.SpriteFrames = _chainLightning.AnimatedSprite2D.SpriteFrames;
            animatedSprite.Animation = "vfx_chain_lightning";
            animatedSprite.SpriteFrames.SetAnimationLoop("vfx_chain_lightning", false);

            // So from what I can understand
            // This angle is between the 2 vectors has a starting point from origin
            // and to get the rotation of the line between the 2 points you need to rotate by an extra 90 degrees (Pi/2)
            var angle = currentEnemy.Position.AngleToPoint(nextEnemy.Position);
            animatedSprite.Rotation = angle - (Mathf.Pi / 2);
            vfxRootNode.AddChild(animatedSprite);
            animatedSprite.Play();

            currentEnemy.DealDamageToCharacter(0.5f, DamageTypes.Electric);
            // Also deal damage to the last character because there's no iteration for it
            if (i == enemies.Count - 2)
                nextEnemy.DealDamageToCharacter(0.5f, DamageTypes.Electric);

            _lightningStrikesToDispose.Add(line);
            _lightningStrikesToDispose.Add(animatedSprite);
        }
    }

    private void CleanUpLightningStrike()
    {
        foreach (var element in _lightningStrikesToDispose)
        {
            element.QueueFree();
        }

        _lightningStrikesToDispose.Clear();
    }

}