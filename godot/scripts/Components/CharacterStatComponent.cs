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
		var characterStatDatabase = CharacterStatParsedData.GetCharacterStatDatabase();

		// Initialize with level 1
		CharacterStatData = characterStatDatabase[0].DeepCopy();
    }

	public Stat GetValueOfStat(string statKey = "Default", int level = 1)
	{

		if (statKey == "Default")
		{
			LoggingUtils.Error("Tried to retrieve a non existing stat");
			return null;
		}

		if (level <= 0)
		{
			LoggingUtils.Error($"Tried to retrieve stat with an impossible level {level}");
			return null;
		}
		
		var characterStatDatabase = CharacterStatParsedData.GetCharacterStatDatabase();
		var currentStatOfLevel = characterStatDatabase[level - 1];
		// Good old switch case :D
		Stat currentStatValue = statKey switch
		{
			"Health" => currentStatOfLevel.Health.DeepCopy(),
			"Attack" => currentStatOfLevel.Attack.DeepCopy(),
			"AttackRange" => currentStatOfLevel.AttackRange.DeepCopy(),
			"AttackSpeed" => currentStatOfLevel.AttackSpeed.DeepCopy(),
			"Speed" => currentStatOfLevel.Speed.DeepCopy(),
			"Crit" => currentStatOfLevel.Crit.DeepCopy(),
			"CritDamage" => currentStatOfLevel.CritDamage.DeepCopy(),
			"Defense" => currentStatOfLevel.Defense.DeepCopy(),
			"ElementalResistance" => currentStatOfLevel.ElementalResistance.DeepCopy(),
			_ => null,
		};

		if (currentStatValue == null)
		{
			LoggingUtils.Error($"Could not find stat {statKey} with level {level} in the database");
			throw new Exception("Failed to get value of stat, please check the log");
		}

		return currentStatValue;
	}

	public void UpgradeStatLevel(Stat stat, int levelToUpgrade = 1)
	{
		var currentStatValue = GetValueOfStat(stat.Name, stat.Level);

		LoggingUtils.Error("#################################");
		if (currentStatValue.Level + levelToUpgrade >= MAX_LEVEL)
		{
			LoggingUtils.Debug("Max Level reached. Cannot upgrade anymore!");
		}

		LoggingUtils.Debug($"Before upgrade: LVL {stat.Level} - {stat.Value}");
		stat.Level = stat.Level + levelToUpgrade;
		stat.Value = GetValueOfStat(stat.Name, stat.Level).Value;
		LoggingUtils.Debug($"After upgrade: LVL {stat.Level} - {stat.Value}");
	}

}