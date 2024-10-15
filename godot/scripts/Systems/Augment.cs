using Godot;

public partial class Augment : Node2D
{
    public enum AugmentType { Stat, Weapon, Item };

    public override void _Ready()
    {
        base._Ready();
        
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (GlobalConfigs.IsGamePaused)
        {
            GetTree().Paused = true;
        }
    }
    public static void StartAugmentSelection()
    {
        // Pause the game
        GlobalConfigs.IsGamePaused = true;
        LoggingUtils.Debug($"In augment selection, paused is {UtilGetter.GetPaused()}");
        UtilGetter.GetMainHUD().SetUpAugmentHUD();
        // Spawn cards
        // Decide which cards should be added at which level
        // When the user selected the card then upgrade accordingly
    }

    public static void EndAugmentSelection()
    {
        // Unpause the game
        GlobalConfigs.IsGamePaused = false;
    }
}