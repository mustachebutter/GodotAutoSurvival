using System;
using System.Collections.Generic;
using Godot;

public static class Utils
{
	public static Font font = GD.Load<Font>("res://assets/fonts/VCR_OSD_MONO_1.001.ttf");
	public static Node2D FindClosestTarget(Vector2 sourcePosition, Area2D area2D)
	{
		var enemyNodes = area2D.GetOverlappingBodies();
		GD.PrintRich($"[color=yellow]{enemyNodes.Count}[/color]");
		if (enemyNodes == null) return null;


		Node2D closestNode = null;
		float closestDistance = 0.0f;
		foreach (var e in enemyNodes)
		{
			float distance = sourcePosition.DistanceTo(e.Position);
			if (closestNode == null || distance < closestDistance)
			{
				closestDistance = distance;
				closestNode = e;
				GD.Print($"Should be in here - {closestNode.Name}\n");
			}
		}

		GD.Print($"Should be the closest - {closestNode?.Name}\n");
		return closestNode;
	}

	public static Node2D FindClosestTarget(Vector2 sourcePosition, Area2D area2D, Enemy enemyToIgnore)
	{
		var enemyNodes = area2D.GetOverlappingBodies();
		if (enemyNodes == null) return null;
 
		Node2D closestNode = null;
		float closestDistance = 0.0f;
		foreach (var e in enemyNodes)
		{
			float distance = sourcePosition.DistanceTo(e.Position);
			GD.Print($"Hit - {e.Name} - {distance} - {enemyToIgnore.Name} - {closestNode == null || distance < closestDistance}");
			if ((Enemy) e != enemyToIgnore)
			{
				if (closestNode == null || distance < closestDistance)
				{
					GD.Print("In here");
					closestDistance = distance;
					closestNode = e;
				}
			}
		} 

		return closestNode;
	}
	
	public static Timer CreateTimer(Node2D parentNode, Action function, float seconds = 0.0f, bool isOneShot = true)
	{
		if (seconds <= 0) return null;

		Timer timer = new Timer();
		parentNode?.AddChild(timer);
		timer.WaitTime = seconds;
		timer.OneShot = isOneShot;
		timer.Connect("timeout", Callable.From(function));

		return timer;
	}

	public static void DestroyTimer(Timer timer)
	{
		timer.Stop();
		// timer.QueueFree();
	}

	public static LabelSettings CreateLabelSettings(Color color, Color outlineColor, int fontSize = 10, int outlineSize = 3)
	{		
		LabelSettings labelSettings = new LabelSettings();
		labelSettings.Font = font;

		labelSettings.FontColor = color;
		labelSettings.FontSize = fontSize;

		labelSettings.OutlineColor = outlineColor;
		labelSettings.OutlineSize = outlineSize;

		return labelSettings;
	}

	// !!!!!! DEBUG ONLY
	public static Enemy CreateDummy(Vector2 position, PackedScene enemyScene, Main main)
	{
		var enemyNode = enemyScene.Instantiate();

		main.AddChild(enemyNode);
		var enemy = (Enemy) enemyNode;
		enemy.Position = position;

		return enemy;
	}
	// !!!!!! DEBUG ONLY
}
