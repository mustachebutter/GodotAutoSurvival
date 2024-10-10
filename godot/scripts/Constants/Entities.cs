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
