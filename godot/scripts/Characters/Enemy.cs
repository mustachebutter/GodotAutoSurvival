using Godot;
using System;
using System.Collections.Generic;

public partial class Enemy : BaseCharacter
{
	private Label _label;
	private Player _player { get; set; }
	private Vector2 _randomPointToWanderTo { get; set; } = Vector2.Zero;
	protected float _orbitSpeed = 5.0f;
	protected float _orbitRadius;
	protected float _angle = -(Mathf.Pi / 2);
	protected Vector2 _hitDetectionAreaOffset = Vector2.Zero;
	protected Blackboard _blackboard { get; set; } = new Blackboard();
	protected BTNode _behaviorTree { get; set; }
	protected List<BTNode> _rootNodes { get; set; }
	protected Timer _mainTimer { get; set; }
	protected Dictionary<string, float> _overrideStats = new Dictionary<string, float>();
	public AnimatedSprite2D AnimatedSprite2D { get; set; }
	public Area2D HitDetectionArea2D { get; set; }
	protected Shape2D Shape2D { get; set; }
	public RichTextLabel StatusEffectHUD { get; set; }

	#region GODOT
	public override void _Ready()
	{
		base._Ready();
		_player = UtilGetter.GetMainPlayer();
		var mainNode = (Main) UtilGetter.GetMotherNode();
		_randomPointToWanderTo = mainNode.GetRandomOutOfViewportPosition();

		_label = GetNode<Label>("Label2");
		_label.Text = Name;

		AnimatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		HitDetectionArea2D = GetNode<Area2D>("HitArea2D");

		StatusEffectHUD = GetNode<RichTextLabel>("StatusEffectHUD");
		StatusEffectHUD.Visible = false;

		SetUpEnemy();

		// Behavior Tree set up
		_blackboard.SetValue("bDealtDamage", false);
		_blackboard.SetValue("bCanAttack", false);
		_blackboard.SetValue("bFinishedAttackAnimation", false);
		_blackboard.SetValue("bIsAttacking", false);
		_blackboard.SetValue("bIsCharging", false);
		_blackboard.SetValue("bIsStopping", false);


		_mainTimer = Utils.CreateTimer
		(
			this,
			() => _blackboard.SetValue("bCanAttack", true),
			CharacterStatComponent.GetStatFromName("AttackSpeed").Value,
			true
		);

		_mainTimer.Start();

		BTNode die = new SequenceNode(new List<BTNode>
		{
			new ConditionalNode(() => IsDead),
			new ActionNode((_) => BTNodeState.Success)
		});

		BTNode attackPlayer = new SequenceNode(new List<BTNode>
			{
				new ConditionalNode(() => DetectedPlayer(Area2D, out _) && _blackboard.GetValue<bool>("bCanAttack")),
				new ActionNode((float delta) =>
				{
					_blackboard.SetValue("bIsAttacking", true);
					StopInPlace();
					return BTNodeState.Success;
				}),
				new WaitNTick(2, Name),
				CreateDebugNode(
					CreatePlayAnimationNode(AnimationPlayer, "attack"),
					"In Animation_attack"
				),
				new ActionNode((float delta) => Attack(delta)),
				new WaitUntilNode(() => _blackboard.GetValue<bool>("bFinishedAttackAnimation")),
				new ActionNode((float delta) => ResetAttack()),
			}
		);

		BTNode chasePlayer = new SequenceNode(new List<BTNode>
			{
				new ConditionalNode(() => GlobalConfigs.EnemySpawnMode.Equals(EnemySpawnMode.Normal) && !_blackboard.GetValue<bool>("bIsStopping")),
				new ActionNode((float delta) =>
				{
					MoveTowardsThePlayer();
					return BTNodeState.Success;
				}),
			}
		);

		BTNode idle = new ActionNode((float delta) =>
		{
			Velocity = Vector2.Zero;
			return BTNodeState.Success;
		});

		_rootNodes = new List<BTNode>
		{
			die,
			attackPlayer,
			chasePlayer,
			idle,
		};

		_behaviorTree = new SelectorNode(_rootNodes);

		// ANIMATIONS
		_animationBehaviorTree = new SelectorNode(new List<BTNode>
		{
			new SequenceNode(new List<BTNode> {
				new ConditionalNode(() => IsDead),
				CreatePlayAnimationNode(AnimationPlayer, "die"),
			}),
			new SequenceNode(new List<BTNode> {
				new ConditionalNode(() => _blackboard.GetValue<bool>("bIsAttacking")),
				// Return success to stop the selector and prevents idle
				new ActionNode((_) => BTNodeState.Success),
			}),
			new SequenceNode(new List<BTNode> {
				new ConditionalNode(() =>  _blackboard.GetValue<bool>("bIsCharging")),
				new ActionNode((_) => BTNodeState.Success),
			}),
			new SequenceNode(new List<BTNode>
			{
				new ConditionalNode(() => Velocity.LengthSquared() > 0),
				new ActionNode((float delta) => {
					SetSpriteFlipDirection();
					return BTNodeState.Success;
				}),
				new SelectorNode(new List<BTNode> {
					new SequenceNode(new List<BTNode> {
						new ConditionalNode(() => CharacterStatComponent.GetCompleteStatFromName("Speed").totalValue > 150),
						CreatePlayAnimationNode(AnimationPlayer, "run"),
					}),
					CreatePlayAnimationNode(AnimationPlayer, "walk"),
				})
			}),
			CreatePlayAnimationNode(AnimationPlayer, "idle"),
		});
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		_behaviorTree.Execute((float)delta);
		_animationBehaviorTree.Execute((float)delta);

	}

