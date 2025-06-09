using System;
using System.Collections;
using Godot;
using System.Collections.Generic;

public partial class Tanker : Enemy
{
	public Area2D ChargeArea2D { get; set; }
	private bool _isAbilityOnCooldown { get; set; } = false;
	private bool _finishedChargingAbility { get; set; } = false;
	private float _chargeDistance { get; set; } = 0.0f;
	private Vector2 _previousFramePosition = Vector2.Zero;
	private Timer _chargeTimer { get; set; }
	private Timer _chargeCooldownTimer { get; set; }
	private readonly float CHARGE_DURATION = 2.0f;
	private readonly float CHARGE_COOLDOWN = 10.0f;
	private readonly float CHARGE_DISTANCE = 700.0f;


	#region GODOT
	public override void _Ready()
	{
		base._Ready();
		ChargeArea2D = GetNode<Area2D>("ChargeArea2D");
		AssignAnimationLibrary("Enemy_Tanker_AnimationLibrary", SavedAnimationLibrary.EnemyAnimationLibrary);

		var overrideStats = new Dictionary<string, float>
		{
			{ "AttackRange", 150.0f },
			{ "Speed", 40.0f },
			{ "Health", 200.0f },
		};

		SetUpEnemy(overrideStats, out float attackRange);

		_chargeTimer = Utils.CreateTimer
		(
			this,
			() =>
			{
				LoggingUtils.Debug("Ability Charged~");
				StatusEffectHUD.Visible = false;

				CharacterStatComponent.AddStat("Speed", 150.0f, StatTypes.Stat);
				_finishedChargingAbility = true;
			},
			CHARGE_DURATION,
			true
		);

		_chargeCooldownTimer = Utils.CreateTimer
		(
			this,
			() => _isAbilityOnCooldown = false,
			CHARGE_COOLDOWN,
			true
		);

		// AI Behavior set up
		_blackboard.SetValue("playerPosition", new Vector3());

		BTNode abilityChargeUp = new ActionNode((float delta) =>
		{
			_chargeTimer.Start();
			// Do the charging animation
			StatusEffectHUD.Visible = true;
			return true;
		});
		BTNode abilityDashTowardsPlayer = new ActionNode(Ability_Charge);
		BTNode abilityEndOfDash = new ActionNode((float delta) => ResetCharge());
		BTNode abilityCooldown = new ActionNode((float delta) =>
		{
			_chargeCooldownTimer.Start();
			return true;
		});


		BTNode detectPlayer = new SequenceNode(new List<BTNode>
		{
			abilityChargeUp,
			// Should finished charging then continue with the rest
			new ConditionalNode(() => _finishedChargingAbility),
			abilityDashTowardsPlayer,
			abilityEndOfDash,
			abilityCooldown,
		});

		BTNode castAbility = new ConditionalControllerNode(() =>
			{
				return DetectedPlayer(ChargeArea2D, out _) && !_isAbilityOnCooldown;
			},
			detectPlayer
		);

		_rootNodes.Insert(0, castAbility);
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

	public bool Ability_Charge(float delta)
	{
		if (MoveTowardsThePlayer())
		{
			LoggingUtils.Debug("Hit a player");
			// TODO: If hit players then knock them back
			return true;
		}

		_chargeDistance += _previousFramePosition.DistanceTo(Position);
		_previousFramePosition = Position;

		if (_chargeDistance >= CHARGE_DISTANCE)
		{
			return true;
		}

		return false;
	}

	public bool ResetCharge()
	{
		LoggingUtils.Debug("Resetting");
		_isAbilityOnCooldown = true;
		_finishedChargingAbility = false;
		_chargeDistance = 0.0f;
		CharacterStatComponent.ReduceStat("Speed", 50.0f, StatTypes.Stat);
		_mainTimer.Start();

		StopInPlace();

		return true;
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
