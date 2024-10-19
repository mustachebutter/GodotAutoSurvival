using System;
using Godot;

public partial class Augment : Node2D
{
    public enum AugmentType { Stat, Weapon, Item };
    // Delegate
    private Action<bool> _onGamePausedHandler;

    public override void _Ready()
    {
        base._Ready();
        ProcessMode = ProcessModeEnum.Always; 
        _onGamePausedHandler = (bool isGamePause) => 
        {
            GetTree().Paused = isGamePause;
        };
        GlobalConfigs.OnGamePausedChanged += _onGamePausedHandler;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }
    public static void StartAugmentSelection()
    {
        // Pause the game
        GlobalConfigs.IsGamePaused = true;
        // Spawn cards
        UtilGetter.GetMainHUD().SetUpAugmentHUD();
        // Decide which cards should be added at which level
        // When the user selected the card then upgrade accordingly
    }

    public static void EndAugmentSelection()
    {
        // Unpause the game
        GlobalConfigs.IsGamePaused = false;
        UtilGetter.GetMainHUD().TurnOffAugmentHUD();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        GlobalConfigs.OnGamePausedChanged -= _onGamePausedHandler;
    }
}