using Godot;
using System;
using System.Collections.Generic;


public partial class Main : Node2D
{
	private PackedScene _playerScene = Scenes.Player;
	private Player _player;
	private PackedScene _enemyScene = Scenes.Enemy;
	private List<Enemy> dummies = new List<Enemy>();

	public override void _Ready()
	{
		// !!!!!! DEBUG ONLY
		GetTree().DebugCollisionsHint = true;
		// !!!!!! DEBUG ONLY
				
		dummies.Add(Utils.CreateDummy(new Vector2(371, 329), _enemyScene, this));
		dummies.Add(Utils.CreateDummy(new Vector2(435, 264), _enemyScene, this));
		dummies.Add(Utils.CreateDummy(new Vector2(455, 396), _enemyScene, this));
		dummies.Add(Utils.CreateDummy(new Vector2(504, 312), _enemyScene, this));

		foreach (var d in dummies)
		{
			d.OnCharacterDeadEvent += () => HandleEnemyDead(d);
		}

		_player = (Player) _playerScene.Instantiate();
		AddChild(_player);		
	}

	public override void _Process(double delta)
	{
		
	}
	
	private void HandleEnemyDead(Enemy enemy)
	{
		if (dummies.Contains(enemy))
		{
			GD.Print($"Removed enemy: {dummies.Remove(enemy)}, {enemy.Name}");
		}
	}

	public void SpawnNode(Node2D node)
	{
		AddChild(node);
	}
}
