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
}