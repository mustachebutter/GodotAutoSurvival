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
		if (_shouldPhysicsProcess && _frameCount == 2)
		{
			// for (int i = 0; i < ShapeCast2D.GetCollisionCount(); i++)
			// {
			// 	var enemy = (Enemy) ShapeCast2D.GetCollider(i);
			// 	if (!enemies.Contains(enemy) && enemy != _ignoredEnemy)
			// 		GD.Print($"[{GetType().Name}] Enemy - {ShapeCast2D.GetCollider(i)}");
			// 		enemies.Add(enemy);
			// }

			var overlappedNodes = Area2D.GetOverlappingBodies();
			if (overlappedNodes != null)
			{
				foreach (var node in overlappedNodes)
				{
					var enemy = node as Enemy;
					if (enemy != _ignoredEnemy)
						GD.Print("Enemy: ", enemy);
						enemies.Add(enemy);		
				}
			}
				


			_frameCount++;
			GD.Print("Frame: ", _frameCount);
			if (_frameCount > _totalFramesToRun)
			{
				_shouldPhysicsProcess = false;
				ReportEnemies.Invoke(enemies);
			}
		}
	}

	public OnScannedEnemiesHandler ScanForEnemies(BaseCharacter ignoredCharacter)
	{
		_shouldPhysicsProcess = true;
		_frameCount = 0;
		_ignoredEnemy = (Enemy) ignoredCharacter;

		return ReportEnemies;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
	}
}
