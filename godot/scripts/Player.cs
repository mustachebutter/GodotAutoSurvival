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
	public float AttackRange = 500.0f;

	public override void _Ready()
	{
		_area2D = GetNode<Area2D>("Area2D");
		var circle = (CircleShape2D) _area2D.GetNode<CollisionShape2D>("CollisionShape2D").Shape;
		circle.Radius = 100;
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

	public Enemy FireProjectileAtTarget()
	{
		var enemyNodes = _area2D.GetOverlappingBodies();
		if (enemyNodes == null) return null;


		Node2D closestNode = null;
		float closestDistance = 0.0f;
		foreach (var e in enemyNodes)
		{
			float distance = Position.DistanceTo(e.Position);
			if (closestNode == null || distance < closestDistance)
			{
				closestDistance = distance;
				closestNode = e;
			}
		}
		GD.Print($"Closest Distance - {closestDistance}");
		// GD.Print($"Closest Node - {closestNode.Name}");
		return (Enemy) closestNode;
	}
}
