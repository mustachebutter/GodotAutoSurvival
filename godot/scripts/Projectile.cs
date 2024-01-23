using Godot;

public partial class Projectile : CharacterBody2D
{
	protected AnimatedSprite2D _animatedSprite;
	private string _animationName = "Fireball";

	private Vector2 _projectileVelocity = new Vector2(0, 0);
	
	[Export]
	public float Speed = 500.0f;
	[Export]
	public string AnimationName
	{
		get { return _animationName; }
		set { _animationName = value; }
	}

	public override void _Ready()
	{
		GD.Print(AnimationName);
		_animatedSprite = GetNode<AnimatedSprite2D>("Sprite");
	}

	public override void _PhysicsProcess(double delta)
	{
		KinematicCollision2D collision = MoveAndCollide(Velocity * (float) delta);
		if (collision != null)
		{
		}
	}

	public void ShootAtTarget(Vector2 sourcePosition, Vector2 targetPosition)
	{
		// Determine the direction (look at rotation)
		Vector2 direction = (targetPosition - sourcePosition).Normalized();
		LookAt(GlobalPosition + direction);
		
		Velocity = direction * Speed;
		// Play the animation
		_animatedSprite.Play(AnimationName);
	}
}
