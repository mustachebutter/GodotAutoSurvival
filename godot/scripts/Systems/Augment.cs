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
        Player player = UtilGetter.GetMainPlayer();
        switch(augmentType)
        {
            case AugmentType.Stat:
                UpgradableObject stat = player.CharacterStatComponent.GetStatFromName(statKey);
                player.CharacterStatComponent.AddStat(statKey, statModifierValue, StatTypes.Modifier);
                stat.Upgrade(UpgradableObjectTypes.Stat, 1);
                break;
            case AugmentType.Weapon:
                player.WeaponComponent.Weapon.WeaponData.WeaponDamageData.UpgradeLevel(1);
                break;
            // TODO: Doesn't have these implemented yet!
            // case AugmentType.Item:
            //     break;
            default:
                break;
        }
    }

    public static void SetUpAugmentCards(List<AugmentCard> augmentCards)
    {
        Random random = new Random();
        bool weaponAugmentHasAppeared = false;

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
            int randomNumber = random.Next(1, 101);
            int randomChanceOfStatPriority = random.Next(0, 10);
            int randomIndex = random.Next(0, 2);
            int randomIndex2 = random.Next(2, GlobalConfigs.STATS.Length);
            int randomChanceOfAugmentType = random.Next(1, 10);
                        
            // We want to prioritize stats that have been upgraded but the chances of getting other stats are never 0
            int indexOfStatToUpgrade = randomChanceOfStatPriority <= 7 ? randomIndex : randomIndex2;            
            UpgradableObject statKeyToUpgrade = characterStats[indexOfStatToUpgrade];
            WeaponData weaponData = player.WeaponComponent.Weapon.WeaponData;


            AugmentCardData augmentCardData = new AugmentCardData();
            var augmentType = augmentCardData.VerifyCardData(randomChanceOfAugmentType, weaponData, statKeyToUpgrade, weaponAugmentHasAppeared);
            
            if (augmentType == AugmentType.Stat)
            {
                if (randomNumber <= augmentRate.CommonRate)
                {
                    augmentCardData.CardRarity = CardRarity.Common;
                    augmentCardData.BackgroundColor = Colors.BLUE;
                    augmentCardData.StatUpgradeValue = 5.0f;
                }
                else if (randomNumber > augmentRate.CommonRate && randomNumber <= rareRate)
                {
                    augmentCardData.CardRarity = CardRarity.Rare;
                    augmentCardData.BackgroundColor = Colors.GREEN;
                    augmentCardData.StatUpgradeValue = 10.0f;
                }
                else if (randomNumber > rareRate && randomNumber <= epicRate)
                {
                    // ac.SetAugmentCard(CardRarity.Epic, augmentType, Colors.PURPLE, statKeyToUpgrade.Level, 20.0f, statKeyToUpgrade.Name);
                    augmentCardData.CardRarity = CardRarity.Epic;
                    augmentCardData.BackgroundColor = Colors.PURPLE;
                    augmentCardData.StatUpgradeValue = 20.0f;
                }
                else if (randomNumber > epicRate && randomNumber <= legendaryRate)
                {
                    augmentCardData.CardRarity = CardRarity.Legendary;
                    augmentCardData.BackgroundColor = Colors.YELLOW;
                    augmentCardData.StatUpgradeValue = 50.0f;
                }
                else
                {
                    augmentCardData.CardRarity = CardRarity.Mythic;
                    augmentCardData.BackgroundColor = Colors.RED;
                    augmentCardData.StatUpgradeValue = 75.0f;
                }
            }
            else if (augmentType == AugmentType.Weapon)
            {
                augmentCardData.BackgroundColor = Colors.BLACK;
                weaponAugmentHasAppeared = true;
            }

            ac.SetAugmentCard(augmentCardData);
        }
        
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        GlobalConfigs.OnGamePausedChanged -= _onGamePausedHandler;
    }
}