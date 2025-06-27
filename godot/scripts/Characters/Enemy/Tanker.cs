using System;
using System.Collections;
using Godot;
using System.Collections.Generic;

public partial class Tanker : Enemy
{
	public Area2D ChargeArea2D { get; set; }
	private bool _isAbilityOnCooldown { get; set; } = false;
	private float _chargeDistance { get; set; } = 0.0f;
	private Vector2 _previousFramePosition = Vector2.Zero;
	private float _chargeTimer { get; set; }
	private Timer _chargeCooldownTimer { get; set; }
	private readonly float CHARGE_DURATION = 2.0f;
	private readonly float CHARGE_COOLDOWN = 10.0f;
	private readonly float CHARGE_DISTANCE = 700.0f;
	private readonly Dictionary<string, float> OVERRIDE_STATS = new Dictionary<string, float>
	{
		{ "AttackRange", 150.0f },
		{ "Speed", 40.0f },
		{ "Health", 200.0f },
	};
	
	public Tanker()
	{
		_overrideStats = OVERRIDE_STATS;
	}
	#region GODOT
	public override void _Ready()
	{
		ChargeArea2D = GetNode<Area2D>("ChargeArea2D");

		base._Ready();

		AssignAnimationLibrary("Enemy_Tanker_AnimationLibrary", SavedAnimationLibrary.EnemyAnimationLibrary);

		_chargeCooldownTimer = Utils.CreateTimer
		(
			this,
			() => _isAbilityOnCooldown = false,
			CHARGE_COOLDOWN,
			true
		);

		_chargeCooldownTimer.Start();

		// AI Behavior set up

		BTNode castAbility = new SequenceNode(new List<BTNode>
		{
			new ConditionalNode(() => DetectedPlayer(ChargeArea2D, out _) && !_isAbilityOnCooldown),
			new ActionNode((_) => {
				_blackboard.SetValue("bIsCharging", true);
				return BTNodeState.Success;
			}),
			CreatePlayAnimationNode(AnimationPlayer, "charge"),
			new ActionNode((float delta) =>
			{
				_chargeTimer += delta;
				// Do the charging animation
				if (!StatusEffectHUD.Visible)
					StatusEffectHUD.Visible = true;

				if (_chargeTimer >= CHARGE_DURATION)
				{
					LoggingUtils.Debug("Ability Charged~");
					StatusEffectHUD.Visible = false;
					CharacterStatComponent.AddStat("Speed", 150.0f, StatTypes.Stat);
					return BTNodeState.Success;
				}

				if (IsDead)
					return BTNodeState.Failure;

				return BTNodeState.Running;
			}),
			new ActionNode((_) => {
				_blackboard.SetValue("bIsCharging", false);
				return BTNodeState.Success;
			}),
			new ActionNode(Ability_Charge),
			new ActionNode((float delta) => ResetCharge()),
			new ActionNode((float delta) =>
			{
				_isAbilityOnCooldown = true;
				_chargeCooldownTimer.Start();
				return BTNodeState.Success;
			}),
		});

		_rootNodes.Insert(0, castAbility);
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
		// base.Attack();
	}

	public BTNodeState Ability_Charge(float delta)
	{
		if (MoveTowardsThePlayer())
		{
			LoggingUtils.Debug("Hit a player");
			// TODO: If hit players then knock them back
			return BTNodeState.Success;
		}

		_chargeDistance += _previousFramePosition.DistanceTo(Position);
		_previousFramePosition = Position;

		if (_chargeDistance >= CHARGE_DISTANCE)
		{
			return BTNodeState.Success;
		}

		return BTNodeState.Running;
	}

	public BTNodeState ResetCharge()
	{
		LoggingUtils.Debug("Resetting");
		_chargeDistance = 0.0f;
		_chargeTimer = 0.0f;
		CharacterStatComponent.ReduceStat("Speed", 150.0f, StatTypes.Stat);
		StopInPlace();
		_isStopping = false;

		return BTNodeState.Success;
	}
	#endregion

	#region EVENT HANDLING
	#endregion

	#region HELPERS
	public override void SetUpEnemy()
	{
		base.SetUpEnemy();
		
		var chargeRangeCircle = (CircleShape2D) ChargeArea2D.GetNode<CollisionShape2D>("CollisionShape2D").Shape;
		chargeRangeCircle.ResourceLocalToScene = true;
		chargeRangeCircle.Radius = 300.0f;
	}
	#endregion

	#region CLEANUP
	#endregion


}
