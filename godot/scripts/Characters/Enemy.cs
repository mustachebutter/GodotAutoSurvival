using Godot;
using System;
using System.Collections.Generic;

public partial class Enemy : BaseCharacter
{
	private Label _label;
	private Player _player;
	protected bool _isStopping = false;
	protected bool _isFacingRight { get; set; } = true;
	private bool _dealtDamage { get; set; } = false;
	protected Blackboard _blackboard { get; set; } = new Blackboard();
	protected BTNode _behaviorTree { get; set; }
	protected List<BTNode> _rootNodes { get; set; }
	protected Timer _mainTimer { get; set; }
	protected Dictionary<string, float> _overrideStats = new Dictionary<string, float>();


	public AnimatedSprite2D AnimatedSprite2D { get; set; }
	public AnimationPlayer AnimationPlayer { get; set; }
	public Area2D HitDetectionArea2D { get; set; }
	public RichTextLabel StatusEffectHUD { get; set; }

	#region GODOT
	public override void _Ready()
	{
		base._Ready();
		_label = GetNode<Label>("Label2");
		_label.Text = Name;
		_player = UtilGetter.GetMainPlayer();

		AnimatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		HitDetectionArea2D = GetNode<Area2D>("HitArea2D");

		AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		AnimationPlayer.AnimationFinished += OnAnimationFinished;

		StatusEffectHUD = GetNode<RichTextLabel>("StatusEffectHUD");
		StatusEffectHUD.Visible = false;

		SetUpEnemy();

		// Behavior Tree set up
		_blackboard.SetValue("bCanAttack", false);
		_blackboard.SetValue("bFinishedAttackAnimation", false);
		_blackboard.SetValue("bIsAttacking", false);

		_mainTimer = Utils.CreateTimer
		(
			this,
			() => _blackboard.SetValue("bCanAttack", true),
			CharacterStatComponent.GetStatFromName("AttackSpeed").Value,
			true
		);

		_mainTimer.Start();

		BTNode attackPlayer = new SequenceNode(new List<BTNode>
			{
				new ConditionalNode(() => DetectedPlayer(Area2D, out _) && _blackboard.GetValue<bool>("bCanAttack")),
				new ActionNode((float delta) =>
				{
					LoggingUtils.Debug("Setting up Attack");
					_blackboard.SetValue("bIsAttacking", true);
					StopInPlace();
					return BTNodeState.Success;
				}),
				CreatePlayAnimationNode(AnimationPlayer, "attack"),
				new ActionNode((float delta) =>
				{
					LoggingUtils.Debug("Attack");
					Attack();
					return BTNodeState.Success;
				}),
				new WaitUntilNode(() => _blackboard.GetValue<bool>("bFinishedAttackAnimation")),
				new ActionNode((float delta) => ResetAttack()),
			}
		);

		BTNode chasePlayer = new SequenceNode(new List<BTNode>
			{
				new ConditionalNode(() => GlobalConfigs.EnemySpawnMode.Equals(EnemySpawnMode.Normal) && !_isStopping),
				new ActionNode((float delta) =>
				{
					MoveTowardsThePlayer();

					LoggingUtils.Debug("IN CHASE");

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
			attackPlayer,
			chasePlayer,
			idle,
		};

		_behaviorTree = new SelectorNode(_rootNodes);

		// ANIMATIONS
		_animationBehaviorTree = new SelectorNode(new List<BTNode>
		{
			new SequenceNode(new List<BTNode> {
				new ConditionalNode(() => _blackboard.GetValue<bool>("bIsAttacking")),
				// Return success to stop the selector and prevents idle
				new ActionNode((f) => BTNodeState.Success),
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
			_isFacingRight = true;
		}
		else if (Velocity.X < 0)
		{
			AnimatedSprite2D.FlipH = true;
			_isFacingRight = false;
		}
	}
	public bool MoveTowardsThePlayer()
	{
		Vector2 direction = _player.GlobalPosition - GlobalPosition;
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
					LoggingUtils.Debug($"Hitt player {player.Name}");
					Velocity = Vector2.Zero;
					return true;
				}

			}
		}

		return false;
	}

	public void StopInPlace()
	{
		_isStopping = true;
		Velocity = Vector2.Zero;
	}

	public virtual void Attack()
	{
		// If it hits, do damage
		if (DetectedPlayer(HitDetectionArea2D, out Player player) && !_dealtDamage)
		{
			LoggingUtils.Debug("Hit Player");
			player.DealDamageToCharacter(CharacterStatComponent.GetCompleteStatFromName("Attack").totalValue, DamageTypes.Normal);
			_dealtDamage = true;
		}
	}

	public virtual BTNodeState ResetAttack()
	{
		LoggingUtils.Debug("ReSetting Attack");

		_dealtDamage = false;
		_blackboard.SetValue("bCanAttack", false);
		_blackboard.SetValue("bFinishedAttackAnimation", false);
		_blackboard.SetValue("bIsAttacking", false);

		_isStopping = false;

		_mainTimer.Start();

		return BTNodeState.Success;
	}

	#endregion

	#region EVENT HANDLING
	private void OnAnimationFinished(StringName anim_name)
	{
		LoggingUtils.Error(anim_name);
		if (anim_name == GetAnimation("attack"))
		{
			LoggingUtils.Error("Finished attacking animation");
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
				player = (Player) bd;
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

		_circle.Radius = CharacterStatComponent.GetCompleteStatFromName("AttackRange").totalValue / 2;
		LoggingUtils.Debug($"Range {_circle.Radius}");
	}

	public void AssignAnimationLibrary(string name, AnimationLibrary animationLibrary)
	{
		AnimationLibraryName = name;
		var animationList = AnimationPlayer.GetAnimationLibraryList();
		if (animationList.Count == 0)
		{
			AnimationPlayer.AddAnimationLibrary(name, animationLibrary);
		}
	}
	
	#endregion

	#region CLEANUP
	#endregion
}
