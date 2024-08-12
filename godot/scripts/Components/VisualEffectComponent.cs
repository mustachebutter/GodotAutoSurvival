using Godot;

[Tool]
public partial class VisualEffectComponent : AnimatedSprite2D
{
	public const string DEFAULT_ANIMATION_NAME = "vfx_default";
	public override void _Ready()
	{
		base._Ready();
		Animation = DEFAULT_ANIMATION_NAME;
	}

	public void PlayVisualEffect(string animationName, bool isLooping)
	{
		Animation = animationName;
		SpriteFrames.SetAnimationLoop(Animation, isLooping);
		Play();
	}

	public void ClearVisualEffect()
	{
		if (Animation != DEFAULT_ANIMATION_NAME)
		{
			Stop();
			Animation = DEFAULT_ANIMATION_NAME;
		}
	}

}
