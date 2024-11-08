using System;
using System.Collections.Generic;
using Godot;

public partial class Augment : Node2D
{
    // Delegate
    private Action<bool> _onGamePausedHandler;

    // TODO: Probably refactor this to hold some data
    // public List<AugmentData> augmentDataList;

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

    public static void OnSelectedAugmentCard(AugmentType augmentType, string statKey = "Default")
    {
        switch(augmentType)
        {
            case AugmentType.Stat:
                Player player = UtilGetter.GetMainPlayer();
                UpgradableObject stat = player.CharacterStatComponent.GetStatFromName(statKey);
                stat.Upgrade(UpgradableObjectTypes.Stat, 1);
                break;
            // TODO: Doesn't have these implemented yet!
            // case AugmentType.Weapon:
            //     break;
            // case AugmentType.Item:
            //     break;
            default:
                break;
        }
    }

    public static void SetUpAugmentCards(List<AugmentCard> augmentCards)
    {
        Player player = UtilGetter.GetMainPlayer();
        int level = player.CharacterLevelComponent.CurrentCharacterLevel.Level;

        AugmentRateData augmentRate = DataParser.GetAugmentRateFromLevel(level);
        LoggingUtils.Debug($"Common: {augmentRate.CommonRate}, Rare: {augmentRate.RareRate}, Epic: {augmentRate.EpicRate}, Legendary: {augmentRate.LegendaryRate}, Mythic: {augmentRate.MythicRate}, ");

        foreach (var ac in augmentCards)
        {
            Random random = new Random();
            int randomNumber = random.Next(1, 101);
            LoggingUtils.Debug($"Generated number: {randomNumber}");
            if (randomNumber <= augmentRate.CommonRate)
            {
                ac.SetAugmentCard(CardRarity.Common, AugmentType.Stat, Colors.BLUE, 1, 5.0f);
            }
            else if (randomNumber > augmentRate.CommonRate && randomNumber <= augmentRate.RareRate)
            {
                ac.SetAugmentCard(CardRarity.Rare, AugmentType.Stat, Colors.GREEN, 1, 10.0f);
            }
            else if (randomNumber > augmentRate.RareRate && randomNumber <= augmentRate.EpicRate)
            {
                ac.SetAugmentCard(CardRarity.Epic, AugmentType.Stat, Colors.PURPLE, 1, 20.0f);
            }
            else if (randomNumber > augmentRate.EpicRate && randomNumber <= augmentRate.LegendaryRate)
            {
                ac.SetAugmentCard(CardRarity.Legendary, AugmentType.Stat, Colors.YELLOW, 1, 50.0f);
            }
            else if (randomNumber > augmentRate.LegendaryRate && randomNumber <= augmentRate.MythicRate)
            {
                ac.SetAugmentCard(CardRarity.Mythic, AugmentType.Stat, Colors.RED, 1, 10.0f);
            }
        }
        
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        GlobalConfigs.OnGamePausedChanged -= _onGamePausedHandler;
    }
}