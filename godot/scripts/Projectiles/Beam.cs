using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Beam : Node2D
{
	protected Area2D _beamHitbox;
	protected float _beamWidth { get; set; } = 50.0f;
	protected float _beamLength { get; set; } = 500.0f;
	protected RectangleShape2D _beamHitboxShape;
	public override void _Ready()
	{
		_beamHitbox = GetNode<Area2D>("Area2D");
		_beamHitboxShape = (RectangleShape2D) GetNode<CollisionShape2D>("Area2D/CollisionShape2D").Shape;
		_beamHitboxShape.Size = new Vector2(_beamWidth, _beamLength);
	}

}
