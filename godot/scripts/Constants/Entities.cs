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
    // public static PackedScene Projectile = (PackedScene) GD.Load("res://scenes/projectiles/projectile.tscn");
    // public static PackedScene ProjectileZap = (PackedScene) GD.Load("res://scenes/projectiles/projectile_zap.tscn");
    // public static PackedScene ProjectileFireball = (PackedScene) GD.Load("res://scenes/projectiles/projectile_fireball.tscn");
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
        var path  = "res://metadata/GodotAutoSurvival_Metadata_StatusEffect.txt";
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
            return statusEffectData;
        }
        else
        {
            GD.PrintErr("No status effect was found, applying the default status effect values!");
            return dictionary["Status_Default"];
        }
    }
}

public static class ProjectileParsedData
{
    public static Dictionary<string, ProjectileData> dictionary = new Dictionary<string, ProjectileData>();
    static ProjectileParsedData()
    {
        var path  = "res://metadata/GodotAutoSurvival_Metadata_Weapon.tsv";
        var pList = ProjectileDataParser.ParseData(path);
        
        foreach (var p in pList)
        {
            dictionary.Add(p.ProjectileId, p);
        }
    }

    public static ProjectileData GetData(string key)
    {
        ProjectileData projectileData;
        if(dictionary.TryGetValue(key, out projectileData))
        {
            return projectileData;
        }
        else
        {
            GD.PrintErr("No projectile was found, applying the default projectile values!");
            return dictionary["Weapon_Default"];
        }
    }

    public static Dictionary<string, ProjectileData> GetAllData()
    {
        return dictionary;
    }
}