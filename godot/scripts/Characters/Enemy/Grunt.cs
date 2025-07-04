using Godot;
using System;
using System.Net.Http.Json;
using System.Collections.Generic;



public partial class Grunt : Enemy
{	
	private readonly Dictionary<string, float> OVERRIDE_STATS = new Dictionary<string, float>
	{
		{ "AttackRange", 50.0f },
		{ "Speed", 100.0f },
		{ "Health", 100.0f },
	};

	#region GODOT
	public Grunt()
	{
		_overrideStats = OVERRIDE_STATS;
		Shape2D = new CircleShape2D();
	}
	public override void _Ready()
	{
		base._Ready();
		AssignAnimationLibrary("Enemy_AnimationLibrary", SavedAnimationLibrary.GruntAnimationLibrary);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		if (GetSlideCollisionCount() > 0)
		{
			for (int i = 0; i < GetSlideCollisionCount() - 1; i++)
			{
				KinematicCollision2D collision2D = GetSlideCollision(i);
				RigidBody2D otherBody = collision2D.GetCollider() as RigidBody2D;

				if (otherBody != null)
				{
					Vector2 knockbackDirection = collision2D.GetNormal();
					otherBody.ApplyCentralImpulse(knockbackDirection * 100.0f);
				}
			}
		}
	}
	#endregion

	#region ACTION
	#endregion

	#region EVENT HANDLING
	#endregion

	#region HELPERS
	public override void SetUpEnemy()
	{
		base.SetUpEnemy();

		Shape2D.ResourceLocalToScene = true;
		(Shape2D as CircleShape2D).Radius = CharacterStatComponent.GetCompleteStatFromName("AttackRange").totalValue / 10;
		_orbitRadius = _attackRangeCircle.Radius;

		HitDetectionArea2D.GetNode<CollisionShape2D>("CollisionShape2D").Shape = Shape2D;
	}
	#endregion

	#region CLEANUP
	#endregion


}
