using System;
using System.Collections;
using Godot;
using System.Collections.Generic;

public partial class Tanker : Enemy
{
	public Area2D ChargeArea2D { get; set; }
	private bool _isChargingAbility { get; set; } = false;
	private bool _isAbilityOnCooldown { get; set; } = false;
	private bool _isAbilityInProgress { get; set; } = false;
	private bool _isAbilityStarting { get; set; } = false;
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
		_chargeTimer = Utils.CreateTimer(this, () =>
		{
			_isAbilityInProgress = true;
			LoggingUtils.Debug("Ability Charged~");
		}, CHARGE_DURATION, true);

		_chargeCooldownTimer = Utils.CreateTimer(this, () => _isAbilityOnCooldown = false, CHARGE_COOLDOWN, true);

		// AI Behavior set up
		BTNode abilityChargeUp = new ActionNode((float delta) =>
		{
			return false;
		});
		BTNode abilityDashTowardsPlayer = new ActionNode((float delta) =>
		{
			return false;
		});
		BTNode abilityEndOfDash = new ActionNode((float delta) =>
		{
			return false;
		});
		BTNode abilityCooldown = new ActionNode((float delta) =>
		{
			return false;
		});


		BTNode detectPlayer = new SequenceNode(new List<BTNode>
		{
			abilityChargeUp,
			abilityDashTowardsPlayer,
			abilityEndOfDash,
			abilityCooldown,
		});

		// TODO: These should be in the Enemy.cs
		BTNode attackPlayer = new ActionNode((float delta) =>
		{
			return false;
		});

		BTNode chasePlayer = new ActionNode((float delta) =>
		{
			return false;
		});

		_behaviorTree = new SelectorNode(new List<BTNode>
		{
			chasePlayer,
			detectPlayer,
			attackPlayer,
		});
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		_behaviorTree.Execute((float) delta);
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
