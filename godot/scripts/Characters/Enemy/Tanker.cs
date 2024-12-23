using Godot;
using Godot.Collections;

public partial class Tanker : Enemy
{
	public Area2D ChargeArea2D { get; set; }
	private bool _isChargingAbility { get; set; } = false;
	private bool _isAbilityOnCooldown { get; set; } = false;
	private bool _isCharging { get; set; } = false;
	private float _chargeMeter { get; set; } = 0.0f;
	private float _chargeCooldown { get; set; } = 0.0f;
	private float _chargeDistance { get; set; } = 0.0f;
	private Vector2 _previousFramePosition = Vector2.Zero;
	private readonly float CHARGE_DURATION = 1.0f;
	private readonly float CHARGE_COOLDOWN = 10.0f;
	private readonly float CHARGE_DISTANCE = 500.0f;
	
	#region GODOT
	public override void _Ready()
	{
		base._Ready();
		ChargeArea2D = GetNode<Area2D>("ChargeArea2D");
		AssignAnimationLibrary("Enemy_AnimationLibrary", SavedAnimationLibrary.EnemyAnimationLibrary);

		var overrideStats = new Dictionary<string, float>
		{ 
			{ "AttackRange", 150.0f },
			{ "Speed", 40.0f },
			{ "Health", 200.0f },
		};

		SetUpEnemy(overrideStats, out float attackRange);

		StartAttackTimer();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (DetectedPlayer(ChargeArea2D, out Player player)) _isChargingAbility = true;
		else _isChargingAbility = false;

		if (_isChargingAbility && !_isAbilityOnCooldown)
		{
			_chargeMeter += (float) delta;
			if (_chargeMeter >= CHARGE_DURATION)
			{
				_isCharging = true;
			}
		}

		if (_isCharging)
		{
			Ability_Charge();
		}

		if (_isAbilityOnCooldown)
		{
			_chargeCooldown += (float) delta;
			if (_chargeCooldown >= CHARGE_COOLDOWN)
			{
				_isAbilityOnCooldown = false;
				_chargeCooldown = 0.0f;
			}
		}
	}
	#endregion
	
	#region ACTION
	public override void Attack()
	{
		// Swing and do hit detection
		// base.Attack();
	}

	public void Ability_Charge()
	{
		// Pause the attack timer
		_mainTimer.Stop();
		// Play animation of charging up
		// Dash towards target
		CharacterStatComponent.AddStat("Speed", 50.0f, StatTypes.Stat);

		if (MoveTowardsThePlayer())
		{
			LoggingUtils.Debug("Hit a player");
			// TODO: If hit players then knock them back

			ResetCharge();
		}

		_chargeDistance += _previousFramePosition.DistanceTo(Position);
		_previousFramePosition = Position;

		if (_chargeDistance >= CHARGE_DISTANCE)
		{
			ResetCharge();
		}
	}

	public override void ResetAttack()
	{
		base.ResetAttack();

	}

	public void ResetCharge()
	{
		_isChargingAbility = false;
		_isAbilityOnCooldown = true;
		_isCharging = false;
		_chargeMeter = 0.0f;
		_chargeDistance = 0.0f;
		CharacterStatComponent.ReduceStat("Speed", 50.0f, StatTypes.Stat);

		_mainTimer.Start();
	}
	#endregion

	#region EVENT HANDLING
	#endregion

	#region HELPERS
	public override void SetUpEnemy(Dictionary<string, float> overrideStats, out float attackRange)
	{
		base.SetUpEnemy(overrideStats, out attackRange);

		var chargeRangeCircle = (CircleShape2D) ChargeArea2D.GetNode<CollisionShape2D>("CollisionShape2D").Shape;
		chargeRangeCircle.ResourceLocalToScene = true;
		chargeRangeCircle.Radius = 300.0f;
	}
	#endregion

	#region CLEANUP
	#endregion


}
