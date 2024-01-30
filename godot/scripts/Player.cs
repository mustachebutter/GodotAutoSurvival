using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
	private Area2D _area2D;
	[Export]
	public const float Speed = 100.0f;

	public float Range = 500.0f;

	public float AttackTimer = 2.0f;
	public float AttackSpeed = 1.0f;
	public float AttackRange = 300.0f;

	public override void _Ready()
	{
		_area2D = GetNode<Area2D>("Area2D");
		var circle = (CircleShape2D) _area2D.GetNode<CollisionShape2D>("CollisionShape2D").Shape;
		circle.Radius = AttackRange / 2;
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

	public void FireProjectileAtTarget(Projectile projectile, string projectileType)
	{
		Node2D closestNode = Utils.FindClosestTarget(Position, _area2D);

		if (closestNode == null) return;

		projectile.AnimationName = projectileType;
		projectile.ShootAtTarget(Position, closestNode.Position, AttackRange);
	}
}
