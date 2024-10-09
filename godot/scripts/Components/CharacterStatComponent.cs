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

	public UpgradableObject GetStatFromName(string statKey = "Default")
	{
		if (statKey == "Default")
		{
			LoggingUtils.Error("Tried to retrieve a non existing stat");
			return null;
		}

		UpgradableObject currentStatValue = statKey switch
		{
			"Health" => CharacterStatData.Health,
			"Attack" => CharacterStatData.Attack,
			"AttackRange" => CharacterStatData.AttackRange,
			"AttackSpeed" => CharacterStatData.AttackSpeed,
			"Speed" => CharacterStatData.Speed,
			"Crit" => CharacterStatData.Crit,
			"CritDamage" => CharacterStatData.CritDamage,
			"Defense" => CharacterStatData.Defense,
			"ElementalResistance" => CharacterStatData.ElementalResistance,
			_ => null,
		};

		if (currentStatValue == null)
		{
			LoggingUtils.Error($"Could not find stat {statKey}. Please add them or double check the key");
			throw new Exception("Failed to get value of stat, please check the log");
		}

		return currentStatValue;
	}

	public UpgradableObject GetStatFromDatabase(string statKey = "Default", int level = 1)
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
		UpgradableObject currentStatValue = statKey switch
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

	public void UpgradeStatLevel(string statKey, int levelToUpgrade = 1)
	{
		var currentStatValue = GetStatFromName(statKey);

		if (currentStatValue.Level + levelToUpgrade > MAX_LEVEL)
		{
			LoggingUtils.Error("#################################");
			LoggingUtils.Error("Max Level reached. Cannot upgrade anymore!");
			LoggingUtils.Error($"Current Level = {currentStatValue.Level}, Level To Upgrade = {levelToUpgrade}");
			return;
		}

		LoggingUtils.Debug($"Before upgrade: LVL {currentStatValue.Level} - {currentStatValue.Value}");
		currentStatValue.Level = currentStatValue.Level + levelToUpgrade;
		currentStatValue.Value = GetStatFromDatabase(currentStatValue.Name, currentStatValue.Level).Value;
		LoggingUtils.Debug($"After upgrade: LVL {currentStatValue.Level} - {currentStatValue.Value}");
	}

	public void AddStat(string statKey, float amount = 0.0f)
	{
		var currentStatValue = GetStatFromName(statKey);
		currentStatValue.Value = currentStatValue.Value + amount;
	}

	public void ReduceStat(string statKey, float amount = 0.0f)
	{
		var currentStatValue = GetStatFromName(statKey);
		if (currentStatValue.Value - amount < 0)
		{
			LoggingUtils.Debug("Reduced stat to lower than 0");
			currentStatValue.Value = 0;
			return;
		}

		currentStatValue.Value = currentStatValue.Value - amount;
		
	}

}