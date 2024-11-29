using System;
using System.Collections.Generic;
using Godot;

public partial class VfxChainLightning : Node2D
{
	public Area2D Area2D;
	public CollisionShape2D CollisionShape2D;
	public ShapeCast2D ShapeCast2D;
	public AnimatedSprite2D AnimatedSprite2D;
	public Line2D Line2D;
	private bool _shouldPhysicsProcess = false;
	private int _frameCount = 0;
	private int _totalFramesToRun = 10;
	public List<Enemy> enemies = new List<Enemy>();

	public delegate void OnScannedEnemiesHandler(List<Enemy> enemies);
	public event OnScannedEnemiesHandler ReportEnemies;
	
	public override void _Ready()
	{
		base._Ready();
		AnimatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		Area2D = GetNode<Area2D>("Area2D");
		CollisionShape2D = Area2D.GetNode<CollisionShape2D>("CollisionShape2D");
		CircleShape2D circleShape = (CircleShape2D) CollisionShape2D.Shape;
		circleShape.Radius = 100.0f;
		Line2D = GetNode<Line2D>("Line2D");

	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		if (_shouldPhysicsProcess)
		{
			enemies = Utils.FindTargetsOrderedByDistance(Position, Area2D);
			
			if (enemies == null)
			{
				QueueFree();
			}
			// We want to make sure that it runs for at least 10 frames
			// Give or take this might not needed but just to make sure everything works.
			_frameCount++;
			if (_frameCount > _totalFramesToRun)
			{
				_shouldPhysicsProcess = false;
				ReportEnemies.Invoke(enemies);
			}
		}
	}

	public void ScanForEnemies()
	{
		_shouldPhysicsProcess = true;
		_frameCount = 0;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
	}
}
