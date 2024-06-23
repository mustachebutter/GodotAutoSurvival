#if TOOLS
using Godot;
using System;

[Tool]
public partial class UiPlugin : EditorPlugin
{
	public override void _EnterTree()
	{
		// Initialization of the plugin goes here.
		var script = GD.Load<Script>("res://scripts/Components/DamageNumberComponent.cs");
		var texture = GD.Load<Texture2D>("res://assets/icons/Icon_HP.png");
		AddCustomType("DamageNumberComponent", "Node2D", script, texture);
	}

	public override void _ExitTree()
	{
		// Clean-up of the plugin goes here.
		RemoveCustomType("DamageNumberComponent");
	}
}
#endif
