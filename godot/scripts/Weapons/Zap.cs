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
		StatusEffect = new Static(DataParser.GetStatusEffectData("Status_Zap"));
	}
	public override void HandleProjectileEffect(BaseCharacter source, Enemy enemy)
	{
		base.HandleProjectileEffect(source, enemy);
		enemy.StatusEffectComponent.ApplyEffectToCharacter(StatusEffect, SourceCharacter, enemy);
	}

}
