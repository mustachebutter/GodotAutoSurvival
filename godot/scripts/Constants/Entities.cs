using System.Collections.Generic;
using Godot;

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
}

public static class Scenes
{
    public static PackedScene Projectile = (PackedScene) GD.Load("res://scenes/projectiles/projectile.tscn");
    public static PackedScene ProjectileZap = (PackedScene) GD.Load("res://scenes/projectiles/projectile_zap.tscn");
    public static PackedScene ProjectileFireball = (PackedScene) GD.Load("res://scenes/projectiles/projectile_fireball.tscn");
    public static PackedScene Player = (PackedScene) GD.Load("res://scenes/characters/player.tscn");
    public static PackedScene Enemy = (PackedScene) GD.Load("res://scenes/characters/enemy.tscn");
    public static PackedScene UiDamageNumber = (PackedScene) GD.Load("res://scenes/ui/damage_number_component.tscn");
    public static PackedScene VfxBurnExplosion = (PackedScene) GD.Load("res://scenes/vfx/vfx_burn_explosion.tscn");
}