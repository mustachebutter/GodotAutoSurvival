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
		CircleShape2D circle = (CircleShape2D) _area2D.GetNode<CollisionShape2D>("CollisionShape2D").Shape;
		circle.Radius = _bounceRadius;

		StatusEffect = new Static(this, StatusEffectParsedData.GetData("Status_Zap"));
	}
	public override void HandleProjectileEffect(Enemy enemy)
	{
		base.HandleProjectileEffect();
		enemy.StatusEffectComponent.ApplyEffectToCharacter(StatusEffect);
	}

}
