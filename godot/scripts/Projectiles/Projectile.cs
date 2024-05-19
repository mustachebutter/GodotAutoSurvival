using System;
using System.Collections.Generic;
using Godot;

public partial class Projectile : CharacterBody2D
{
	protected AnimatedSprite2D _animatedSprite;
	protected CollisionShape2D _collisionShape2D;
	protected Area2D _bulletHeadArea2D;
	private Vector2 _projectileVelocity = new Vector2(0, 0);
	protected float _distanceTravelled = 0;
	private Vector2 _previousPosition = new Vector2(0, 0);
	protected float _playerRange = 0;
	protected bool _shouldDestroyProjectile = false;
	protected bool _shouldDamageEnemy = false;
	protected List<Enemy> _bouncedEnemies = new List<Enemy>();
	public StatusEffect StatusEffect { get; protected set; }

	public ProjectileData ProjectileData { get; set;}

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

		if (collision != null)
		{
			Enemy enemy = (Enemy) collision.GetCollider();

			enemy.DealDamageToCharacter(ProjectileData.Damage);
			enemy.OnCharacterDeadEvent += () => HandleTargetDead(enemy);
			
			HandleProjectileEffect(enemy);
			// When the projectile hits, destroy itself
			QueueFree();

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
		Velocity = direction * ProjectileData.Speed;
		// Play the animation
		_animatedSprite.Play(ProjectileData.AnimationName);

		// Reset some variables
		_distanceTravelled = 0.0f;
		_shouldDamageEnemy = true;
	}
	public void SpawnNodeInMain(Node2D node)
	{
		var mainNode = (Main) GetParent();
		mainNode.SpawnNode(node);
	}

	public void HandleTargetDead(Enemy enemy)
	{
		_shouldDestroyProjectile = true;
	}
}
