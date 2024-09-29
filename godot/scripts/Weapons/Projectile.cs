using System;
using System.Collections.Generic;
using Godot;

public partial class Projectile : Weapon
{
	protected AnimatedSprite2D _animatedSprite;
	protected CollisionShape2D _collisionShape2D;
	private Vector2 _projectileVelocity = new Vector2(0, 0);
	protected float _distanceTravelled = 0;
	private Vector2 _previousPosition = new Vector2(0, 0);
	protected float _playerRange = 0;
	protected bool _shouldDestroyProjectile = false;
	protected bool _shouldDamageEnemy = false;

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

			enemy.DealDamageToCharacter(CalculateTotalDamage());
			
			HandleProjectileEffect(SourceCharacter, enemy);
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

	public void ShootAtTarget(Vector2 sourcePosition, Vector2 targetPosition, float playerRange, BaseCharacter sourceCharacter)
	{
		// This is set so that the projectile can shoot from the player
		Position = sourcePosition;
		_previousPosition = Position;
		_playerRange = playerRange;
		SourceCharacter = sourceCharacter;

		// Determine the direction (look at rotation)
		Vector2 direction = (targetPosition - sourcePosition).Normalized();
		LookAt(GlobalPosition + direction);
		// LookAt(Vector2.Left);
		Velocity = direction * WeaponData.Speed;
		// Play the animation
		_animatedSprite.Play(WeaponData.AnimationName);

		// Reset some variables
		_distanceTravelled = 0.0f;
		_shouldDamageEnemy = true;
	}
	public void SpawnNodeInMain(Node2D node)
	{
		var mainNode = (Main) GetParent();
		mainNode.SpawnNode(node);
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		StatusEffect = null;
		SourceCharacter = null;
	}
}
