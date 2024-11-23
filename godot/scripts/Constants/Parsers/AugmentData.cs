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
    // Weapon specific
    // Item specific
    // public AugmentCardData(
    //     CardRarity cardRarity,
    //     AugmentType augmentType,
    //     // Texture2D CardIcon,
    //     Color backgroundColor,
    //     int currentLevel
    // )
    // {
    //     CardRarity = cardRarity;
    //     AugmentType = augmentType;
    //     BackgroundColor = backgroundColor;
    //     CurrentLevel = currentLevel;
    // }

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

    public void VerifyCardData(ColorRect backgroundColor, Label levelText, RichTextLabel augmentDescription)
    {
        switch(AugmentType)
        {
            case AugmentType.Stat:
                if(StatKeyToUpgrade == "Default")
                {
                    LoggingUtils.Error($"[{typeof(AugmentCardData)}] AugmentType: {AugmentType}, StatType: {StatKeyToUpgrade}");
                    throw new Exception("No stat type was specified to set augment card. Please check the logs");
                }

                augmentDescription.Text = $"Level up {StatKeyToUpgrade} stats. +{StatUpgradeValue}%";
            break;
            case AugmentType.Weapon:
                augmentDescription.Text = $"Upgrade weapon [weaponName]\n\n";
                augmentDescription.Text += $"Damage: [color=red][oldDamage][/color] => [color=green][newDamage][/color]\n";
                augmentDescription.Text += $"Attack Speed: [color=red][oldAttackSpeed][/color] => [color=green][newAttackSpeed][/color]\n";
                augmentDescription.Text += $"Missile Speed: [color=red][oldMissileSpeed][/color] => [color=green][newMissileSpeed][/color]\n";
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