using Godot;
using System.Collections.Generic;

public partial class Enemy : BaseCharacter
{
	private Label _label;
	private Player _player;
	protected bool _isStopping = false;
	private bool _isWalking { get; set; } = false;
	private bool _isAttacking { get; set; } = false;
	private bool _isIdle { get; set; } = false;
	protected bool _isFacingRight { get; set; } = true;
	private bool _dealtDamage { get; set; } = false;
	protected Blackboard _blackboard { get; set; } = new Blackboard();
	protected BTNode _behaviorTree { get; set; }

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

		// Behavior Tree set up
		BTNode attack = new ActionNode((float delta) =>
		{
			return false;
		});

		BTNode chase = new ActionNode((float delta) =>
		{
			MoveTowardsThePlayer();
			if (DetectedPlayer(Area2D, out _))
			{
				return true;
			}

			return false;
		});

		BTNode chasePlayer = new SequenceConditionalNode(() =>
			{
				return GlobalConfigs.EnemySpawnMode.Equals(EnemySpawnMode.Normal) && !_isStopping;
			},
			new SequenceNode(new List<BTNode>
			{
				chase
			})
		);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (GlobalConfigs.EnemySpawnMode.Equals(EnemySpawnMode.Normal) && !_isStopping)
			MoveTowardsThePlayer();
		
		if (Velocity == Vector2.Zero && _isIdle == false && _isAttacking == false)
		{	
			_isIdle = true;
			_isWalking = false;
			_isAttacking = false;
			AnimationPlayer.Play(GetAnimation("idle"));
		}
		else if (Velocity != Vector2.Zero && _isWalking == false&& _isAttacking == false)
		{
			_isIdle = false;
			_isWalking = true;
			_isAttacking = false;
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

		if (_isAttacking)
		{
			LoggingUtils.Debug("Atacking");
			Attack();
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
			for (int i = 0; i < GetSlideCollisionCount() - 1; i++)
			{
				Node2D otherBody = GetSlideCollision(i).GetCollider() as Node2D;

				if (otherBody is Player) return true;
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

	public virtual void ResetAttack()
	{
		_dealtDamage = false;
	}

	#endregion

	#region EVENT HANDLING
	protected virtual void HandleAttackTimer()
	{
		if (DetectedPlayer(Area2D, out Player player))
		{
			// Attempt to attack
			AnimationPlayer.Stop();
			_isIdle = false;
			_isWalking = false;
			_isAttacking = true;
			AnimationPlayer.Play(GetAnimation("attack"));
			StopInPlace();
		}
	}

	private void OnAnimationFinished(StringName anim_name)
	{
		if (anim_name == GetAnimation("attack"))
		{
			_isAttacking = false;
			_isStopping = false;
			ResetAttack();
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

	public void StartAttackTimer()
	{
		_mainTimer = Utils.CreateTimer(this, HandleAttackTimer, CharacterStatComponent.GetStatFromName("AttackSpeed").Value, false);
		_mainTimer.Start();
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
