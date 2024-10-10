using System;
using Godot;

public partial class CharacterStatComponent : Node2D
{
	public CharacterStatData CharacterStatData { get; set; }

    public override void _Ready()
    {
		base._Ready();
		var firstCharacterStatLevel = DataParser.GetCharacterStatDatabase()[0].DeepCopy();
		CharacterStatData = firstCharacterStatLevel;
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

	public void AddStat(string statKey, float amount = 0.0f)
	{
		var currentStatValue = GetStatFromName(statKey);
		currentStatValue.Value = currentStatValue.Value + amount;
	}

	public void ReduceStat(string statKey, float amount = 0.0f)
	{
		var currentStatValue = GetStatFromName(statKey);
		if (currentStatValue.Value - amount <= 0)
		{
			LoggingUtils.Debug("Reduced stat to lower than 0");
			currentStatValue.Value = 0;
			return;
		}

		currentStatValue.Value = currentStatValue.Value - amount;
	}
}