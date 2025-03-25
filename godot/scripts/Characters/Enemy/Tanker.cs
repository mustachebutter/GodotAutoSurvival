using System.Collections;
using Godot;
using Godot.Collections;

public partial class Tanker : Enemy
{
	public Area2D ChargeArea2D { get; set; }
	private bool _isChargingAbility { get; set; } = false;
	private bool _isAbilityOnCooldown { get; set; } = false;
	private bool _isAbilityInProgress { get; set; } = false;
	private float _chargeDistance { get; set; } = 0.0f;
	private Vector2 _previousFramePosition = Vector2.Zero;
	private Timer _chargeTimer { get; set; }
	private Timer _chargeCooldownTimer { get; set; }
	private readonly float CHARGE_DURATION = 1.0f;
	private readonly float CHARGE_COOLDOWN = 10.0f;
	private readonly float CHARGE_DISTANCE = 500.0f;
	
	#region GODOT
	public override void _Ready()
	{
		base._Ready();
		ChargeArea2D = GetNode<Area2D>("ChargeArea2D");
		AssignAnimationLibrary("Enemy_Tanker_AnimationLibrary", SavedAnimationLibrary.EnemyAnimationLibrary);
		LoggingUtils.Debug(GetAnimation("attack"));

		var overrideStats = new Dictionary<string, float>
		{ 
			{ "AttackRange", 150.0f },
			{ "Speed", 40.0f },
			{ "Health", 200.0f },
		};

		SetUpEnemy(overrideStats, out float attackRange);

		StartAttackTimer();
		_chargeTimer = Utils.CreateTimer(this, () => Ability_Charge(), CHARGE_DURATION, true);
		_chargeCooldownTimer = Utils.CreateTimer(this, () => _isAbilityOnCooldown = false, CHARGE_COOLDOWN, true);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (_isAbilityInProgress) return;
		
		if (DetectedPlayer(ChargeArea2D, out Player player)) 
			_isAbilityInProgress = true;
		else return;

		if (!_isAbilityOnCooldown)
		{
			_chargeTimer.Start();
		}
		else
		{
			_chargeCooldownTimer.Start();
		}
	}
	#endregion
	
	#region ACTION
	public override void Attack()
	{
		// Swing and do hit detection
		// base.Attack();
	}

	public IEnumerator Ability_Charge()
	{
		// Pause the attack timer
		if (!_mainTimer.IsStopped())
			_mainTimer.Stop();

		LoggingUtils.Debug("CHAAAARGE");
		// Play animation of charging up
		// Dash towards target
		CharacterStatComponent.AddStat("Speed", 50.0f, StatTypes.Stat);

		if (MoveTowardsThePlayer())
		{
			LoggingUtils.Debug("Hit a player");
			// TODO: If hit players then knock them back
			ResetCharge();
			yield break;
		}

		_chargeDistance += _previousFramePosition.DistanceTo(Position);
		_previousFramePosition = Position;

		if (_chargeDistance >= CHARGE_DISTANCE)
		{
			ResetCharge();
			yield break;
		}

		yield return null;
	}

	public override void ResetAttack()
	{
		base.ResetAttack();

	}

	public void ResetCharge()
	{
		_isChargingAbility = false;
		_isAbilityOnCooldown = true;
		_isAbilityInProgress = false;
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
