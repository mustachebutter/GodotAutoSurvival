using System;
using System.Collections.Generic;

public static class DataParser
{
	public readonly static string[] STATS_MAPPER = {"Health", "Attack", "AttackRange", "AttackSpeed", "Speed", "Crit", "CritDamage", "Defense", "ElementalResistance" };
	private readonly static string FILE_METADATA_STATUS_EFFECT  = "res://metadata/GodotAutoSurvival_Metadata_StatusEffect.tsv";
	private readonly static string FILE_METADATA_WEAPON  = "res://metadata/GodotAutoSurvival_Metadata_Weapon.tsv";
	private readonly static string FILE_METADATA_WEAPON_DAMAGE = "res://metadata/GodotAutoSurvival_Metadata_WeaponMetadata.tsv";
	private readonly static string FILE_METADATA_STATS = "res://metadata/GodotAutoSurvival_Metadata_Stats.tsv";
	private readonly static string FILE_METADATA_CHARACTER_LEVEL = "res://metadata/GodotAutoSurvival_Metadata_CharacterLevel.tsv";
	private readonly static string FILE_AUGMENT_RATE = "res://metadata/GodotAutoSurvival_Metadata_AugmentRate.tsv";

	private static Dictionary<string, StatusEffectData> _statusEffectDictionary = new Dictionary<string, StatusEffectData>();
	private static Dictionary<string, WeaponData> _weaponEffectDictionary = new Dictionary<string, WeaponData>();
	private static List<WeaponDamageData> _weaponDamageDatabase = new List<WeaponDamageData>();
	private readonly static List<CharacterStatData> _characterStatDatabase = new List<CharacterStatData>();
	private readonly static List<CharacterLevelData> _characterLevelDatabase = new List<CharacterLevelData>();
	private readonly static List<AugmentRateData> _augmentRateDatabase = new List<AugmentRateData>();

	static DataParser()
	{
		var seList = StatusEffectDataParser.ParseData(FILE_METADATA_STATUS_EFFECT);
		
		foreach (var se in seList)
		{
			_statusEffectDictionary.Add(se.StatusEffectId, se);
		}

		var (pList, pdList) = WeaponDataParser.ParseData(FILE_METADATA_WEAPON, FILE_METADATA_WEAPON_DAMAGE);
		_weaponDamageDatabase = pdList;
		
		foreach (var p in pList)
		{
			_weaponEffectDictionary.Add(p.WeaponId, p);
		}

		_characterStatDatabase = CharacterStatDataParser.ParseData(FILE_METADATA_STATS);
		_characterLevelDatabase = CharacterStatDataParser.ParseLevelData(FILE_METADATA_CHARACTER_LEVEL);
		_augmentRateDatabase = AugmentRateDataParser.ParseData(FILE_AUGMENT_RATE);
	}

	public static StatusEffectData GetStatusEffectData(string key)
	{
		StatusEffectData statusEffectData;
		if(_statusEffectDictionary.TryGetValue(key, out statusEffectData))
		{
			// We have to return a DeepCopy because we are instantiating StatusEffect Data
			// one time only. Deep copy will generate a new object with a different
			// address and default state variables.
			return statusEffectData.DeepCopy();
		}
		else
		{
			LoggingUtils.Error("No status effect was found, applying the default status effect values!");
			return _statusEffectDictionary["Status_Default"];
		}
	}

	public static WeaponData GetWeaponData(string key)
	{
		WeaponData weaponData;
		if(_weaponEffectDictionary.TryGetValue(key, out weaponData))
		{
			return weaponData;
		}
		else
		{
			LoggingUtils.Error("No weapon was found, applying the default weapon values!");
			return _weaponEffectDictionary["Weapon_Default"];
		}
	}

	public static Dictionary<string, WeaponData> GetWeaponDatabase()
	{
		return _weaponEffectDictionary;
	}

	public static List<WeaponDamageData> GetWeaponDamageDatabase()
	{
		return _weaponDamageDatabase;
	}

	public static List<CharacterStatData> GetCharacterStatDatabase()
	{
		return _characterStatDatabase;
	}

	public static List<CharacterLevelData> GetCharacterLevelDatabase()
	{
		return _characterLevelDatabase;
	}

	public static List<AugmentRateData> GetAugmentRateDatabase()
	{
		return _augmentRateDatabase;
	}

	public static UpgradableObject GetStatByLevel(string statKey = "Default", int level = 1)
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
		
		var characterStatDatabase = GetCharacterStatDatabase();
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

	public static UpgradableObject GetWeaponDamageByLevel(string weaponId = "Weapon_Default", string weaponDamageStat = "WStat_Default", int level = 1)
	{
		if (weaponId == "Weapon_Default")
		{
			LoggingUtils.Error("Did not supply any weapon ID");
			return null;
		}

		if (weaponDamageStat == "WStat_Default")
		{
			LoggingUtils.Error("Did not supply any weapon damage stat");
			return null;
		}

		if (level <= 0)
		{
			LoggingUtils.Error($"Tried to retrieve weapon with an impossible level {level}");
			return null;
		}

		var weaponDamageDatabse = GetWeaponDamageDatabase();
		var currentWeaponDamageData = weaponDamageDatabse.Find(x => level == x.MainLevel);

		if (currentWeaponDamageData == null)
		{
			LoggingUtils.Error($"Could not find weapon {weaponId} with stat {weaponDamageStat} with level {level} in the database");
			throw new Exception("Failed to get value of weapon, please check the log");
		}

		UpgradableObject wds = weaponDamageStat switch 
		{
			"WStat_Damage" => currentWeaponDamageData.Damage,
			"WStat_AttackSpeed" => currentWeaponDamageData.AttackSpeed,
			"WStat_Speed" => currentWeaponDamageData.Speed,
			_ => null
		};

		if (wds == null)
		{
			LoggingUtils.Error($"Could not find stat {weaponDamageStat}, please double check the key");
			throw new Exception("Failed to get value of weapon, please check the log");
		}

		return wds;
	}

	public static AugmentRateData GetAugmentRateFromLevel(int level)
	{
		AugmentRateData augmentRateFound = _augmentRateDatabase.Find(x => x.Level == level);
		return augmentRateFound.DeepCopy();
	}
}