using Godot;
using Godot.Collections;
using System;
using System.Net.Http.Json;


public partial class Grunt : Enemy
{
	private float _orbitSpeed = 1.0f;
	private float _orbitRadius = 50.0f;
	private float _angle =  -(Mathf.Pi / 2);
	
	#region GODOT
	public override void _Ready()
	{
		base._Ready();
		AssignAnimationLibrary("Enemy_AnimationLibrary", SavedAnimationLibrary.EnemyAnimationLibrary);

		var overrideStats = new Dictionary<string, float>
		{ 
			{ "AttackRange", 50.0f },
			{ "Speed", 100.0f },
			{ "Health", 100.0f },
		};

		SetUpEnemy(overrideStats, out float attackRange);

		StartAttackTimer();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

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

	public override void ResetAttack()
	{
		base.ResetAttack();
		HitDetectionArea2D.Position = new Vector2(0.0f, -(CharacterStatComponent.GetCompleteStatFromName("AttackRange").totalValue / 2));
		_angle =  -(Mathf.Pi / 2);

	}
	#endregion

	#region EVENT HANDLING
	#endregion

	#region HELPERS
	public override void SetUpEnemy(Dictionary<string, float> overrideStats, out float attackRange)
	{
		base.SetUpEnemy(overrideStats, out attackRange);

		HitDetectionArea2D.Position = new Vector2(0.0f, -(attackRange));
		_orbitRadius = attackRange;

	}
	#endregion

	#region CLEANUP
	#endregion


}

