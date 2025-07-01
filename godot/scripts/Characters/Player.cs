using Godot;
using System;
using System.Collections.Generic;

public partial class Player : BaseCharacter
{
	public WeaponComponent WeaponComponent { get; private set; }
	public CharacterLevelComponent CharacterLevelComponent { get; private set; }
	public AnimationTree AnimationTree { get; set; }

	public override void _Ready()
	{
		base._Ready();
		WeaponComponent = GetNode<WeaponComponent>("WeaponComponent");
		CharacterLevelComponent = GetNode<CharacterLevelComponent>("CharacterLevelComponent");
		AnimationTree = GetNode<AnimationTree>("AnimationTree");

		// This use of AttackRange is used for determining the Area2D that detects the closest enemy
		// Essentially, what this means is that it would detect targets further away as AttackRange increases
		CharacterStatComponent.OnAnyStatUpgraded += HandleStatUpgraded;
		// _circle.Radius = CharacterStatComponent.GetCompleteStatFromName("AttackRange").totalValue / 2;
		WeaponComponent.StartTimer(1 / CharacterStatComponent.GetCompleteStatFromName("AttackSpeed").totalValue);

	}

	private void HandleStatUpgraded(UpgradableObject @object, float baseValue, float modifierValue, float totalValue)
	{
		switch (@object.Name)
		{
			case "AttackRange":
				_circle.Radius = CharacterStatComponent.GetCompleteStatFromName(@object.Name).totalValue / 2;
				break;
			case "AttackSpeed":
				WeaponComponent.OverrideTimer(1 / CharacterStatComponent.GetCompleteStatFromName(@object.Name).totalValue);
				break;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = new Vector2();
		if (Input.IsActionPressed("Up"))
		{
			velocity.Y -= 1;
		}

		if (Input.IsActionPressed("Down"))
		{
			velocity.Y += 1;
		}

		if (Input.IsActionPressed("Left"))
		{
			velocity.X -= 1;
		}

		if (Input.IsActionPressed("Right"))
		{
			velocity.X += 1;
		}

		if (Input.IsActionJustPressed("SwitchWeapon"))
		{
			WeaponComponent.SwitchNextWeapon();	
		}


		if (velocity == Vector2.Zero)
		{	
			AnimationTree.Set("parameters/conditions/idle", true);
			AnimationTree.Set("parameters/conditions/isWalking", false);
		}
		else
		{
			AnimationTree.Set("parameters/conditions/idle", false);
			AnimationTree.Set("parameters/conditions/isWalking", true);

			AnimationTree.Set("parameters/Idle/blend_position", velocity);
			AnimationTree.Set("parameters/Walk/blend_position", velocity);
		}
		

		// Normalized the Vector
		velocity = velocity.Normalized() * CharacterStatComponent.GetCompleteStatFromName("Speed").totalValue;

		Velocity = velocity;
		MoveAndSlide();
	}

	public void FireProjectileAtTarget(Node2D closestTarget, Weapon weapon)
	{
		if (weapon is Projectile projectile)
		{
			// This use of AttackRange is used for determining the travel distance of projectile
			// What this means in simple term is that it would travel further if the AttackRange is greater
			projectile.ShootAtTarget(Position, closestTarget.Position, CharacterStatComponent.GetCompleteStatFromName("AttackRange").totalValue, this);
		}
		else if (weapon is Beam beam)
		{
			beam.PrimeBeamAtTarget(closestTarget, this);
		}
	}
}
