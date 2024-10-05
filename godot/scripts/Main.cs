using Godot;
using System;
using System.Collections.Generic;


public partial class Main : Node2D
{
	private PackedScene _playerScene = Scenes.Player;
	private Player _player;

	public override void _Ready()
	{
		// !!!!!! DEBUG ONLY
		GetTree().DebugCollisionsHint = true;
		// !!!!!! DEBUG ONLY
		
		_player = (Player) _playerScene.Instantiate();
		GetNode<Node2D>("CharactersParentNode").AddChild(_player);		
	}

	public override void _Process(double delta)
	{
		
	}
	
	public void SpawnNode(Node2D node)
	{
		AddChild(node);
	}
}
