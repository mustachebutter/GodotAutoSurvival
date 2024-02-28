using Godot;

[Tool]
public partial class VisualEffectComponent : AnimatedSprite2D
{
	public override void _Ready()
	{
		base._Ready();
	}

	public void PlayVisualEffect(string animationName)
	{
		Animation = animationName;
		Play();
	}

	public void ClearVisualEffect()
	{
		Stop();
		Animation = "default";
	}

}
