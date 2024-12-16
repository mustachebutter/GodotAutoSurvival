using System;
using Godot;

public partial class Grunt : Enemy
{
	public AnimationTree AnimationTree { get; set; }
	public AnimatedSprite2D AnimatedSprite2D { get; set; }
	public AnimationPlayer AnimationPlayer { get; set; }
	private AnimationNodeStateMachinePlayback _stateMachine { get; set; }

	public override void _Ready()
	{
		base._Ready();
		AnimationTree = GetNode<AnimationTree>("AnimationTree");
		_stateMachine = (AnimationNodeStateMachinePlayback) AnimationTree.Get("parameters/playback");
		AnimatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		DetectPlayer();

		if (Velocity == Vector2.Zero)
		{	
			AnimationTree.Set("parameters/conditions/isIdle", true);
			AnimationTree.Set("parameters/conditions/isWalking", false);
		}
		else
		{
			AnimationTree.Set("parameters/conditions/isIdle", false);
			AnimationTree.Set("parameters/conditions/isWalking", true);

			// AnimationTree.Set("parameters/Idle/blend_position", Velocity.Normalized().X);
			// AnimationTree.Set("parameters/Walk/blend_position", Velocity.Normalized().X);
		}

		if (Velocity.X > 0)
		{
			AnimatedSprite2D.FlipH = false;
		}
		else if (Velocity.X < 0)
		{
			AnimatedSprite2D.FlipH = true;
		}
	}

	public void DetectPlayer()
	{
		var bodies = Area2D.GetOverlappingBodies();
		if (bodies == null)
		{
			LoggingUtils.Info("No bodies scanned with Area2D");
			return;
		}

		foreach (var bd in bodies)
		{
			if (bd is Player)
			{
				LoggingUtils.Debug("Found player");
				// Attempt to attack
				// AnimationPlayer.Play("Enemy_AnimationLibrary/attack");
				AnimationTree.Set("parameters/conditions/isAttacking", true);
				AnimationTree.Set("parameters/conditions/isIdle", false);
				AnimationTree.Set("parameters/conditions/isWalking", false);
				// AnimationTree.Set("parameters/conditions/isAttacking", true);
				// AnimationTree.Set("parameters/conditions/isAttackingFinished", false);
				// Deal Damage
			}
		}
	}

	private void OnAnimationTreeAnimationFinished(StringName anim_name)
	{
		// Replace with function body.
		LoggingUtils.Debug($"AnimName : {anim_name}");

		if (anim_name == "Enemy_AnimationLibrary/attack")
		{
			AnimationTree.Set("parameters/conditions/isAttackingFinished", true);
		}

	}
}

