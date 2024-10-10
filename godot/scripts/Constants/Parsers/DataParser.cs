using System.Collections.Generic;

public static class DataParser
{
	public readonly static string[] STATS_MAPPER = {"Health", "Attack", "AttackRange", "AttackSpeed", "Speed", "Crit", "CritDamage", "Defense", "ElementalResistance" };
	private readonly static string FILE_METADATA_STATUS_EFFECT  = "res://metadata/GodotAutoSurvival_Metadata_StatusEffect.tsv";
	private readonly static string FILE_METADATA_WEAPON  = "res://metadata/GodotAutoSurvival_Metadata_Weapon.tsv";
	private readonly static string FILE_METADATA_WEAPON_DAMAGE = "res://metadata/GodotAutoSurvival_Metadata_WeaponMetadata.tsv";
	private readonly static string FILE_METADATA_STATS = "res://metadata/GodotAutoSurvival_Metadata_Stats.tsv";

	private static Dictionary<string, StatusEffectData> _statusEffectDictionary = new Dictionary<string, StatusEffectData>();
	private static Dictionary<string, WeaponData> _weaponEffectDictionary = new Dictionary<string, WeaponData>();
	private static List<WeaponDamageData> _weaponDamageDatabase = new List<WeaponDamageData>();
	private readonly static List<CharacterStatData> _characterStatDatabase = new List<CharacterStatData>();


	static DataParser()
	{
		var seList = StatusEffectDataParser.ParseData(FILE_METADATA_STATUS_EFFECT);
		
		foreach (var se in seList)
		{
			_statusEffectDictionary.Add(se.StatusEffectId, se);
		}

		var pList = WeaponDataParser.ParseData(FILE_METADATA_WEAPON);
		
		foreach (var p in pList)
		{
			_weaponEffectDictionary.Add(p.WeaponId, p);
		}
		_weaponDamageDatabase = WeaponDataParser.ParseDamageData(FILE_METADATA_WEAPON_DAMAGE);

		_characterStatDatabase = CharacterStatDataParser.ParseData(FILE_METADATA_STATS);

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

	public static Dictionary<string, WeaponData> GetAllData()
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

}