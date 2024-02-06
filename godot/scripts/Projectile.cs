using System;
using System.Collections.Generic;
using Godot;

public partial class Projectile : CharacterBody2D
{
	protected AnimatedSprite2D _animatedSprite;
	protected CollisionShape2D _collisionShape2D;
	protected Area2D _bulletHeadArea2D;
	private string _animationName = "Fireball";
	private Vector2 _projectileVelocity = new Vector2(0, 0);
	protected float _distanceTravelled = 0;
	private Vector2 _previousPosition = new Vector2(0, 0);
	protected float _playerRange = 0;
	protected bool _shouldDestroyProjectile = false;
	protected bool _shouldDamageEnemy = false;
	protected List<Enemy> _bouncedEnemies = new List<Enemy>();

	public delegate void OnEnemyKilledHandler(Enemy enemy);
	public event OnEnemyKilledHandler OnEnemyKilledEvent;
	
	[Export]
	public float Speed = 200.0f;
	[Export]
	public float Damage = 1.0f;
	[Export]
	public string AnimationName
	{
		get { return _animationName; }
		set { _animationName = value; }
	}

	public virtual void HandleProjectileEffect() { }
	public virtual void HandleProjectileEffect(Enemy hitEnemy) { }

	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite2D>("Sprite");
		_collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
	}

	public override void _Process(double delta)
	{
		KinematicCollision2D collision = MoveAndCollide(Velocity * (float) delta);
		// Position += Velocity * (float) delta;

		// var hitBodies = _bulletHeadArea2D.GetOverlappingBodies();
		
		// if (hitBodies != null && hitBodies.Count > 0)
		if (collision != null && _shouldDamageEnemy)
		{
			Enemy enemy = (Enemy) collision.GetCollider();
			GD.PrintRich($"[color=black]{enemy.Name} - {collision != null && _shouldDamageEnemy}[/color]");
			// Enemy enemy = (Enemy) hitBodies[0];
			if(!_bouncedEnemies.Contains(enemy))
			{
				_bouncedEnemies.Add(enemy);
				// !!!!DEBUG: Be extremely careful with this
				// It might not work on higher attack speed
				enemy.SetCollisionLayerValue(5, true);
				enemy.SetCollisionLayerValue(3, false);

				// !!!!DEBUG

				// This is where the projectile should deal damage or apply an effect
				if (enemy.DealDamageTo(Damage))
				{
					OnEnemyKilledEvent.Invoke(enemy);
					enemy.QueueFree();
					_shouldDestroyProjectile = true;
				}

				_shouldDamageEnemy = false;
				HandleProjectileEffect(enemy);
			}
		}

		_distanceTravelled += (Position - _previousPosition).Length();
		_previousPosition = Position;

		if (_distanceTravelled >= _playerRange)
			_shouldDestroyProjectile = true;

		if (_shouldDestroyProjectile)
			QueueFree();
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		foreach(var e in _bouncedEnemies)
		{
			if (IsInstanceValid(e))
			{
				e.SetCollisionLayerValue(5, false);
				e.SetCollisionLayerValue(3, true);
			}
		}
		_bouncedEnemies.Clear();
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
		// LookAt(Vector2.Left);
		Velocity = direction * Speed;
		// Play the animation
		_animatedSprite.Play(AnimationName);

		// Reset some variables
		_distanceTravelled = 0.0f;
		_shouldDamageEnemy = true;
	}
}
