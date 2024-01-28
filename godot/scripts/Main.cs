using Godot;
using System;
using System.Collections.Generic;


public partial class Main : Node2D
{
	private Timer _timer;
	private PackedScene _projectileScene = (PackedScene) GD.Load("res://scenes/projectile.tscn");
	private Projectile _projectile;
	private PackedScene _playerScene = (PackedScene) GD.Load("res://scenes/player.tscn");
	private Player _player;
	private PackedScene _enemyScene = (PackedScene) GD.Load("res://scenes/enemy.tscn");
	private List<Enemy> dummies = new List<Enemy>();

	// TEMP
	private int number = 11;

	public override void _Ready()
	{
		// !!!!!! DEBUG ONLY
		GetTree().DebugCollisionsHint = true;
		// !!!!!! DEBUG ONLY
		
		_timer = GetNode<Timer>("Timer");
		for (int i = 0; i < number; i++)
		{
			Vector2 randomPosition = new Vector2((float) GD.RandRange(0.0f, 800.0f), (float) GD.RandRange(000.0f, 600.0f));
			var enemyNode = _enemyScene.Instantiate();
			
			AddChild(enemyNode);
			var enemy = (Enemy) enemyNode;
			enemy.Position = randomPosition;

			dummies.Add(enemy);
		}

		_player = (Player) _playerScene.Instantiate();
		AddChild(_player);
		
		StartTimer(1 / _player.AttackSpeed);
	}

	public override void _Process(double delta)
	{
		
	}

	private void StartTimer(float seconds = 0.0f)
	{
		GD.Print(IsInstanceValid(_projectile));
		if (seconds > 0)
		{	
			CreateProjectile();
			_timer.WaitTime = seconds;
			_timer.OneShot = false;
			_timer.Connect("timeout", new Callable(this, "OnTimerTimeout"));

			_timer.Start();

		}
	}

	private void OnTimerTimeout()
	{
		CreateProjectile();
	}

	private void CreateProjectile()
	{
		_projectile = (Projectile) _projectileScene.Instantiate();
		_projectile.OnEnemyKilledEvent += HandleEnemyDead;
		AddChild(_projectile);
		_player.FireProjectileAtTarget(_projectile);			
	}
	private void HandleEnemyDead(Enemy enemy)
	{
		if (dummies.Contains(enemy))
		{
			GD.Print(dummies.Remove(enemy));
		}
	}
}
