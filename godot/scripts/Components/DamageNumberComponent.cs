using System;
using System.Collections.Generic;
using Godot;
using static AnimationCreator;

[Tool]
public partial class DamageNumberComponent : Node2D
{
	private float _maxTextOffset = 50.0f;

	private LabelSettings defaultLabelSettings = Utils.CreateLabelSettings(Colors.WHITE, Colors.BLACK, 20);
	private LabelSettings critLabelSettings = Utils.CreateLabelSettings(Colors.RED, Colors.BLACK, 20);
	public List<AnimationPlayer> AnimationPlayers { get; private set; } = new List<AnimationPlayer>();
	Label _label;
	AnimationPlayer _animationPlayer;
	AnimatedLabel animatedLabel = new AnimatedLabel();

	public void UpdateText(string text, DamageTypes damageType)
	{
		// The reason that I have to this in here is because this is a "Tool" script and it won't call Ready unless you reload the scene
		// And since the scene is being compiled first in Constants/Entities to quickly access. This is being done here.
		// s: https://github.com/godotengine/godot/issues/16974
		_label = GetNode<Label>("Label");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		Color fontColor = damageType switch
		{
			DamageTypes.Fire => Colors.RED,
			DamageTypes.Electric => Colors.YELLOW,
			DamageTypes.Normal => Colors.WHITE,
			_ => Colors.WHITE
		};

		// If normal damage then this color
		_label.LabelSettings = defaultLabelSettings;
		_label.LabelSettings.FontColor = fontColor;
		//TODO: If crits then change to critLabelSettings
		_label.Text = text;

		_animationPlayer.AnimationFinished += OnFinishedAnimation;
		_animationPlayer.Play("UI_AnimationLibrary/text_start");
	}

	public void OffsetText(Vector2 offset)
	{
		offset.X = (offset.X > _maxTextOffset) ? 0.0f : offset.X;
		offset.Y = (offset.Y > _maxTextOffset) ? 0.0f : offset.Y;
		Translate(offset);
	}

	private void OnFinishedAnimation(StringName animName)
	{
		QueueFree();
	}

}
