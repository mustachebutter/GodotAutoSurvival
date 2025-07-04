using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Beam : Weapon
{
	protected Area2D _beamHitbox;
	protected float _beamWidth { get; set; } = 50.0f;
	protected float _beamLength { get; set; } = 500.0f;
	protected RectangleShape2D _beamHitboxShape;
	// protected Sprite2D _beamTexture;
	protected ColorRect _beamTexture;
	protected AnimationPlayer _beamAnimationPlayer;
	public override void _Ready()
	{
		_beamHitbox = GetNode<Area2D>("Area2D");
		_beamTexture = GetNode<ColorRect>("Area2D/CollisionShape2D/ColorRect");
		_beamHitboxShape = (RectangleShape2D) GetNode<CollisionShape2D>("Area2D/CollisionShape2D").Shape;
		_beamAnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		_beamHitboxShape.Size = new Vector2(_beamWidth, _beamLength);
		_beamAnimationPlayer.AnimationFinished += OnFinishedAnimation;

	}

	public void OnFinishedAnimation(StringName animName)
	{
		switch(animName)
		{
			case "VFX_AnimationLibrary/prime":
				QueueFree();
				break;
			default:
				break;
		}
	}

	public void PrimeBeamAtTarget(Node2D closestTarget, BaseCharacter sourceCharacter)
	{
		if (_beamAnimationPlayer == null)
			LoggingUtils.Error($"[{nameof(Beam)}] Beam animation player is null!");
		else
		{
			SourceCharacter = sourceCharacter;
			var angle = GlobalPosition.AngleToPoint(closestTarget.GlobalPosition);
			GlobalRotation = angle - (float) Math.PI / 2;
			_beamAnimationPlayer.Play(WeaponData.AnimationName);
		}
	}

	public virtual void DealDamageToCharacter()
	{
		// Scan with Area2D
		var bodies = _beamHitbox.GetOverlappingBodies();
		if (bodies != null)
		{
			foreach (var bd in bodies)
			{
				var enemy = (Enemy) bd;
				enemy.DealDamageToCharacter(CalculateTotalDamage());

				if (enemy.IsDead)
				{
					OnTargetDied(enemy);
				}
			}
		}
	}

	// NOTE: The reason this is only being done here in Beam is because
	// Beam kill enemies based on the weapon's damage itself. The other weapons
	// kill based on status effects and we don't want the Weapon's
	// OnTargetDied to take precedence over status effects'
	public override void OnTargetDied(BaseCharacter target)
	{
		base.OnTargetDied(target);

		target.Perish();
	}
}
