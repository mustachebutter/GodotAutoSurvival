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

				break;
			default:
				break;
		}
	}

	public void HandlePrimedBeam()
	{
		if (_beamAnimationPlayer == null)
			GD.Print("Null");
		else
			_beamAnimationPlayer.Play(WeaponData.AnimationName);
	}

}
