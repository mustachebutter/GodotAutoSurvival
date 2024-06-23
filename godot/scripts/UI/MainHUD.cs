using Godot;
using System;

public partial class MainHUD : CanvasLayer
{
	public Label Debug_CurrentWeapon;
	public RichTextLabel Debug_CurrentWeaponDetails;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Debug_CurrentWeapon = GetNode<Label>("Control/VBoxContainer/Label");
		Debug_CurrentWeaponDetails = GetNode<RichTextLabel>("Control/VBoxContainer/RichTextLabel");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetDebugWeapon(ProjectileData projectileData)
	{
		if (projectileData.ProjectileName != null || projectileData.ProjectileName != "")
			Debug_CurrentWeapon.Text = projectileData.ProjectileName;
		
		string weaponDetails = "";
		weaponDetails += $"[b][color=red]Damage[/color][/b]: {projectileData.Damage}\n";
		weaponDetails += $"[b][color=blue]Attack Speed[/color][/b]: {projectileData.Speed}\n";
		Debug_CurrentWeaponDetails.Text = weaponDetails;
	}
}
