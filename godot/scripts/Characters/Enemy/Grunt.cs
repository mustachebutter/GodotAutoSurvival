using Godot;
using Godot.Collections;

public partial class Grunt : Enemy
{
	public AnimatedSprite2D AnimatedSprite2D { get; set; }
	public AnimationPlayer AnimationPlayer { get; set; }
	public Area2D HitDetectionArea2D { get; set; }
	private bool _isWalking { get; set; } = false;
	private bool _isAttacking { get; set; } = false;
	private bool _isIdle { get; set; } = false;
	private bool _isFacingRight { get; set; } = true;
	private Timer _mainTimer { get; set; }
	private float _angle =  -(Mathf.Pi / 2);
	[Export]
	private float _orbitSpeed = 1.0f;
	[Export]
	private float _orbitRadius = 50.0f;



	public override void _Ready()
	{
		base._Ready();
		AnimatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		HitDetectionArea2D = GetNode<Area2D>("HitArea2D");

		AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		AnimationPlayer.AnimationFinished += OnAnimationFinished;

		CharacterStatComponent.GetStatFromName("AttackRange").Value = 150;
		var attackRange = _circle.Radius = CharacterStatComponent.GetCompleteStatFromName("AttackRange").totalValue / 2;
		
		HitDetectionArea2D.Position = new Vector2(0.0f, -(attackRange));
		_orbitRadius = attackRange;

		_mainTimer = Utils.CreateTimer(this, HandleAttackTimer, CharacterStatComponent.GetStatFromName("AttackSpeed").Value, false);
		_mainTimer.Start();
	}

	private void HandleAttackTimer()
	{

		if (DetectedPlayer(Area2D))
		{
			LoggingUtils.Debug("Found player");
			// Attempt to attack
			AnimationPlayer.Stop();
			_isIdle = false;
			_isWalking = false;
			_isAttacking = true;
			AnimationPlayer.Play(GetAnimation("attack"));
			StopInPlace();
			// Deal Damage

		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

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
			Attack();
		}
	}

	public bool DetectedPlayer(Area2D area2D)
	{
		var bodies = area2D.GetOverlappingBodies();
		if (bodies == null)
		{
			LoggingUtils.Info("No bodies scanned with Area2D");
			return false;
		}

		foreach (var bd in bodies)
		{
			if (bd is Player) return true;
		}

		return false;
	}
	
	public void Attack()
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
		// If it hits, do damage
		if (DetectedPlayer(HitDetectionArea2D))
		{
			LoggingUtils.Debug("Hit Player");
		}
	}

	public void ResetAttack()
	{
		HitDetectionArea2D.Position = new Vector2(0.0f, -(CharacterStatComponent.GetCompleteStatFromName("AttackRange").totalValue / 2));
		_angle =  -(Mathf.Pi / 2);
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
}

