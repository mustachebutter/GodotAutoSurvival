#if TOOLS
using Godot;
using System;

[Tool]
public partial class GameComponentsPlugin : EditorPlugin
{
	public override void _EnterTree()
	{
		// Initialization of the plugin goes here.
		var script = GD.Load<Script>("res://scripts/Components/StatusEffectComponent.cs");
		var texture = GD.Load<Texture2D>("res://assets/icons/Icon_StatusEffect.png");
		AddCustomType("StatusEffectComponent", "Node2D", script, texture);

		script = GD.Load<Script>("res://scripts/Components/VisualEffectComponent.cs");
		texture = GD.Load<Texture2D>("res://assets/icons/Icon_VisualEffect.png");
		AddCustomType("VisualEffectComponent", "AnimatedSprite2D", script, texture);
		
		script = GD.Load<Script>("res://scripts/Components/WeaponComponent.cs");
		texture = GD.Load<Texture2D>("res://assets/icons/Icon_Weapon.png");
		AddCustomType("WeaponComponent", "Node2D", script, texture);

		script = GD.Load<Script>("res://scripts/Components/CharacterStatComponent.cs");
		texture = GD.Load<Texture2D>("res://assets/icons/Icon_Stat.png");
		AddCustomType("CharacterStatComponent", "Node2D", script, texture);

		script = GD.Load<Script>("res://scripts/Components/CharacterLevelComponent.cs");
		texture = GD.Load<Texture2D>("res://assets/icons/Icon_Level.png");
		AddCustomType("CharacterLevelComponent", "Node2D", script, texture);
	}

	public override void _ExitTree()
	{
		// Clean-up of the plugin goes here.
		RemoveCustomType("StatusEffectComponent");
		RemoveCustomType("VisualEffectComponent");
		RemoveCustomType("WeaponComponent");
		RemoveCustomType("CharacterStatComponent");
		RemoveCustomType("CharacterLevelComponent");
	}
}
#endif
