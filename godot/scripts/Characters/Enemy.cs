using Godot;
using System.Collections.Generic;

public partial class Enemy : BaseCharacter
{
	private Label _label;
	private Player _player;
	protected bool _isStopping = false;
	private bool _isWalking { get; set; } = false;
	private bool _isIdle { get; set; } = false;
	protected bool _isFacingRight { get; set; } = true;
	private bool _dealtDamage { get; set; } = false;
	protected Blackboard _blackboard { get; set; } = new Blackboard();
	protected BTNode _behaviorTree { get; set; }
	protected List<BTNode> _rootNodes { get; set; }

	protected Timer _mainTimer { get; set; }

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

		_mainTimer = Utils.CreateTimer
		(
			this,
			() =>  _blackboard.SetValue("bCanAttack", true),
			CharacterStatComponent.GetStatFromName("AttackSpeed").Value,
			true
		);

		// Behavior Tree set up
		_blackboard.SetValue("bCanAttack", false);
		_blackboard.SetValue("bFinishedAttackAnimation", false);


		BTNode chase = new ActionNode((float delta) =>
		{
			if (DetectedPlayer(Area2D, out _))
			{
				LoggingUtils.Error("Should be here");
				return false;
			}

			MoveTowardsThePlayer();

			return true;
		});

		BTNode chasePlayer = new ConditionalControllerNode(() =>
			{
				return GlobalConfigs.EnemySpawnMode.Equals(EnemySpawnMode.Normal) && !_isStopping;
			},
			chase
		);

		BTNode attackSetUp = new ActionNode((float delta) =>
		{
			LoggingUtils.Debug("Setting up Attack");
			AnimationPlayer.Stop();
			_isIdle = false;
			_isWalking = false;
			AnimationPlayer.Play(GetAnimation("attack"));
			LoggingUtils.Debug(AnimationPlayer.CurrentAnimation);
			StopInPlace();

			return true;
		});

		BTNode attack = new ActionNode((float delta) =>
		{
			LoggingUtils.Debug("Attack");
			Attack();
			return true;
		});

		BTNode attackReset = new ActionNode((float delta) => ResetAttack());

		BTNode attackPlayer = new ConditionalControllerNode(() =>
			{
				return DetectedPlayer(Area2D, out _) && _blackboard.GetValue<bool>("bCanAttack");
			},
			new SequenceNode(new List<BTNode>
			{
				attackSetUp,
				attack,
				new ConditionalNode(() => _blackboard.GetValue<bool>("bFinishedAttackAnimation")),
				attackReset,
			})
		);

		_rootNodes = new List<BTNode>
		{
			// chasePlayer,
			attackPlayer,
		};

		_behaviorTree = new SelectorNode(_rootNodes);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		var _canAttack = _blackboard.GetValue<bool>("bCanAttack");

		if (GlobalConfigs.EnemySpawnMode.Equals(EnemySpawnMode.Normal) && !_isStopping)
			MoveTowardsThePlayer();
		
		if (Velocity == Vector2.Zero && _isIdle == false && _canAttack == false)
		{	
			_isIdle = true;
			_isWalking = false;
			AnimationPlayer.Play(GetAnimation("idle"));
		}
		else if (Velocity != Vector2.Zero && _isWalking == false&& _canAttack == false)
		{
			_isIdle = false;
			_isWalking = true;
			AnimationPlayer.Play(GetAnimation("run"));
		}
		
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

	#endregion
	#region ACTION
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

	public virtual bool ResetAttack()
	{
		LoggingUtils.Debug("ReSetting Attack");

		_dealtDamage = false;
		_blackboard.SetValue("bCanAttack", false);
		_blackboard.SetValue("bFinishedAttackAnimation", false);
		_isStopping = false;

		_mainTimer.Start();

		return true;
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
			AnimationPlayer.Play(GetAnimation("idle"));
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

	public virtual void SetUpEnemy(Dictionary<string, float> overrideStats, out float attackRange)
	{
		if (overrideStats.Count > 0)
		{
			foreach (var statKey in overrideStats.Keys)
			{
				var stat = CharacterStatComponent.GetStatFromName(statKey);
				stat.Value = overrideStats[statKey];
			}
		}

		attackRange = _circle.Radius = CharacterStatComponent.GetCompleteStatFromName("AttackRange").totalValue / 2;
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
