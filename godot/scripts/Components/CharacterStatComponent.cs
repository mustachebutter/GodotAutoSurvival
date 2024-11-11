using System;
using Godot;

public partial class CharacterStatComponent : Node2D
{
	public CharacterStatData CharacterStatData { get; set; }
	public CharacterStatData StatModifierData { get; set; } = new CharacterStatData();
	public event Action<UpgradableObject, float, float, float> OnAnyStatUpgraded;

    public override void _Ready()
    {
		base._Ready();
		var firstCharacterStatLevel = DataParser.GetCharacterStatDatabase()[0].DeepCopy();
		CharacterStatData = firstCharacterStatLevel;
		foreach(var statKey in GlobalConfigs.STATS)
		{
			var stat = GetStatFromName(statKey);
			stat.OnLevelChanged += HandleOnStatLevelChanged; 
		}
    }

    private void HandleOnStatLevelChanged(UpgradableObject @object)
    {
        (float baseValue, float modifierValue, float totalValue) = GetAllStatFromName(@object.Name);

		OnAnyStatUpgraded?.Invoke(@object, baseValue, modifierValue, totalValue);
    }

    public UpgradableObject GetStatFromName(string statKey = "Default", StatTypes statType = StatTypes.Stat)
	{
		if (statKey == "Default")
		{
			LoggingUtils.Error("Tried to retrieve a non existing stat");
			return null;
		}

		CharacterStatData csd = statType == StatTypes.Stat ? CharacterStatData : StatModifierData;

		UpgradableObject currentStatValue = statKey switch
		{
			"Health" => csd.Health,
			"Attack" => csd.Attack,
			"AttackRange" => csd.AttackRange,
			"AttackSpeed" => csd.AttackSpeed,
			"Speed" => csd.Speed,
			"Crit" => csd.Crit,
			"CritDamage" => csd.CritDamage,
			"Defense" => csd.Defense,
			"ElementalResistance" => csd.ElementalResistance,
			_ => null,
		};

		if (currentStatValue == null)
		{
			LoggingUtils.Error($"Could not find stat {statKey}. Please add them or double check the key");
			throw new Exception("Failed to get value of stat, please check the log");
		}

		return currentStatValue;
	}

	public (float baseValue, float modifierValue, float totalValue) GetAllStatFromName(string statKey)
	{
		UpgradableObject stat = GetStatFromName(statKey);
		UpgradableObject modifier = GetStatFromName(statKey, StatTypes.Modifier);

		float totalValue = stat.Value * (100 + modifier.Value) / 100;

		return (stat.Value, modifier.Value, totalValue);
	}

	public void AddStat(string statKey, float amount = 0.0f, StatTypes statType = StatTypes.Stat)
	{
		var currentStatValue = GetStatFromName(statKey, statType);
		currentStatValue.Value = currentStatValue.Value + amount;
	}

	public void ReduceStat(string statKey, float amount = 0.0f, StatTypes statType = StatTypes.Stat)
	{
		var currentStatValue = GetStatFromName(statKey, statType);
		if (currentStatValue.Value - amount <= 0)
		{
			LoggingUtils.Debug("Reduced stat to lower than 0");
			currentStatValue.Value = 0;
			return;
		}

		currentStatValue.Value = currentStatValue.Value - amount;
	}
}