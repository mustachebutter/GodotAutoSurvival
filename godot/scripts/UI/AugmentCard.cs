using Godot;
using System;

public partial class AugmentCard : MarginContainer
{
	public Button Button { get; set; }
	public ColorRect BackgroundColor { get; set; }
	public Label LevelText { get; set; }
	public TextureRect AugmentIcon { get; set; }
	public RichTextLabel AugmentDescription { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Button = GetNode<Button>("Button");
		Button.Pressed += () => { LoggingUtils.Info($"Test button #{Name}"); };
		BackgroundColor = GetNode<ColorRect>("ColorRect");
		LevelText = GetNode<Label>("VBoxContainer/MarginContainer2/Label");
		AugmentIcon = GetNode<TextureRect>("VBoxContainer/MarginContainer3/TextureRect");
		AugmentDescription = GetNode<RichTextLabel>("VBoxContainer/MarginContainer/RichTextLabel");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
