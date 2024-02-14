using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Zap : Projectile
{
	private Area2D _area2D;
	private float _bounceRadius = 100.0f;
	private int _numberOfBounces = 3;
	private float _damageReduction = 0.9f;

	public override void _Ready()
	{
		base._Ready();
		_area2D = GetNode<Area2D>("Area2D");
		// Find closest enemy and bounce to it
		CircleShape2D circle = (CircleShape2D) _area2D.GetNode<CollisionShape2D>("CollisionShape2D").Shape;
		circle.Radius = _bounceRadius;
	}
	public override void HandleProjectileEffect(Enemy hitEnemy)
	{
		base.HandleProjectileEffect();
		// _bouncedEnemies.Add(hitEnemy);
		// _collisionShape2D.SetDeferred("disabled", true);
		
		Enemy closestEnemy = null;
		// if (_bouncedEnemies.Count == 1)
		// 	closestEnemy = (Enemy) Utils.FindClosestTarget(Position, _area2D);
		// else
		closestEnemy = (Enemy) Utils.FindClosestTarget(Position, _area2D, _bouncedEnemies?.Last());

		if (_numberOfBounces <= 0)
			_shouldDestroyProjectile = true;

		if(closestEnemy != null)
		{
			// _bouncedEnemies.Add(closestEnemy);
			GD.Print($"Bounce!!! - {closestEnemy.Name} - {_numberOfBounces}");
			GD.Print($"Health - {closestEnemy?.Health}");
			GD.Print($"Damage - {Damage}\n\n");

			// !!!!DEBUG: Be extremely careful with this
			// It might not work on higher attack speed
			// closestEnemy.SetCollisionLayerValue(5, true);
			// closestEnemy.SetCollisionLayerValue(3, false);
			// !!!!DEBUG

			ShootAtTarget(Position, closestEnemy.Position, _playerRange);
			_numberOfBounces--;
			GD.Print($"Bounce - {_numberOfBounces}");

			// Deal reduced X% damage to Y enemies
			Damage *= (1 - _damageReduction);
		}

	}
}
