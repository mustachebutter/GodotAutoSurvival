using System;
using System.Collections.Generic;
using Godot;

public partial class Zap : Projectile
{
	private Area2D _area2D;
	private float _bounceRadius = 200.0f;
	private int _numberOfBounces = 3;
	private float _damageReduction = 0.5f;
	private List<Enemy> _bouncedEnemies = new List<Enemy>();

	public override void _Ready()
	{
		base._Ready();
		_area2D = GetNode<Area2D>("Area2D");

	}
	public override void HandleProjectileEffect()
	{
		base.HandleProjectileEffect();
		_collisionShape2D.SetDeferred("disabled", true);
		
		// Find closest enemy and bounce to it
		CircleShape2D circle = (CircleShape2D) _area2D.GetNode<CollisionShape2D>("CollisionShape2D").Shape;
		circle.Radius = _bounceRadius;

		Enemy closestEnemy = (Enemy) Utils.FindClosestTarget(Position, _area2D);
		GD.Print($"Name - {closestEnemy?.Name}");
		GD.Print($"Health - {closestEnemy?.Health}");
		GD.Print($"Damage - {Damage}\n\n");

		if (_numberOfBounces < 0)
			_shouldDestroyProjectile = true;

		if(closestEnemy != null && !_bouncedEnemies.Contains(closestEnemy))
		{
			// Reset some variables
			_distanceTravelled = 0.0f;

			_bouncedEnemies.Add(closestEnemy);
			ShootAtTarget(Position, closestEnemy.Position, _bounceRadius);
			_numberOfBounces--;
			// Deal reduced X% damage to Y enemies
			Damage *= (1 - _damageReduction);
		}

	}
}
