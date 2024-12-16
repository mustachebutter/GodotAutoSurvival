using Godot;
using Godot.Collections;

public partial class Grunt : Enemy
{
	public AnimatedSprite2D AnimatedSprite2D { get; set; }
	public AnimationPlayer AnimationPlayer { get; set; }
	[Export]
	private bool _isWalking { get; set; } = false;
	[Export]
	private bool _isAttacking { get; set; } = false;
	[Export]
	private bool _isIdle { get; set; } = false;
	private Timer _mainTimer { get; set; }

	public override void _Ready()
	{
		base._Ready();
		AnimatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		CharacterStatComponent.GetStatFromName("AttackRange").Value = 150;
		_circle.Radius = CharacterStatComponent.GetCompleteStatFromName("AttackRange").totalValue / 2;
		LoggingUtils.Debug($"{CharacterStatComponent.GetStatFromName("AttackSpeed").Value}");
		_mainTimer = Utils.CreateTimer(this, HandleAttackTimer, CharacterStatComponent.GetStatFromName("AttackSpeed").Value, false);
		AnimationPlayer.AnimationFinished += OnAnimationFinished;
		_mainTimer.Start();
	}

	private void HandleAttackTimer()
	{
		var bodies = DetectPlayer();

		if (bodies != null)
		{
			foreach (var bd in bodies)
			{
				if (bd is Player)
				{
					LoggingUtils.Debug("Found player");
					// Attempt to attack
					// AnimationTree.Set("parameters/conditions/isAttacking", true);
					// AnimationTree.Set("parameters/conditions/isIdle", false);
					// AnimationTree.Set("parameters/conditions/isWalking", false);
					AnimationPlayer.Stop();
					_isIdle = false;
					_isWalking = false;
					_isAttacking = true;
					AnimationPlayer.Play("Enemy_AnimationLibrary/attack");
					StopInPlace();
					// Deal Damage

				}
			}
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (Velocity == Vector2.Zero && _isIdle == false && _isAttacking == false)
		{	
			// AnimationTree.Set("parameters/conditions/isIdle", true);
			// AnimationTree.Set("parameters/conditions/isWalking", false);
			_isIdle = true;
			_isWalking = false;
			_isAttacking = false;
			AnimationPlayer.Play("Enemy_AnimationLibrary/idle");
		}
		else if (Velocity != Vector2.Zero && _isWalking == false&& _isAttacking == false)
		{
			// AnimationTree.Set("parameters/conditions/isIdle", false);
			// AnimationTree.Set("parameters/conditions/isWalking", true);
			_isIdle = false;
			_isWalking = true;
			_isAttacking = false;
			AnimationPlayer.Play("Enemy_AnimationLibrary/run");
		}

		if (Velocity.X > 0)
		{
			AnimatedSprite2D.FlipH = false;
		}
		else if (Velocity.X < 0)
		{
			AnimatedSprite2D.FlipH = true;
		}
	}

	public Array<Node2D> DetectPlayer()
	{
		var bodies = Area2D.GetOverlappingBodies();
		if (bodies == null)
		{
			LoggingUtils.Info("No bodies scanned with Area2D");
			return null;
		}

		return bodies;
	}

	private void OnAnimationFinished(StringName anim_name)
	{
		// Replace with function body.
		LoggingUtils.Debug($"AnimName : {anim_name}");

		if (anim_name == "Enemy_AnimationLibrary/attack")
		{
			// AnimationTree.Set("parameters/conditions/isAttackingFinished", true);
			// AnimationTree.Set("parameters/conditions/isAttacking", false);
			_isAttacking = false;
			_isStopping = false;
		}

	}
}

