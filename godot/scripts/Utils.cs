using System.Collections.Generic;
using Godot;

public static class Utils
{
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
			GD.Print($"Hit - {e.Name}");
			if ((Enemy) e != enemyToIgnore)
			{
				float distance = sourcePosition.DistanceTo(e.Position);
				if (closestNode == null || distance < closestDistance)
				{
					closestDistance = distance;
					closestNode = e;
				}
			}
		} 

        return closestNode;
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