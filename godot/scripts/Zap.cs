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

	}
	public override void HandleProjectileEffect(Enemy hitEnemy)
	{
		base.HandleProjectileEffect();
		// _bouncedEnemies.Add(hitEnemy);
		// _collisionShape2D.SetDeferred("disabled", true);
		
		// Find closest enemy and bounce to it
		CircleShape2D circle = (CircleShape2D) _area2D.GetNode<CollisionShape2D>("CollisionShape2D").Shape;
		circle.Radius = _bounceRadius;

		Enemy closestEnemy = null;
		if (_bouncedEnemies.Count == 0)
			closestEnemy = (Enemy) Utils.FindClosestTarget(Position, _area2D);
		else
			closestEnemy = (Enemy) Utils.FindClosestTarget(Position, _area2D, _bouncedEnemies?.Last());
		GD.Print($"Name - {closestEnemy?.Name}");
		GD.Print($"Health - {closestEnemy?.Health}");
		GD.Print($"Damage - {Damage}\n\n");

		if (_numberOfBounces < 0)
			_shouldDestroyProjectile = true;

		if(closestEnemy != null)
		{
			_bouncedEnemies.Add(closestEnemy);
			// !!!!DEBUG: Be extremely careful with this
			// It might not work on higher attack speed
			closestEnemy.SetCollisionLayerValue(5, true);
			closestEnemy.SetCollisionLayerValue(3, false);
			// !!!!DEBUG

			ShootAtTarget(Position, closestEnemy.Position, _bounceRadius);
			_numberOfBounces--;
			// Deal reduced X% damage to Y enemies
			Damage *= (1 - _damageReduction);
		}

	}
}
