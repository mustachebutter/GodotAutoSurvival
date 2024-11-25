using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class AugmentRateData
{
    public int Level { get; set; }
    public int CommonRate { get; set; }
    public int RareRate { get; set; }
    public int EpicRate { get; set; }
    public int LegendaryRate { get; set; }
    public int MythicRate { get; set; }
    public AugmentRateData DeepCopy()
    {
        return new AugmentRateData
        {
            Level = this.Level,
            CommonRate = this.CommonRate,
            RareRate = this.RareRate,
            EpicRate = this.EpicRate,
            LegendaryRate = this.LegendaryRate,
            MythicRate = this.MythicRate,
        };
    }
}

public class AugmentCardData
{
    public CardRarity CardRarity { get; set; }
    public AugmentType AugmentType { get; set; }
    public Texture2D CardIcon { get; set; }
    public Color BackgroundColor { get; set; }
    public int CurrentLevel { get; set; }
    // Stat specific
    public float StatUpgradeValue { get; set; }
    public string StatKeyToUpgrade { get; set; } = "Default";
    public UpgradableObject StatToUpgrade { get; set; }
    // Weapon specific
    public string WeaponName { get; set; }
    public WeaponData WeaponData { get; set; }
    // Item specific
    public AugmentCardData DeepCopy()
    {
        return new AugmentCardData
        {
            CardRarity = this.CardRarity,
            AugmentType = this.AugmentType,
            CardIcon = this.CardIcon,
            BackgroundColor = this.BackgroundColor,
            CurrentLevel = this.CurrentLevel,
        };
    }

    public AugmentType VerifyCardData(int randomChanceOfAugmentType, WeaponData weaponData, UpgradableObject statData, bool weaponAugmentHasAppeared)
    {
        AugmentType = randomChanceOfAugmentType > 7 ? AugmentType.Weapon : AugmentType.Stat;
        CurrentLevel = randomChanceOfAugmentType > 7 ? weaponData.WeaponDamageData.MainLevel : statData.Level;

        // Changed it to a stat augment if there is already a card that appears as a weapon
        if (weaponAugmentHasAppeared)
            AugmentType = AugmentType.Stat;

        WeaponData = weaponData;
        StatToUpgrade = statData;
        StatKeyToUpgrade = statData.Name;

        switch (AugmentType)
        {
            case AugmentType.Stat:
                if(StatKeyToUpgrade == "Default")
                {
                    LoggingUtils.Error($"[{typeof(AugmentCardData)}] AugmentType: {AugmentType}, StatType: {StatKeyToUpgrade}");
                    throw new Exception("No stat type was specified to set augment card. Please check the logs");
                }
            break;
            case AugmentType.Weapon:
                if(WeaponData == null)
                {
                    LoggingUtils.Error($"[{typeof(AugmentCardData)}] AugmentType: {AugmentType}, Weapon: null");
                    throw new Exception("No weapon was specified to set augment card. Please check the logs");
                }
            break;
            case AugmentType.Item:
            break;
        }
        return AugmentType;
    }
    public void SetCardData(ColorRect backgroundColor, Label levelText, RichTextLabel augmentDescription)
    {
        switch(AugmentType)
        {
            case AugmentType.Stat:
                augmentDescription.Text = $"Level up {StatKeyToUpgrade} stats. +{StatUpgradeValue}%";
            break;
            case AugmentType.Weapon:
                augmentDescription.Text = $"Upgrade weapon {WeaponData.WeaponName}\n\n";

                foreach (var item in GlobalConfigs.WEAPON_STATS)
                {
                    var currentLevelDamageData = DataParser.GetWeaponDamageByLevel(WeaponName, item, CurrentLevel);
                    var nextLevelDamageData = DataParser.GetWeaponDamageByLevel(WeaponName, item, CurrentLevel + 1);
                    augmentDescription.Text += $"{currentLevelDamageData.Name.Split("_")[1]}:";
                    augmentDescription.Text += $"[color=red]{currentLevelDamageData.Value}[/color] => [color=green]{nextLevelDamageData.Value}[/color]\n";
                }
            break;
            case AugmentType.Item:
            break;
        }

        backgroundColor.Color = BackgroundColor;
        levelText.Text = $"Lv. {CurrentLevel}";

    }
}

public static class AugmentRateDataParser
{
    public static List<AugmentRateData> ParseData(string path)
    {
        var augmentRates = new List<AugmentRateData>();
        var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if(file == null)
            LoggingUtils.Error($"Failed to parse file {path}");
        
        // Skip the first header line
        string[] keys = file.GetCsvLine("\t");
        try
        {
            LoggingUtils.Info("AugmentRateData Parsing!!!", isBold: true, fontSize: 20);
            while(!file.EofReached())
            {
                string[] content = file.GetCsvLine("\t");
                LoggingUtils.Info($"Parsing {content.Length} column(s)");
                if (content.Length == 1)
                {
                    continue;
                }

                var ar = new AugmentRateData
                {
                    Level = int.Parse(content[0]),
                    CommonRate = int.Parse(content[1]),
                    RareRate = int.Parse(content[2]),
                    EpicRate = int.Parse(content[3]),
                    LegendaryRate = int.Parse(content[4]),
                    MythicRate = int.Parse(content[5]),
                };
                
                augmentRates.Add(ar);
            }

            if(augmentRates.Count == 0)
                LoggingUtils.Error("Wasn't able to parsed any augment rate data!");
    
            return augmentRates;
        }
        catch(FormatException ex)
        {
            LoggingUtils.Error($"Failed to parse column for augment rate: {ex}");
        }
        catch(IndexOutOfRangeException ex)
        {
            LoggingUtils.Error($"The number of parsed columns doesn't match the expected for augment rate: {ex}");
        }
        catch(ArgumentNullException ex)
        {
            LoggingUtils.Error($"Tried to parse a null value: {ex}");
        }

        return null;
    }
}