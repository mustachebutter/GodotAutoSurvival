using Godot;
using System;
using System.Net.Http.Json;
using System.Collections.Generic;



public partial class Grunt : Enemy
{
	private float _orbitSpeed = 1.0f;
	private float _orbitRadius = 50.0f;
	private float _angle =  -(Mathf.Pi / 2);
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
	}
	public override void _Ready()
	{
		base._Ready();
		AssignAnimationLibrary("Enemy_AnimationLibrary", SavedAnimationLibrary.EnemyAnimationLibrary);
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
	public override void Attack()
	{
		// Swing and do hit detection
		if (_isFacingRight)
			_angle += _orbitSpeed * (float) GetProcessDeltaTime();
		else
			_angle -= _orbitSpeed * (float) GetProcessDeltaTime();

		if (_angle > (Mathf.Pi / 2))
		{
			_angle = -(Mathf.Pi / 2);
		}

		float x = GlobalPosition.X + _orbitRadius * Mathf.Cos(_angle);
		float y = GlobalPosition.Y + _orbitRadius * Mathf.Sin(_angle);

		HitDetectionArea2D.GlobalPosition = new Vector2(x, y);
		base.Attack();
	}

	public override BTNodeState ResetAttack()
	{
		BTNodeState status = base.ResetAttack();
		if (status == BTNodeState.Success)
		{
			HitDetectionArea2D.Position = new Vector2(0.0f, -(CharacterStatComponent.GetCompleteStatFromName("AttackRange").totalValue / 2));
			_angle = -(Mathf.Pi / 2);
			return BTNodeState.Success ;
		}

		LoggingUtils.Error("Grunt: Failed to Reset Attack");
		return BTNodeState.Failure;
	}
	#endregion

	#region EVENT HANDLING
	#endregion

	#region HELPERS
	public override void SetUpEnemy()
	{
		base.SetUpEnemy();

		var hitDetectionCircle = (CircleShape2D) HitDetectionArea2D.GetNode<CollisionShape2D>("CollisionShape2D").Shape;
		hitDetectionCircle.ResourceLocalToScene = true;
		hitDetectionCircle.Radius = 5.0f;
		HitDetectionArea2D.Position = new Vector2(0.0f, -(_circle.Radius));
		_orbitRadius = _circle.Radius;

	}
	#endregion

	#region CLEANUP
	#endregion


}

