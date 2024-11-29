using System;
using System.Collections.Generic;
using Godot;

public class Static : StatusEffect
{
    private VfxChainLightning _chainLightning;

    private List<Node2D> _lightningStrikesToDispose = new List<Node2D>();

    // ######################################################
    // CONSTRUCTOR
    // ######################################################
    #region CONSTRUCTOR

    public Static(StatusEffectData statusEffectData)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(statusEffectData);
        }
        catch (ArgumentNullException e)
        {
            LoggingUtils.Error($"{nameof(Static)} Status effect data is null");
            throw;
        }
        
        StatusEffectData = statusEffectData;

        _chainLightning = (VfxChainLightning) Scenes.VfxChainLightning.Instantiate();
        _chainLightning.ReportEnemies += LightningStrikeTargets;
    }
    #endregion

    // ######################################################
    // STATUS EFFECT TIMELINE
    // ######################################################
    #region STATUS EFFECT TIMELINE
    public override void StartStatusEffect()
    {
        base.StartStatusEffect();
        LoggingUtils.Debug($"Static stacks: {StatusEffectData.NumberOfStacks}");
		if (StatusEffectData.NumberOfStacks != 0 && StatusEffectData.NumberOfStacks % 2 == 0)
		{
            _chainLightning.Position = Target.Position;
            if (_chainLightning.GetParent() == null)
            {
                UtilGetter.GetVfxParentNode().AddChild(_chainLightning);
            }
            // Trigger the lightning strike
			_chainLightning.ScanForEnemies();
		}

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
    public void LightningStrikeTargets(List<Enemy> enemies)
    {
        var vfxRootNode = UtilGetter.GetVfxParentNode();
        for (int i = 0; i < enemies.Count - 1; i++)
        {
            var currentEnemy = enemies[i];
            var nextEnemy = enemies[i + 1];

            var line = new Line2D();
            line.Points = new Vector2[] { currentEnemy.Position, nextEnemy.Position };
            line.DefaultColor = new Color(1, 1, 0, 0.0f);
            vfxRootNode.AddChild(line);

            var animatedSprite = new AnimatedSprite2D();
            animatedSprite.Position = (currentEnemy.Position + nextEnemy.Position) / 2;
            animatedSprite.AnimationFinished += CleanUpLightningStrike;
            animatedSprite.SpriteFrames = _chainLightning.AnimatedSprite2D.SpriteFrames;
            animatedSprite.Animation = "vfx_chain_lightning";
            animatedSprite.SpriteFrames.SetAnimationLoop("vfx_chain_lightning", false);

            _lightningStrikesToDispose.Add(line);
            _lightningStrikesToDispose.Add(animatedSprite);

            // So from what I can understand
            // This angle is between the 2 vectors has a starting point from origin
            // and to get the rotation of the line between the 2 points you need to rotate by an extra 90 degrees (Pi/2)
            var angle = currentEnemy.Position.AngleToPoint(nextEnemy.Position);
            animatedSprite.Rotation = angle - (Mathf.Pi / 2);
            vfxRootNode.AddChild(animatedSprite);

            currentEnemy.DealDamageToCharacter(CalculateTotalDamage(), DamageTypes.Electric);
            // Also deal damage to the last character because there's no iteration for it
            if (i == enemies.Count - 2)
                nextEnemy.DealDamageToCharacter(CalculateTotalDamage(), DamageTypes.Electric);

            animatedSprite.Play();
        }
    }

    public override void OnTargetDied()
    {
        base.OnTargetDied();
        _chainLightning.QueueFree();
    }
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
    private void CleanUpLightningStrike()
    {
        foreach (var element in _lightningStrikesToDispose)
        {
            element.QueueFree();
        }

        _lightningStrikesToDispose.Clear();
    }
    #endregion
}