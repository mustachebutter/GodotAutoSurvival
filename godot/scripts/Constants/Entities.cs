using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class UtilGetter
{
	public static SceneTree GetSceneTree()
	{
		var sceneTree = (SceneTree) Engine.GetMainLoop() ?? throw new InvalidOperationException($"ERROR [{nameof(UtilGetter)}] Could not get the Scene Tree");
		
		return sceneTree;
	}

	public static Player GetMainPlayer()
	{
		var player = GetSceneTree().Root.GetNode<Node2D>("MotherNode/CharactersParentNode").GetNode<CharacterBody2D>("Player");
		
		if (player != null)
		{
			return (Player) player;
		}

		LoggingUtils.Error("Could not retrieved player node");
		throw new Exception("Could not retrieved player node");
	}

	public static MobSpawnerComponent GetMainMobSpawner()
	{
		var mobSpawner = GetSceneTree().Root.GetNode<Node2D>("MotherNode").GetNode<MobSpawnerComponent>("MobSpawnerComponent");

		if (mobSpawner != null)
		{
			return (MobSpawnerComponent) mobSpawner;
		}

		LoggingUtils.Error("Could not retrieved mob spawner node");
		throw new Exception("Could not retrieved mob spawner node");
	}
}
public static class ProjectileTypes
{
	// DEBUG: Do this for quick test
	// Please change this to an actual database that reads from .csv
	// Or a dictionary!
	public static string Zap = "Zap";
	public static string CrissCross = "CrissCross";
	public static string Fireball = "Fireball";
}

public enum DamageTypes
{
	Fire,
	Electric,
	Normal,
	Light,
}

public enum WeaponTypes
{
	Projectile,
	Beam,
}

public static class Scenes
{
	public static PackedScene Player = (PackedScene) GD.Load("res://scenes/characters/player.tscn");
	public static PackedScene Enemy = (PackedScene) GD.Load("res://scenes/characters/enemy.tscn");
	public static PackedScene UiDamageNumber = (PackedScene) GD.Load("res://scenes/ui/damage_number_component.tscn");
	public static PackedScene VfxBurnExplosion = (PackedScene) GD.Load("res://scenes/vfx/vfx_burn_explosion.tscn");
	public static PackedScene VfxChainLightning = (PackedScene) GD.Load("res://scenes/vfx/vfx_chain_lightning.tscn");
}

public static class StatusEffectParsedData
{
	public static Dictionary<string, StatusEffectData> dictionary = new Dictionary<string, StatusEffectData>();
	static StatusEffectParsedData()
	{
		var path  = "res://metadata/GodotAutoSurvival_Metadata_StatusEffect.tsv";
		var seList = StatusEffectDataParser.ParseData(path);
		
		foreach (var se in seList)
		{
			dictionary.Add(se.StatusEffectId, se);
		}
	}

	public static StatusEffectData GetData(string key)
	{
		StatusEffectData statusEffectData;
		if(dictionary.TryGetValue(key, out statusEffectData))
		{
			// We have to return a DeepCopy because we are instantiating StatusEffect Data
			// one time only. Deep copy will generate a new object with a different
			// address and default state variables.
			return statusEffectData.DeepCopy();
		}
		else
		{
			LoggingUtils.Error("No status effect was found, applying the default status effect values!");
			return dictionary["Status_Default"];
		}
	}
}

public static class WeaponParsedData
{
	public static Dictionary<string, WeaponData> dictionary = new Dictionary<string, WeaponData>();
	public static List<WeaponDamageData> weaponDamageDatabase = new List<WeaponDamageData>();
	static WeaponParsedData()
	{
		var path  = "res://metadata/GodotAutoSurvival_Metadata_Weapon.tsv";
		var path_damage = "res://metadata/GodotAutoSurvival_Metadata_WeaponMetadata.tsv";
		var pList = WeaponDataParser.ParseData(path);
		weaponDamageDatabase = WeaponDataParser.ParseDamageData(path_damage);
		
		foreach (var p in pList)
		{
			dictionary.Add(p.WeaponId, p);
		}
	}

	public static WeaponData GetData(string key)
	{
		WeaponData weaponData;
		if(dictionary.TryGetValue(key, out weaponData))
		{
			return weaponData;
		}
		else
		{
			LoggingUtils.Error("No weapon was found, applying the default weapon values!");
			return dictionary["Weapon_Default"];
		}
	}

	public static Dictionary<string, WeaponData> GetAllData()
	{
		return dictionary;
	}
}

public static class CharacterStatParsedData
{
	public readonly static string[] STATS_MAPPER = {"Health", "Attack", "AttackRange", "AttackSpeed", "Speed", "Crit", "CritDamage", "Defense", "ElementalResistance" };

	private readonly static List<CharacterStatData> _characterStatDataList = new List<CharacterStatData>();

	static CharacterStatParsedData()
	{
		var path = "res://metadata/GodotAutoSurvival_Metadata_Stats.tsv";
		_characterStatDataList = CharacterStatDataParser.ParseData(path);
	}

	public static List<CharacterStatData> GetCharacterStatDatabase()
	{
		return _characterStatDataList;
	}

}
