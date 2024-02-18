using System;
using System.Collections.Generic;
using Godot;

[Tool]
public partial class DamageNumberComponent : Node2D
{
	// public Label Label { get; private set; }
	public AnimationPlayer AnimationPlayer { get; private set; }
	public List<Label> labels = new List<Label>();

	public override void _Ready()
	{
		base._Ready();
		AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		AnimationPlayer.AnimationFinished += OnFinishedAnimation;
	}

	private void OnFinishedAnimation(StringName animName)
	{
		GD.Print("Finished animation");
		labels[0].QueueFree();
		labels.RemoveAt(0);
	}

	public void UpdateText(string text)
	{
		labels.Add(new Label());
		labels[0].Text = text;

		AddChild(labels[0]);
		AnimationPlayer.Play("text_start");
	}
}
