using System;
using System.Collections.Generic;
using Godot;

[Tool]
public partial class DamageNumberComponent : Node2D
{
	private LabelSettings defaultLabelSettings = Utils.CreateLabelSettings(Colors.WHITE, Colors.BLACK, 20);
	private LabelSettings critLabelSettings = Utils.CreateLabelSettings(Colors.RED, Colors.BLACK, 20);
	public AnimationPlayer AnimationPlayer { get; private set; }
	public List<Label> labels = new List<Label>();

	public override void _Ready()
	{
		base._Ready();
		AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		AnimationPlayer.AnimationFinished += OnFinishedAnimation;
	}

	public void UpdateText(string text, DamageTypes damageType)
	{
		Label label = new Label();
		labels.Add(label);

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
		AnimationPlayer.Play("text_start");
	}

	private void OnFinishedAnimation(StringName animName)
	{
		GD.Print("Finished animation");
		labels[0].QueueFree();
		labels.RemoveAt(0);
	}

}
