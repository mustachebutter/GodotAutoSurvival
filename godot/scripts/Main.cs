using Godot;
using System;
using System.Collections.Generic;


public partial class Main : Node2D
{
	private Timer _timer;
	private PackedScene _projectileScene = (PackedScene) GD.Load(Scenes.ProjectileZap);
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
		
		// for (int i = 0; i < number; i++)
		// {
		// 	Vector2 randomPosition = new Vector2((float) GD.RandRange(0.0f, 800.0f), (float) GD.RandRange(000.0f, 600.0f));
		// 	dummies.Add(Utils.CreateDummy(randomPosition, _enemyScene, this));
		// }
		
		dummies.Add(Utils.CreateDummy(new Vector2(371, 329), _enemyScene, this));
		dummies.Add(Utils.CreateDummy(new Vector2(435, 264), _enemyScene, this));
		dummies.Add(Utils.CreateDummy(new Vector2(455, 396), _enemyScene, this));
		dummies.Add(Utils.CreateDummy(new Vector2(504, 312), _enemyScene, this));

		_player = (Player) _playerScene.Instantiate();
		AddChild(_player);
		
		StartTimer(1 / _player.AttackSpeed);
	}

	public override void _Process(double delta)
	{
		
	}

	private void StartTimer(float seconds = 0.0f)
	{
		if (seconds > 0)
		{	
			CreateProjectile();
			_timer = Utils.CreateTimer(this, OnTimerTimeout, seconds, false);
			_timer?.Start();

		}
	}

	private void OnTimerTimeout()
	{
		CreateProjectile();
	}

	private void CreateProjectile()
	{
		Node2D closestTarget = Utils.FindClosestTarget(Position, _player.Area2D);
		GD.PrintRich("[color=orange]HA![/color]");

		if (closestTarget == null) return;

		_projectile = (Zap) _projectileScene.Instantiate();
		_projectile.OnEnemyKilledEvent += HandleEnemyDead;
		AddChild(_projectile);
		_player.FireProjectileAtTarget(closestTarget, _projectile, ProjectileTypes.Zap);
	}
	private void HandleEnemyDead(Enemy enemy)
	{
		if (dummies.Contains(enemy))
		{
			GD.Print($"Removed enemy: {dummies.Remove(enemy)}, {enemy.Name}");
		}
	}
}
