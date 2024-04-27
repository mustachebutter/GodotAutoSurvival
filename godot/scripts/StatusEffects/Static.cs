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
        
		if (NumberOfStacks == 3)
		{
            _chainLightning.Position = Target.Position;
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
            line.DefaultColor = new Color(0, 1, 1, 0.5f);
            vfxRootNode.AddChild(line);

            var animatedSprite = new AnimatedSprite2D();
            animatedSprite.Position = (currentEnemy.Position + nextEnemy.Position) / 2;
            line.AddChild(animatedSprite);
            animatedSprite.AnimationFinished += CleanUpLightningStrike;
            animatedSprite.SpriteFrames = _chainLightning.AnimatedSprite2D.SpriteFrames;
            animatedSprite.Animation = "vfx_chain_lightning";
            animatedSprite.SpriteFrames.SetAnimationLoop("vfx_chain_lightning", false);
            animatedSprite.Play();

            _lightningStrikesToDispose.Add(line);
            _lightningStrikesToDispose.Add(animatedSprite);
        }
    }

    private void CleanUpLightningStrike()
    {
        foreach (var element in _lightningStrikesToDispose)
        {
            element.QueueFree();
            // _lightningStrikesToDispose.Remove(element);
            GD.Print(_lightningStrikesToDispose.Count);
        }
    }

}