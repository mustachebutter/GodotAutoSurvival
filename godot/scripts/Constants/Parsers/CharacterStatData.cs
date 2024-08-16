using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class CharacterStatData
{
	public float Attack { get; set;} = 100.0f;
	public float AttackRange { get; set; } = 500.0f;
	public float AttackSpeed { get; set; } = 1.0f;

	public float Health { get; set;} = 0.0f;
	public float Defense { get; set; } = 50.0f;
	public float ElementalResistance { get; set; } = 50.0f;

	public float Speed { get; set; } = 100.0f;
	public float Crit { get; set; } = 1.0f;
	public float CritDamage { get; set; } = 100.0f;

    public float GetPropertyValue(string propertyName)
    {
        var propertyInfo = GetType().GetProperty(propertyName);
        if (propertyInfo == null)
        {
            LoggingUtils.Error($"Tried to get an non-existing property of character data {propertyName}");
            throw new Exception("Error getting character stat, please check error log");
        }

        return (float) propertyInfo.GetValue(this);
    }
}

public static class CharacterStatDataParser
{
    // public static (string[], Dictionary<int, CharacterStatData>) ParseData(string path)
    public static (string[], List<(int, CharacterStatData)>) ParseData(string path)
    {
        var characterStats = new List<(int, CharacterStatData)>();
        var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if(file == null)
            LoggingUtils.Error($"Failed to parse file {path}");
        
        // Skip the first header line
        string[] keys = file.GetCsvLine("\t");
        try
        {
            LoggingUtils.Info("CharacterStatData Parsing!!!", isBold: true, fontSize: 20);
            while(!file.EofReached())
            {
                string[] content = file.GetCsvLine("\t");
                LoggingUtils.Info($"Parsing {content.Length} column(s)");
                if (content.Length == 1)
                {
                    continue;
                }

                var level = int.Parse(content[0]);
                var csd = new CharacterStatData
                {
                    Health = float.Parse(content[1]),
                    Attack = float.Parse(content[2]),
                    AttackRange = float.Parse(content[3]),
                    AttackSpeed = float.Parse(content[4]),
                    Speed = float.Parse(content[5]),
                    Crit = float.Parse(content[6]),
                    CritDamage = float.Parse(content[7]),
                    Defense = float.Parse(content[8]),
                    ElementalResistance = float.Parse(content[9]),
                };
                
                characterStats.Add((level, csd));
            }

            if(characterStats.Count == 0)
                LoggingUtils.Error("Wasn't able to parsed any character stats data!");
    
            return (keys, characterStats);
        }
        catch(FormatException ex)
        {
            LoggingUtils.Error($"Failed to parse column for character stat: {ex}");
        }
        catch(IndexOutOfRangeException ex)
        {
            LoggingUtils.Error($"The number of parsed columns doesn't match the expected for character stat: {ex}");
        }
        catch(ArgumentNullException ex)
        {
            LoggingUtils.Error($"Tried to parse a null value: {ex}");
        }


        return (Array.Empty<string>(), null);
    }
}