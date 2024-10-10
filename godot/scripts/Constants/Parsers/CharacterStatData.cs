using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class UpgradableObject
{
    public string Name { get; set; }
    public int Level { get; set; }
    public float Value { get; set; }

    public UpgradableObject DeepCopy()
    {
        return new UpgradableObject { Name = this.Name, Level = this.Level, Value = this.Value };
    }

    public void Upgrade(UpgradableObjectTypes type, int levelToUpgrade = 1)
    {
        int maxLevel = type switch
        {
            UpgradableObjectTypes.Stat => 30,
            UpgradableObjectTypes.Weapon => 12,
            UpgradableObjectTypes.StatusEffect => 10,
            _ => 1
        };

        if (maxLevel == 1)
        {
            LoggingUtils.Error("Passing in the incorrect UpgradableObjectType while trying to upgrade stat");
            return;
        }

		if (Level + levelToUpgrade > maxLevel)
		{
			LoggingUtils.Error("#################################");
			LoggingUtils.Error("Max Level reached. Cannot upgrade anymore!");
			LoggingUtils.Error($"Current Level = {Level}, Level To Upgrade = {levelToUpgrade}");
			return;
		}

		LoggingUtils.Debug($"Before upgrade: LVL {Level} - {Value}");
		Level = Level + levelToUpgrade;
		Value = DataParser.GetStatFromDatabase(Name, Level).Value;
		LoggingUtils.Debug($"After upgrade: LVL {Level} - {Value}");
    }

    public void Downgrade(int levelToDowngrade = 1)
    {
        if (Level - levelToDowngrade < 0)
        {
            LoggingUtils.Error("#################################");
			LoggingUtils.Error("Lowest Level reached. Cannot downgrade anymore!");
			LoggingUtils.Error($"Current Level = {Level}, Level To Downgrade = {levelToDowngrade}");
			return;
        }

        LoggingUtils.Debug($"Before downgrade: LVL {Level} - {Value}");
		Level = Level - levelToDowngrade;
		Value = DataParser.GetStatFromDatabase(Name, Level).Value;
		LoggingUtils.Debug($"After downgrade: LVL {Level} - {Value}");
    }
}

public class CharacterLevelData
{
    public int Level { get; set; }
    public int ExperienceToLevelUp { get; set; }

    public CharacterLevelData DeepCopy()
    {
        return new CharacterLevelData
        {
            Level = this.Level,
            ExperienceToLevelUp = this.ExperienceToLevelUp,
        };
    }
}

public class CharacterStatData
{
	public UpgradableObject Attack { get; set;} = new UpgradableObject { Level = 1, Value = 100.0f };
	public UpgradableObject AttackRange { get; set; } = new UpgradableObject { Level = 1, Value = 500.0f };
	public UpgradableObject AttackSpeed { get; set; } = new UpgradableObject { Level = 1, Value = 1.0f };
	public UpgradableObject Health { get; set;} = new UpgradableObject { Level = 1, Value = 100.0f };
	public UpgradableObject Defense { get; set; } = new UpgradableObject { Level = 1, Value = 50.0f };
	public UpgradableObject ElementalResistance { get; set; } = new UpgradableObject { Level = 1, Value = 50.0f };
	public UpgradableObject Speed { get; set; } = new UpgradableObject { Level = 1, Value = 100.0f };
	public UpgradableObject Crit { get; set; } = new UpgradableObject { Level = 1, Value = 1.0f };
	public UpgradableObject CritDamage { get; set; } = new UpgradableObject { Level = 1, Value = 100.0f };

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
                    Health = new UpgradableObject { Name = keys[1], Level = level, Value = float.Parse(content[1]) },
                    Attack = new UpgradableObject { Name = keys[2], Level = level, Value = float.Parse(content[2]) },
                    AttackRange = new UpgradableObject { Name = keys[3], Level = level, Value = float.Parse(content[3]) },
                    AttackSpeed = new UpgradableObject { Name = keys[4], Level = level, Value = float.Parse(content[4]) },
                    Speed = new UpgradableObject { Name = keys[5], Level = level, Value = float.Parse(content[5]) },
                    Crit = new UpgradableObject { Name = keys[6], Level = level, Value = float.Parse(content[6]) },
                    CritDamage = new UpgradableObject { Name = keys[7], Level = level, Value = float.Parse(content[7]) },
                    Defense = new UpgradableObject { Name = keys[8], Level = level, Value = float.Parse(content[8]) },
                    ElementalResistance = new UpgradableObject { Name = keys[9], Level = level, Value = float.Parse(content[9]) },
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

    public static List<CharacterLevelData> ParseLevelData(string path)
    {
        var characterLevels = new List<CharacterLevelData>();
        var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if(file == null)
            LoggingUtils.Error($"Failed to parse file {path}");
        
        // Skip the first header line
        string[] keys = file.GetCsvLine("\t");
        try
        {
            LoggingUtils.Info("CharacterLevelData Parsing!!!", isBold: true, fontSize: 20);
            while(!file.EofReached())
            {
                string[] content = file.GetCsvLine("\t");
                LoggingUtils.Info($"Parsing {content.Length} column(s)");
                if (content.Length == 1)
                {
                    continue;
                }

                var level = int.Parse(content[0]);
                var cld = new CharacterLevelData
                {
                    Level = int.Parse(content[0]),
                    ExperienceToLevelUp = int.Parse(content[1]),
                };
                
                characterLevels.Add(cld);
            }

            if(characterLevels.Count == 0)
                LoggingUtils.Error("Wasn't able to parsed any character level data!");
    
            return characterLevels;
        }
        catch(FormatException ex)
        {
            LoggingUtils.Error($"Failed to parse column for character level: {ex}");
        }
        catch(IndexOutOfRangeException ex)
        {
            LoggingUtils.Error($"The number of parsed columns doesn't match the expected for character level: {ex}");
        }
        catch(ArgumentNullException ex)
        {
            LoggingUtils.Error($"Tried to parse a null value: {ex}");
        }

        return null;
    }
}