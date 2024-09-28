using Godot;
using System;
using System.Collections.Generic;

public partial class Player : BaseCharacter
{
	public WeaponComponent WeaponComponent;
	public override void _Ready()
	{
		base._Ready();
		WeaponComponent = GetNode<WeaponComponent>("WeaponComponent");
		WeaponComponent.StartTimer(1 / CharacterStatComponent.CharacterStatData.AttackSpeed.Value);
		var MainHUD = UtilGetter.GetSceneTree().Root.GetNode<MainHUD>("Node2D/MainHUD");
		MainHUD.SetDebugStats(CharacterStatComponent);

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

		var animationTree = GetNode<AnimationTree>("AnimationTree");
		var stateMachine = (AnimationNodeStateMachinePlayback) animationTree.Get("parameters/playback");

		if (velocity == Vector2.Zero)
		{	
			// stateMachine.Travel("Idle");
			animationTree.Set("parameters/conditions/idle", true);
			animationTree.Set("parameters/conditions/isWalking", false);
		}
		else
		{
			animationTree.Set("parameters/conditions/idle", false);
			animationTree.Set("parameters/conditions/isWalking", true);

			animationTree.Set("parameters/Idle/blend_position", velocity);
			animationTree.Set("parameters/Walk/blend_position", velocity);
		}
		

		// Normalized the Vector
		velocity = velocity.Normalized() * CharacterStatComponent.CharacterStatData.Speed.Value;

		Velocity = velocity;
		MoveAndSlide();
	}

	public void FireProjectileAtTarget(Node2D closestTarget, Weapon weapon)
	{
		if (weapon is Projectile projectile)
		{
			projectile.ShootAtTarget(Position, closestTarget.Position, CharacterStatComponent.CharacterStatData.AttackRange.Value, this);
		}
		else if (weapon is Beam beam)
		{
			beam.PrimeBeamAtTarget(closestTarget);
		}
	}
}
