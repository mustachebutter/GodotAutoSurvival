using System;
using Godot;

public partial class Projectile : CharacterBody2D
{
	protected AnimatedSprite2D _animatedSprite;
	private string _animationName = "Fireball";

	private Vector2 _projectileVelocity = new Vector2(0, 0);
	private float _distanceTravelled = 0;
	private Vector2 _previousPosition = new Vector2(0, 0);
	private float _playerRange = 0;

	public delegate void OnEnemyKilledHandler(Enemy enemy);
	public event OnEnemyKilledHandler OnEnemyKilledEvent;
	
	[Export]
	public float Speed = 500.0f;
	[Export]
	public float Damage = 10.0f;
	[Export]
	public string AnimationName
	{
		get { return _animationName; }
		set { _animationName = value; }
	}

	public virtual void HandleProjectileEffect() { }

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("Sprite");
		// _previousPosition = Position;
		GD.PrintErr($"Projectile - {Position}");
	}

	public override void _PhysicsProcess(double delta)
	{
		KinematicCollision2D collision = MoveAndCollide(Velocity * (float) delta);
		if (collision != null)
		{
			Enemy enemy = (Enemy) collision.GetCollider();
			

			// This is where the projectile should deal damage or apply an effect

			if (enemy.DealDamageTo(Damage))
			{
				OnEnemyKilledEvent.Invoke(enemy);
				enemy.QueueFree();

			}

			HandleProjectileEffect();
			QueueFree();
		}

		_distanceTravelled += (Position - _previousPosition).Length();
		_previousPosition = Position;

		if (_distanceTravelled >= _playerRange)
		{
			QueueFree();
		}
	}

	public void ShootAtTarget(Vector2 sourcePosition, Vector2 targetPosition, float playerRange)
	{
		// This is set so that the projectile can shoot from the player
		Position = sourcePosition;
		_previousPosition = Position;
		_playerRange = playerRange;
		// Determine the direction (look at rotation)
		Vector2 direction = (targetPosition - sourcePosition).Normalized();
		LookAt(GlobalPosition + direction);
		
		Velocity = direction * Speed;
		// Play the animation
		_animatedSprite.Play(AnimationName);
	}
}
