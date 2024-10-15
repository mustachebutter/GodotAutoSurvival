using Godot;

public partial class AugmentHUD : CanvasLayer
{
	public const int MAX_AUGMENTS = 3;
	public HBoxContainer AugmentContainer { get; set; }

	public override void _Ready()
	{
		base._Ready();

		AugmentContainer = GetNode<HBoxContainer>("Control/HBoxContainer");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	public void PopulateAugmentCard()
	{
		for (int i = 0; i < MAX_AUGMENTS; i++)
		{
			AugmentCard augmentCard = (AugmentCard) Scenes.AugmentCard.Instantiate();
			LoggingUtils.Debug($"{augmentCard.Name}");
			AugmentContainer.AddChild(augmentCard);
		};
	}
}
