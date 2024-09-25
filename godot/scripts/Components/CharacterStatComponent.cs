using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;

public partial class CharacterStatComponent : Node2D
{
	public const int MAX_LEVEL = 30;
	public CharacterStatData CharacterStatData { get; set; }

    public override void _Ready()
    {
        base._Ready();
		var characterStatDict = CharacterStatParsedData.GetCharacterStatDictionary();

		CharacterStatData = new CharacterStatData 
		{
			Health = characterStatDict["Health"][0],
			Attack = characterStatDict["Attack"][0],
			AttackRange = characterStatDict["AttackRange"][0],
			AttackSpeed = characterStatDict["AttackSpeed"][0],
			Speed = characterStatDict["Speed"][0],
			Crit = characterStatDict["Crit"][0],
			CritDamage = characterStatDict["CritDamage"][0],
			Defense = characterStatDict["Defense"][0],
			ElementalResistance = characterStatDict["ElementalResistance"][0],
		};
    }

	public void UpgradeStatLevel(string statKey, int levelToUpgrade = 1)
	{
		var stat = CharacterStatData.GetPropertyValue(statKey);

		if (stat == null)
		{
			LoggingUtils.Error("Attempted to upgrade a non existing stat");
			return;
		}

		if (stat.Level + levelToUpgrade > MAX_LEVEL)
		{
			LoggingUtils.Debug("Max Level reached. Cannot upgrade anymore!");
		}

		LoggingUtils.Debug($"Before upgrade ({statKey}): LVL {stat.Level} - {stat.Value}");
		stat.Level = stat.Level + levelToUpgrade;
		stat.Value = GetValueOfStat(statKey, stat.Level);
		LoggingUtils.Debug($"After upgrade ({statKey}): LVL {stat.Level} - {stat.Value}");
	}

	public float GetValueOfStat(string statKey, int level)
	{
		var statDictionary = CharacterStatParsedData.GetCharacterStatDictionary();

		if (statDictionary.TryGetValue(statKey, out List<Stat> allLevelsOfStat))
		{
			return allLevelsOfStat.Find(x => x.Level == level).Value;
		}
		
		LoggingUtils.Error($"Could not find stat {statKey} with level {level} in the dictionary");
		throw new Exception("Failed to get value of stat, please check the log");
	}
}