using System;
using System.Collections.Generic;
using System.Linq;
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

    public static void OnSelectedAugmentCard(AugmentType augmentType, string statKey = "Default", float statModifierValue = 0.0f)
    {
        switch(augmentType)
        {
            case AugmentType.Stat:
                Player player = UtilGetter.GetMainPlayer();
                UpgradableObject stat = player.CharacterStatComponent.GetStatFromName(statKey);
                player.CharacterStatComponent.AddStat(statKey, statModifierValue, StatTypes.Modifier);
                LoggingUtils.Debug($"Modifier {statKey}: {player.CharacterStatComponent.StatModifierData}");
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

        // Determine rarity
        AugmentRateData augmentRate = DataParser.GetAugmentRateFromLevel(level);
        LoggingUtils.Debug($"Common: {augmentRate.CommonRate}, Rare: {augmentRate.RareRate}, Epic: {augmentRate.EpicRate}, Legendary: {augmentRate.LegendaryRate}, Mythic: {augmentRate.MythicRate}, ");
        int rareRate = augmentRate.CommonRate + augmentRate.RareRate;
        int epicRate = rareRate + augmentRate.EpicRate;
        int legendaryRate = epicRate + augmentRate.LegendaryRate;

        // Set stat priority to prioritize stats upgrade to spawn
        List<UpgradableObject> characterStats = new List<UpgradableObject>();
        foreach (var statKey in GlobalConfigs.STATS)
        {
            UpgradableObject stat = player.CharacterStatComponent.GetStatFromName(statKey);
            characterStats.Add(stat);
        }
        characterStats = characterStats
            .OrderByDescending(s => s.Level)
            .ThenBy(s => s.Name)
            .ToList();

        // Setting each card
        foreach (var ac in augmentCards)
        {
            Random random = new Random();
            int randomNumber = random.Next(1, 101);
            int randomChanceOfStatPriority = random.Next(0, 10);
            int randomIndex = random.Next(0, 2);
            int randomIndex2 = random.Next(2, GlobalConfigs.STATS.Length);
            
            // We want to prioritize stats that have been upgraded but the chances of getting other stats are never 0
            int indexOfStatToUpgrade = randomChanceOfStatPriority <= 7 ? randomIndex : randomIndex2;            
            UpgradableObject statKeyToUpgrade = characterStats[indexOfStatToUpgrade];

            if (randomNumber <= augmentRate.CommonRate)
            {
                ac.SetAugmentCard(CardRarity.Common, AugmentType.Stat, Colors.BLUE, statKeyToUpgrade.Level, 5.0f, statKeyToUpgrade.Name);
            }
            else if (randomNumber > augmentRate.CommonRate && randomNumber <= rareRate)
            {
                ac.SetAugmentCard(CardRarity.Rare, AugmentType.Stat, Colors.GREEN, statKeyToUpgrade.Level, 10.0f, statKeyToUpgrade.Name);
            }
            else if (randomNumber > rareRate && randomNumber <= epicRate)
            {
                ac.SetAugmentCard(CardRarity.Epic, AugmentType.Stat, Colors.PURPLE, statKeyToUpgrade.Level, 20.0f, statKeyToUpgrade.Name);
            }
            else if (randomNumber > epicRate && randomNumber <= legendaryRate)
            {
                ac.SetAugmentCard(CardRarity.Legendary, AugmentType.Stat, Colors.YELLOW, statKeyToUpgrade.Level, 50.0f, statKeyToUpgrade.Name);
            }
            else
            {
                ac.SetAugmentCard(CardRarity.Mythic, AugmentType.Stat, Colors.RED, statKeyToUpgrade.Level, 75.0f, statKeyToUpgrade.Name);
            }
        }
        
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        GlobalConfigs.OnGamePausedChanged -= _onGamePausedHandler;
    }
}