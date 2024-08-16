using System;
using System.Collections.Generic;
using Godot;

public partial class CharacterStatComponent : Node2D
{
    [ExportGroup("Offense")]
    [Export(PropertyHint.Range, "0, 999, 1")]
	public float Attack { get; set;} = 100.0f;
	[Export(PropertyHint.Range, "0, 999, 1")]
	public float AttackRange { get; set; } = 500.0f;
	[Export(PropertyHint.Range, "0, 2, 0.1")]
	public float AttackSpeed { get; set; } = 1.0f;

    [ExportGroup("Defense")]
	[Export(PropertyHint.Range, "0, 9999, 1")]
	public float Health { get; set;} = 0.0f;
	[Export(PropertyHint.Range, "0, 99, 1")]
	public float Defense { get; set; } = 50.0f;
	[Export(PropertyHint.Range, "0, 99, 1")]
	public float ElementalResistance { get; set; } = 50.0f;

    [ExportGroup("Utilities")]
	[Export(PropertyHint.Range, "0, 200, 1")]
	public float Speed { get; set; } = 100.0f;
    [Export(PropertyHint.Range, "0, 100, 1")]
	public float Crit { get; set; } = 1.0f;
	[Export(PropertyHint.Range, "0, 999, 1")]
	public float CritDamage { get; set; } = 100.0f;

	public Dictionary<string, (int Level, float Value)> StatDictionary { get; set; } = new Dictionary<string, (int Level, float Value)>();

    //TODO: Handling stats level
	public Dictionary<string, (int Level, float Value)> GetStats()
	{
		// Do some verification
		foreach (var item in StatDictionary)
		{
			(int level, float value) = item.Value;
			if (level < 1 || level > 30)
			{
				LoggingUtils.Error($"Attempting to get stat ({item.Key}) but level is {level} with value {value}");
				return null;
			}
		}

		return StatDictionary;
	}

	public (int Level, float Value) GetStat(string key)
	{
		var statDictionary = GetStats();

		if (statDictionary == null)
		{
			throw new Exception("Error getting stats, please check the error log");
		}

		if (!statDictionary.ContainsKey(key))
		{
			LoggingUtils.Error($"No key {key} found. Currently available keys: {statDictionary.Keys}");
			throw new Exception($"Error getting stat {key}, please check the error log");
		}

		(int Level, float Value) = statDictionary[key];

		return (Level, Value);
	}
    //TODO: Get stats from file
}