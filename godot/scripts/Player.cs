using Godot;
using System;
using System.Collections.Generic;

public partial class Player : BaseCharacter
{

	public override void _Ready()
	{
		base._Ready();
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = new Vector2();

		if (Input.IsActionPressed("Up"))
		{
			velocity.Y -= 1;
		}

		if (Input.IsActionPressed("Down"))
		{
			velocity.Y += 1;
		}

		if (Input.IsActionPressed("Left"))
		{
			velocity.X -= 1;
		}

		if (Input.IsActionPressed("Right"))
		{
			velocity.X += 1;
		}

		var animationTree = GetNode<AnimationTree>("AnimationTree");
		var stateMachine = (AnimationNodeStateMachinePlayback) animationTree.Get("parameters/playback");

		if (velocity == Vector2.Zero)
		{	
			// stateMachine.Travel("Idle");
			animationTree.Set("parameters/conditions/idle", true);
			animationTree.Set("parameters/conditions/isWalking", false);
		}
		else
		{
			animationTree.Set("parameters/conditions/idle", false);
			animationTree.Set("parameters/conditions/isWalking", true);

			animationTree.Set("parameters/Idle/blend_position", velocity);
			animationTree.Set("parameters/Walk/blend_position", velocity);
		}
		

		// Normalized the Vector
		velocity = velocity.Normalized() * Speed;
		Velocity = velocity;
		MoveAndSlide();

		if (Input.IsActionJustPressed("Action"))
		{
			GD.Print($"PLAYER - {Position}");
		}

	}

	public void FireProjectileAtTarget(Node2D closestTarget, Projectile projectile, string projectileType)
	{
		GD.PrintRich("[color=green]HERE![/color]");

		projectile.AnimationName = projectileType;
		projectile.ShootAtTarget(Position, closestTarget.Position, AttackRange);
	}
}
