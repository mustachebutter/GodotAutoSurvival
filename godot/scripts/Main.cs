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
	private List<CharacterBody2D> dummies = new List<CharacterBody2D>();

	public override void _Ready()
	{
		_timer = GetNode<Timer>("Timer");
		for (int i = 2; i <= 5; i++)
		{
			var tempNode = GetNodeOrNull<CharacterBody2D>($"Enemy{i}");
			if(tempNode != null)
			{
				GD.Print(tempNode.Name);
			}
			else
			{
				GD.Print("Failed to retrieve node");
			}
			dummies.Add(tempNode);
			
		}

		_player = (Player) _playerScene.Instantiate();
		AddChild(_player);
		
		StartTimer(2);
	}

	public override void _Process(double delta)
	{
		
	}

	private void StartTimer(float seconds = 0.0f)
	{
		if (seconds > 0)
		{
			_projectile = (Projectile) _projectileScene.Instantiate();
			_projectile.AnimationName = "Fireball";
			AddChild(_projectile);
			_projectile.Position = _player.Position;
			_projectile.ShootAtTarget(_player.Position, dummies[0].Position);
			GD.Print($"MAINT - {_player.Position}");

			_timer.WaitTime = seconds;
			_timer.OneShot = true;
			_timer.Start();
			_timer.Connect("timeout", new Callable(this, "OnTimerTimeout"));
		}
	}

	private void OnTimerTimeout()
	{
		_projectile.QueueFree();
		StartTimer(2);
	}
}
