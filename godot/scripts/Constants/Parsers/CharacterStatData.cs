using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class Stat
{
    public string Name { get; set; }
    public int Level { get; set; }
    public float Value { get; set; }

    public Stat DeepCopy()
    {
        return new Stat { Name = this.Name, Level = this.Level, Value = this.Value };
    }
}


public class CharacterStatData
{
	public Stat Attack { get; set;} = new Stat { Level = 1, Value = 100.0f };
	public Stat AttackRange { get; set; } = new Stat { Level = 1, Value = 500.0f };
	public Stat AttackSpeed { get; set; } = new Stat { Level = 1, Value = 1.0f };
	public Stat Health { get; set;} = new Stat { Level = 1, Value = 100.0f };
	public Stat Defense { get; set; } = new Stat { Level = 1, Value = 50.0f };
	public Stat ElementalResistance { get; set; } = new Stat { Level = 1, Value = 50.0f };
	public Stat Speed { get; set; } = new Stat { Level = 1, Value = 100.0f };
	public Stat Crit { get; set; } = new Stat { Level = 1, Value = 1.0f };
	public Stat CritDamage { get; set; } = new Stat { Level = 1, Value = 100.0f };

    public CharacterStatData DeepCopy()
    {
        return new CharacterStatData
        {
            Attack = this.Attack.DeepCopy(),
            AttackRange = this.AttackRange.DeepCopy(),
            AttackSpeed = this.AttackSpeed.DeepCopy(),
            Health = this.Health.DeepCopy(),
            Defense = this.Defense.DeepCopy(),
            ElementalResistance = this.ElementalResistance.DeepCopy(),
            Speed = this.Speed.DeepCopy(),
            Crit = this.Crit.DeepCopy(),
            CritDamage = this.CritDamage.DeepCopy(),
        };
    }
}

public static class CharacterStatDataParser
{
    public static List<CharacterStatData> ParseData(string path)
    {
        var characterStats = new List<CharacterStatData>();
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
                    Health = new Stat { Name = keys[1], Level = level, Value = float.Parse(content[1]) },
                    Attack = new Stat { Name = keys[2], Level = level, Value = float.Parse(content[2]) },
                    AttackRange = new Stat { Name = keys[3], Level = level, Value = float.Parse(content[3]) },
                    AttackSpeed = new Stat { Name = keys[4], Level = level, Value = float.Parse(content[4]) },
                    Speed = new Stat { Name = keys[5], Level = level, Value = float.Parse(content[5]) },
                    Crit = new Stat { Name = keys[6], Level = level, Value = float.Parse(content[6]) },
                    CritDamage = new Stat { Name = keys[7], Level = level, Value = float.Parse(content[7]) },
                    Defense = new Stat { Name = keys[8], Level = level, Value = float.Parse(content[8]) },
                    ElementalResistance = new Stat { Name = keys[9], Level = level, Value = float.Parse(content[9]) },
                };
                
                characterStats.Add(csd);
            }

            if(characterStats.Count == 0)
                LoggingUtils.Error("Wasn't able to parsed any character stats data!");
    
            return characterStats;
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

        return null;
    }
}