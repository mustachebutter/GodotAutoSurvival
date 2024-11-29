using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class UtilGetter
{
	public static Node2D GetMotherNode()
	{
		var sceneTree = (SceneTree) Engine.GetMainLoop() ?? throw new InvalidOperationException($"ERROR [{nameof(UtilGetter)}] Could not get the Scene Tree");
		var motherNode = sceneTree.Root.GetNode<Node2D>("MotherNode");

		return motherNode;
	}

	public static Node2D GetCharactersParentNode() { return GetMotherNode().GetNode<Node2D>("CharactersParentNode"); }
	public static Node2D GetVfxParentNode() { return GetMotherNode().GetNode<Node2D>("VFXParentNode"); }
	public static Node2D GetProjectileParentNode() { return GetMotherNode().GetNode<Node2D>("ProjectileParentNode"); }
	public static MainHUD GetMainHUD() { return GetMotherNode().GetNode<MainHUD>("MainHUD"); }

	public static Player GetMainPlayer()
	{
		var player = GetCharactersParentNode().GetNode<CharacterBody2D>("Player");
		
		if (player != null)
		{
			return (Player) player;
		}

		LoggingUtils.Error("Could not retrieved player node");
		throw new Exception("Could not retrieved player node");
	}

	public static MobSpawnerComponent GetMainMobSpawner()
	{
		var mobSpawner = GetMotherNode().GetNode<MobSpawnerComponent>("MobSpawnerComponent");

		if (mobSpawner != null)
		{
			return (MobSpawnerComponent) mobSpawner;
		}

		LoggingUtils.Error("Could not retrieved mob spawner node");
		throw new Exception("Could not retrieved mob spawner node");
	}

	public static bool GetPaused()
	{
		var sceneTree = (SceneTree) Engine.GetMainLoop() ?? throw new InvalidOperationException($"ERROR [{nameof(UtilGetter)}] Could not get the Scene Tree");
		return sceneTree.Paused;
	}
}

public enum DamageTypes { Fire, Electric, Normal, Light, }
public enum WeaponTypes { Projectile, Beam, }
public enum UpgradableObjectTypes { Stat, Weapon, StatusEffect, };
public enum AugmentType { Stat, Weapon, Item };
public enum CardRarity { Common, Rare, Epic, Legendary, Mythic }
public enum StatTypes { Stat, Modifier };



public static class Scenes
{
	public static PackedScene Player = (PackedScene) GD.Load("res://scenes/characters/player.tscn");
	public static PackedScene Enemy = (PackedScene) GD.Load("res://scenes/characters/enemy.tscn");
	public static PackedScene UiDamageNumber = (PackedScene) GD.Load("res://scenes/ui/damage_number_component.tscn");
	public static PackedScene VfxBurnExplosion = (PackedScene) GD.Load("res://scenes/vfx/vfx_burn_explosion.tscn");
	public static PackedScene VfxChainLightning = (PackedScene) GD.Load("res://scenes/vfx/vfx_chain_lightning.tscn");
	public static PackedScene ExperienceOrb = (PackedScene) GD.Load("res://scenes/experience_orb.tscn");
	public static PackedScene AugmentHud = (PackedScene) GD.Load("res://scenes/ui/augment_hud.tscn");
	public static PackedScene AugmentCard = (PackedScene) GD.Load("res://scenes/ui/augment_card.tscn");

}
