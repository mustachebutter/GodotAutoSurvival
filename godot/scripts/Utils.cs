using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class Utils
{
	public static Font font = GD.Load<Font>("res://assets/fonts/VCR_OSD_MONO_1.001.ttf");
	public static Node2D FindClosestTarget(Vector2 sourcePosition, Area2D area2D)
	{
		var enemyNodes = area2D.GetOverlappingBodies();
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
			}
		}

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
			if ((Enemy) e != enemyToIgnore)
			{
				if (closestNode == null || distance < closestDistance)
				{
					closestDistance = distance;
					closestNode = e;
				}
			}
		} 

		return closestNode;
	}

	public static List<Enemy> FindTargetsOrderedByDistance(Vector2 sourcePosition, Area2D area2D)
	{
		var overlappedNodes = area2D.GetOverlappingBodies();
		if (overlappedNodes == null) return null;
		var enemyNodes = overlappedNodes.Cast<Enemy>().ToList();

		return enemyNodes.OrderBy(x => sourcePosition.DistanceTo(x.Position)).ToList();
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
		try
		{
			ArgumentNullException.ThrowIfNull(timer);
		}
		catch
		{
			LoggingUtils.Error($"[{nameof(Utils)}]: Tried to destroy a null timer.");
			throw;
		}

		timer?.Stop();
		timer?.QueueFree();
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
	public static Enemy CreateDummy(Vector2 position, PackedScene enemyScene)
	{
		var enemyNode = enemyScene.Instantiate();

		var parent = UtilGetter.GetSceneTree().Root.GetNode<Node2D>("MotherNode/CharactersParentNode");
		parent.AddChild(enemyNode);
		var enemy = (Enemy) enemyNode;
		enemy.Position = position;

		return enemy;
	}
	// !!!!!! DEBUG ONLY
}
