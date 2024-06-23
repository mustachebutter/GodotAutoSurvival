using System;
using System.Collections.Generic;
using Godot;

public partial class VfxBurnExplosion : Node2D
{
	public Area2D Area2D;
	public CollisionShape2D CollisionShape2D;
	public ShapeCast2D ShapeCast2D;
	public AnimatedSprite2D AnimatedSprite2D;
	private bool _shouldPhysicsProcess = false;
	private int _frameCount = 0;
	private int _totalFramesToRun = 10;
	public List<Enemy> enemies = new List<Enemy>();
	public Enemy _ignoredEnemy;

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
		// ShapeCast2D = GetNode<ShapeCast2D>("ShapeCast2D");

	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		if (_shouldPhysicsProcess)
		{
			var overlappedNodes = Area2D.GetOverlappingBodies();
			if (overlappedNodes != null)
			{
				foreach (var node in overlappedNodes)
				{
					var enemy = node as Enemy;
					if (enemy != _ignoredEnemy && !enemies.Contains(enemy))
						enemies.Add(enemy);		
				}
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

	public void ScanForEnemies(BaseCharacter ignoredCharacter)
	{
		_shouldPhysicsProcess = true;
		_frameCount = 0;
		_ignoredEnemy = (Enemy) ignoredCharacter;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
	}
}
