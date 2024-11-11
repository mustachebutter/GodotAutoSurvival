using Godot;
using System;

public partial class AugmentCard : MarginContainer
{
	public ColorRect BackgroundColor { get; set; }
	public Label LevelText { get; set; }
	public TextureRect AugmentIcon { get; set; }
	public RichTextLabel AugmentDescription { get; set; }
	public AugmentType CardType { get; set; }
	public (string StatKey, float Value) CardValue { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BackgroundColor = GetNode<ColorRect>("ColorRect");
		LevelText = GetNode<Label>("VBoxContainer/MarginContainer2/Label");
		AugmentIcon = GetNode<TextureRect>("VBoxContainer/MarginContainer3/TextureRect");
		AugmentDescription = GetNode<RichTextLabel>("VBoxContainer/MarginContainer/RichTextLabel");
		ProcessMode = Node.ProcessModeEnum.WhenPaused;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _GuiInput(InputEvent @event)
	{
		base._GuiInput(@event);
		// LoggingUtils.Debug("HEllo");
		if (@event is InputEventMouseButton mb)
		{
			if (mb.ButtonIndex == MouseButton.Left && mb.Pressed)
			{
				Augment.OnSelectedAugmentCard(CardType, CardValue.StatKey, CardValue.Value);
				Augment.EndAugmentSelection();
			}
		}
	}

	public void SetAugmentCard(CardRarity cardRarity, AugmentType cardType, Color backgroundColor, int currentLevel, float value = 0.0f, string statKey = "Default")
	{
		if(cardType == AugmentType.Stat && statKey == "Default")
		{
			LoggingUtils.Error($"[{typeof(AugmentCard)}] AugmentType: {cardType}, StatType: {statKey}");
			throw new Exception("No stat type was specified to set augment card. Please check the logs");
		}

		LoggingUtils.Debug($"{backgroundColor}");
		BackgroundColor.Color = backgroundColor;
		LevelText.Text = $"Lv. {currentLevel}";
		CardValue = (statKey, value);
		// AugmentIcon
		AugmentDescription.Text = $"Level up {statKey} stats. +{value}%";
	}
}
