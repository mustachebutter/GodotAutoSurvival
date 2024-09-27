using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class MainHUD : CanvasLayer
{
	public Label Debug_CurrentWeapon;
	public RichTextLabel Debug_CurrentWeaponDetails;
	public VBoxContainer StatsContainer;
	public Dictionary<string, HBoxContainer> statContainerPair = new Dictionary<string, HBoxContainer>();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Debug_CurrentWeapon = GetNode<Label>("Control/VBoxContainer/Label");
		Debug_CurrentWeaponDetails = GetNode<RichTextLabel>("Control/VBoxContainer/RichTextLabel");
		StatsContainer = GetNode<VBoxContainer>("Control/VBoxContainer2");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetDebugWeapon(WeaponData projectileData)
	{
		if (projectileData.WeaponName != null || projectileData.WeaponName != "")
			Debug_CurrentWeapon.Text = projectileData.WeaponName;
		
		string weaponDetails = "";
		weaponDetails += $"[b][color=red]Damage[/color][/b]: {projectileData.Damage}\n";
		weaponDetails += $"[b][color=blue]Attack Speed[/color][/b]: {projectileData.Speed}\n";
		Debug_CurrentWeaponDetails.Text = weaponDetails;
	}

	public (HBoxContainer, RichTextLabel, Button, Button) CreateDebugStatContainer()
	{
		HBoxContainer hBoxContainer = new HBoxContainer();
		Button levelUpBtn = new Button();
		levelUpBtn.CustomMinimumSize = new Vector2(30.0f, 30.0f);
		levelUpBtn.Text = "↑";

		Button levelDownBtn = new Button();
		levelDownBtn.CustomMinimumSize = new Vector2(30.0f, 30.0f);
		levelDownBtn.Text = "↓";

		RichTextLabel statFieldLabel = new RichTextLabel();
		statFieldLabel.BbcodeEnabled = true;
		statFieldLabel.AddThemeFontSizeOverride("theme_overrides_font_sizes/normal_font_size", 10);
		statFieldLabel.FitContent = true;
		statFieldLabel.AutowrapMode = TextServer.AutowrapMode.Off;
		statFieldLabel.SizeFlagsVertical = Control.SizeFlags.ShrinkCenter;

		hBoxContainer.AddChild(statFieldLabel);
		hBoxContainer.AddChild(levelUpBtn);
		hBoxContainer.AddChild(levelDownBtn);

		return (hBoxContainer, statFieldLabel, levelUpBtn, levelDownBtn);
	}
	public void SetDebugStats(CharacterStatData characterStatData, CharacterStatComponent characterStatComponent)
	{
		foreach (var stat in CharacterStatParsedData.STATS_MAPPER)
		{
			var (hBoxContainer, container, buttonUp, buttonDown) = CreateDebugStatContainer();
			var statValue = characterStatComponent.GetValueOfStat(stat);

			if (statValue == null) return;

			var finalStr = $"[color=red]{stat}[/color]: {statValue.Value} (LVL {statValue.Level})";
			container.Text = finalStr;

			buttonUp.Pressed += () => {
				characterStatComponent.UpgradeStatLevel(statValue, 1);
				var finalStr = $"[color=red]{stat}[/color]: {statValue.Value} (LVL {statValue.Level})";
				container.Text = finalStr;
			};

			StatsContainer.AddChild(hBoxContainer);
		}
	}
}
