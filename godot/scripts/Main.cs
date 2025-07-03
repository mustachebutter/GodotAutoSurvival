using System;
using Godot;


public partial class Main : Node2D
{
	private PackedScene _playerScene = Scenes.Player;
	private Player _player;
	private SubViewport _subViewport;

	public override void _Ready()
	{
		// !!!!!! DEBUG ONLY
		GetTree().DebugCollisionsHint = true;
		// !!!!!! DEBUG ONLY

		_player = (Player)_playerScene.Instantiate();
		GetNode<Node2D>("CharactersParentNode").AddChild(_player);
		GetRandomOutOfViewportPosition();
		_subViewport = GetParent<SubViewport>();
	}

	public override void _Process(double delta)
	{

	}

	public void SpawnNode(Node2D node)
	{
		AddChild(node);
	}

	public Vector2 GetRandomOutOfViewportPosition()
	{
		Viewport viewport = GetViewport();
		Vector2 viewportSize = viewport.GetVisibleRect().Size;
		Vector2 cameraPosition = viewport.GetCamera2D().GlobalPosition;

		// Apparently if you think of the viewport as a rectangle
		// and the camera position is in the center of the rectangle
		// Subtracting half the viewport will get you the top left
		// Adding half the viewport will get you the bottom right.
		Vector2 topLeftCorner = cameraPosition - viewportSize / 2;
		Vector2 bottomRightCorner = cameraPosition + viewportSize / 2;
		// LoggingUtils.Info($"{topLeftCorner}");
		// LoggingUtils.Info($"{bottomRightCorner}");

		Random random = new Random();

		// Formula for random floating point number
		// float randomNumber = (float)(random.NextDouble() * (maxValue - minValue)) + minValue;
		Vector2 randomTopBorderPosition = new Vector2(
			(float)(random.NextDouble() * ((topLeftCorner.X + viewportSize.X) - topLeftCorner.X) + topLeftCorner.X),
			topLeftCorner.Y - random.Next(1, 200) // Increase this amount to make mob spawn totally outside the screen
		);

		Vector2 randomLeftBorderPosition = new Vector2(
			topLeftCorner.X - random.Next(1, 200),
			(float)(random.NextDouble() * (topLeftCorner.Y + viewportSize.Y - topLeftCorner.Y) + topLeftCorner.Y)
		);

		Vector2 randomBottomBorderPosition = new Vector2(
			(float)(random.NextDouble() * (bottomRightCorner.X - topLeftCorner.X) + topLeftCorner.X),
			bottomRightCorner.Y + random.Next(1, 200)
		);

		Vector2 randomRightBorderPosition = new Vector2(
			bottomRightCorner.X + random.Next(1, 200),
			(float)(random.NextDouble() * (bottomRightCorner.Y - topLeftCorner.Y) + topLeftCorner.Y)
		);

		// Choose a random border to return!
		int randomDirection = random.Next(1, 5);

		Vector2 randomDirectionToReturn = randomDirection switch
		{
			1 => randomTopBorderPosition,
			2 => randomBottomBorderPosition,
			3 => randomLeftBorderPosition,
			4 => randomRightBorderPosition,
			_ => Vector2.Zero
		};

		if (randomDirectionToReturn == Vector2.Zero)
			LoggingUtils.Error("Failed to get a random position to spawn");

		return randomDirectionToReturn;
	}

	public void CullEnemyRender()
	{
		_subViewport.SetCanvasCullMaskBit(1, false);
	}
}
