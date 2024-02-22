using System;
using System.Collections.Generic;
using Godot;
using static AnimationCreator;

[Tool]
public partial class DamageNumberComponent : Node2D
{
	private LabelSettings defaultLabelSettings = Utils.CreateLabelSettings(Colors.WHITE, Colors.BLACK, 20);
	private LabelSettings critLabelSettings = Utils.CreateLabelSettings(Colors.RED, Colors.BLACK, 20);
	public List<AnimationPlayer> AnimationPlayers { get; private set; } = new List<AnimationPlayer>();
	public List<Label> Labels = new List<Label>();

	Animation test;

	public override void _Ready()
	{
		base._Ready();
		test = GD.Load<Animation>("res://assets/animations/text_start.res");
	}

	public void UpdateText(string text, DamageTypes damageType)
	{
		Label label = new Label();
		Labels.Add(label);

		Color fontColor = damageType switch
		{
			DamageTypes.Fire => Colors.RED,
			DamageTypes.Electric => Colors.YELLOW,
			DamageTypes.Normal => Colors.WHITE,
			_ => Colors.WHITE
		};

		// If normal damage then this color
		label.LabelSettings = defaultLabelSettings;
		label.LabelSettings.FontColor = fontColor;
		//TODO: If crits then change to critLabelSettings
		label.Text = text;

		AddChild(label);

		AnimationPlayer animPlayer = new AnimationPlayer();
		AnimationPlayers.Add(animPlayer);
		animPlayer.AnimationFinished += OnFinishedAnimation;
		// Animation animation = CreateAnimation(AnimationTypes.UI_DamageNumber);
		AnimationLibrary animLibrary = new AnimationLibrary();

		animLibrary.AddAnimation("text_start", test);
		animPlayer.AddAnimationLibrary("UI", animLibrary);

		AddChild(animPlayer);
		animPlayer.Play("UI/text_start");
	}

	private void OnFinishedAnimation(StringName animName)
	{
		GD.Print("Finished animation");
		Labels[0].QueueFree();
		Labels.RemoveAt(0);
		AnimationPlayers[0].QueueFree();
		AnimationPlayers.RemoveAt(0);
	}

}