	#endregion
	#region ACTION
	public void SetSpriteFlipDirection()
	{
		if (Velocity.X > 0)
		{
			AnimatedSprite2D.FlipH = false;
		}
		else if (Velocity.X < 0)
		{
			AnimatedSprite2D.FlipH = true;
		}
	}
	public void SetHitDetectionAreaPosition()
	{
		_hitDetectionAreaOffset.X *= AnimatedSprite2D.FlipH ? 1 : -1;
		LoggingUtils.Debug($"{AnimatedSprite2D.FlipH} && {_hitDetectionAreaOffset.X}");
		HitDetectionArea2D.Position = new Vector2(
			0.0f + _hitDetectionAreaOffset.X,
			-(CharacterStatComponent.GetCompleteStatFromName("AttackRange").totalValue / 2));
	}
	
	public bool MoveTowardsThePlayer()
	{
		Vector2 direction;

		if (_player.IsDead)
		{
			direction = _randomPointToWanderTo - GlobalPosition;
		}
		else
		{
			direction = _player.GlobalPosition - GlobalPosition;
		}

		direction = direction.Normalized();
		var speed = CharacterStatComponent.GetStatFromName("Speed").Value;

		Velocity = direction * speed;
		MoveAndSlide();

		if (GetSlideCollisionCount() > 0)
		{
			for (int i = 0; i < GetSlideCollisionCount(); i++)
			{
				Node2D otherBody = GetSlideCollision(i).GetCollider() as Node2D;

				if (otherBody is Player player)
				{
					Velocity = Vector2.Zero;
					return true;
				}

			}
		}

		return false;
	}

	public void StopInPlace()
	{
		_blackboard.SetValue("bIsStopping", true);
		Velocity = Vector2.Zero;
	}

	public virtual BTNodeState Attack(float delta)
	{
		// Swing and do hit detection
		// If facing right
		if (!AnimatedSprite2D.FlipH)
		{
			_angle += _orbitSpeed * delta;

			if (_angle > (Mathf.Pi / 2))
			{
				_angle = -(Mathf.Pi / 2);
				return BTNodeState.Success;
			}
		}
		else
		{
			_angle -= _orbitSpeed * delta;
			if (_angle < -(3 * Mathf.Pi / 2))
			{
				_angle = -(Mathf.Pi / 2);
				return BTNodeState.Success;
			}
		}

		// Reset the position everytime it tries to attack to reflect where it is facing
		// For enemies who have offset
		SetHitDetectionAreaPosition();

		float x = GlobalPosition.X + _orbitRadius * Mathf.Cos(_angle);
		float y = GlobalPosition.Y + _orbitRadius * Mathf.Sin(_angle);
		HitDetectionArea2D.GlobalPosition = new Vector2(x, y);

		// If it hits, do damage
		if (DetectedPlayer(HitDetectionArea2D, out Player player) && !_blackboard.GetValue<bool>("bDealtDamage"))
		{
			player.DealDamageToCharacter(CharacterStatComponent.GetCompleteStatFromName("Attack").totalValue, DamageTypes.Normal);
			_blackboard.SetValue("bDealtDamage", true);
		}

		return BTNodeState.Running;
	}

	public virtual BTNodeState ResetAttack()
	{
		_blackboard.SetValue("bDealtDamage", false);
		_blackboard.SetValue("bCanAttack", false);
		_blackboard.SetValue("bFinishedAttackAnimation", false);
		_blackboard.SetValue("bIsAttacking", false);
		_blackboard.SetValue("bIsStopping", false);

		_mainTimer.Start();

		SetHitDetectionAreaPosition();
		_angle = -(Mathf.Pi / 2);
		return BTNodeState.Success ;
	}

	#endregion

	#region EVENT HANDLING
	protected override void OnAnimationFinished(StringName anim_name)
	{
		base.OnAnimationFinished(anim_name);
		if (anim_name == GetAnimation("attack"))
		{
			_blackboard.SetValue("bFinishedAttackAnimation", true);
		}
	}

	#endregion

	#region HELPERS
	public bool DetectedPlayer(Area2D area2D, out Player player)
	{
		var bodies = area2D.GetOverlappingBodies();
		player = null;
		if (bodies == null)
		{
			LoggingUtils.Info("No bodies scanned with Area2D");
			return false;
		}

		foreach (var bd in bodies)
		{
			if (bd is Player)
			{
				player = (Player)bd;
				return true;
			}
		}

		return false;
	}

	public virtual void SetUpEnemy()
	{
		if (_overrideStats.Count > 0)
		{
			foreach (var statKey in _overrideStats.Keys)
			{
				var stat = CharacterStatComponent.GetStatFromName(statKey);
				stat.Value = _overrideStats[statKey];
			}
		}

		//NOTE: This is done specifically for Enemies because call base._Ready()
		// at the top and BaseCharacter are trying to set attackCircleRadius before
		// stats are ready by then.
		_attackRangeCircle.Radius = CharacterStatComponent.GetCompleteStatFromName("AttackRange").totalValue / 2;
		LoggingUtils.Debug($"{Name} range {_attackRangeCircle.Radius}");

		SetHitDetectionAreaPosition();
	}
	#endregion

	#region CLEANUP
	public override void Perish()
	{
		base.Perish();

		Random random = new Random();

		var orb = (ExperienceOrb)Scenes.ExperienceOrb.Instantiate();
		orb.Position = Position + new Vector2(random.Next(1, 20), random.Next(1, 20));
		orb.Scale = new Vector2(0.1f, 0.1f);
		UtilGetter.GetMotherNode().AddChild(orb);

		QueueFree();
	}

	#endregion
}
