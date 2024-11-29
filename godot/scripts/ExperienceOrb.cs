using System;
using Godot;

public partial class ExperienceOrb : CharacterBody2D
{
	public int ExperienceValue { get; set; } = 500;
	public bool ShouldMoveTowardsPlayer { get; set; } = false;
	public CollisionShape2D CollisionShape2D { get; set; }
	private float _timeElapsed = 0.0f;

	public override void _Ready()
	{
		base._Ready();
		CollisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		
		if (ShouldMoveTowardsPlayer)
		{
			_timeElapsed += (float) delta;

			Vector2 playerPosition = UtilGetter.GetMainPlayer().Position;
			Vector2 direction = Position.DirectionTo(playerPosition);
			
			int speedToMove = _timeElapsed > 5 ? 200 : 150;
			
			KinematicCollision2D collision = MoveAndCollide(direction * speedToMove * (float) delta);

			if (collision != null)
			{
				Player player = (Player) collision.GetCollider();
				player.CharacterLevelComponent.GainExperience(ExperienceValue);
				QueueFree();
			}
		}


	}



}
