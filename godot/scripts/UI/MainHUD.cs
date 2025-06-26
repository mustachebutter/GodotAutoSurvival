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
	public Button SpawnDummyButton;
	public CheckButton SpawnModeButton;
	public Dictionary<string, HBoxContainer> statContainerPair = new Dictionary<string, HBoxContainer>();
	public MobSpawnerComponent MobSpawner;
	public ProgressBar ExpBar;
	public Label LevelLabel;
	private List<Vector2> dummyPositions = new List<Vector2> 
	{ 
		new Vector2(371, 329), 
		new Vector2(435, 264), 
		new Vector2(455, 396),
		new Vector2(504, 312)
	};
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Debug_CurrentWeapon = GetNode<Label>("VBoxContainer/Label");
		Debug_CurrentWeaponDetails = GetNode<RichTextLabel>("VBoxContainer/RichTextLabel");
		StatsContainer = GetNode<VBoxContainer>("VBoxContainer2");
		SpawnDummyButton = GetNode<Button>("VBoxContainer3/SpawnDummyButton");
		SpawnModeButton = GetNode<CheckButton>("VBoxContainer3/SpawmModeButton");
		ExpBar = GetNode<ProgressBar>("VBoxContainer4/ProgressBar");
		LevelLabel = GetNode<Label>("VBoxContainer4/Label");

		SpawnDummyButton.Pressed += SpawnDummies;
		SpawnModeButton.Toggled += SwitchSpawnMode;

		MobSpawner = UtilGetter.GetMainMobSpawner();
		SpawnModeButton.ButtonPressed = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetDebugWeapon(WeaponData projectileData)
	{
		if (projectileData.WeaponName != null || projectileData.WeaponName != "")
			Debug_CurrentWeapon.Text = $"{projectileData.WeaponName} - Lv. {projectileData.WeaponDamageData.MainLevel}";
		
		string weaponDetails = "";
		weaponDetails += $"[b][color=red]Damage[/color][/b]: {projectileData.WeaponDamageData.Damage.Value}\n";
		weaponDetails += $"[b][color=blue]Attack Speed[/color][/b]: {projectileData.WeaponDamageData.AttackSpeed.Value}\n";
		weaponDetails += $"[b][color=green]Travel Speed[/color][/b]: {projectileData.WeaponDamageData.Speed.Value}\n";
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
	public void SetDebugStats(CharacterStatComponent characterStatComponent)
	{
		foreach (var stat in DataParser.STATS_MAPPER)
		{
			var (hBoxContainer, container, buttonUp, buttonDown) = CreateDebugStatContainer();
			var statValue = characterStatComponent.GetStatFromName(stat);
			(float baseValue, float modifierValue, float totalValue) = characterStatComponent.GetCompleteStatFromName(stat);

			if (statValue == null) return;

			// Needs to do this to distinguish the containers so that it can be used to check for the proper event to update itself
			container.Name = statValue.Name.Replace(" ", "_");

			characterStatComponent.OnAnyStatUpgraded += (UpgradableObject @object, float baseValue, float modifierValue, float totalValue) => 
			{
				if (container.Name == @object.Name)
					SetStatText(container, @object, baseValue, modifierValue, totalValue);
			};

			SetStatText(container, statValue, baseValue, modifierValue, totalValue);

			buttonUp.Pressed += () => {
				statValue.Upgrade(UpgradableObjectTypes.Stat, 1);
			};

			buttonDown.Pressed += () => {
				statValue.Downgrade(1);
			};

			StatsContainer.AddChild(hBoxContainer);
		}
	}

	private void SetStatText(RichTextLabel container, UpgradableObject @object, float baseValue, float modifierValue, float totalValue)
	{
		container.Text = $"[color=red]{@object.Name}[/color]: \n{@object.Value}([color=green]+{modifierValue}%[/color]) = {totalValue} (LVL {@object.Level})";;
	}

	public void SpawnDummies()
	{
		LoggingUtils.Debug("Spawn some dummies");
		// foreach (var position in dummyPositions)
		// {
		// 	var enemy = Utils.CreateDummy(position, Scenes.Enemy);
		// 	enemy.OnCharacterDeadEvent += enemy.DestroyCharacter;
		// 	enemy.CharacterStatComponent.AddStat("Health", 100.0f);
		// }

		if (GlobalConfigs.EnemySpawnMode.Equals(EnemySpawnMode.Dummy))
		{
			Random random = new Random();

			for (int i = 0; i < 0; i++)
			{
				int randomX = random.Next(0, 500);
				int randomY = random.Next(0, 500);
				var enemy = Utils.CreateDummy(new Vector2(randomX, randomY), Scenes.Grunt);
				enemy.OnCharacterDeadEvent += enemy.DestroyCharacter;
			}

			for (int i = 0; i < 2; i++)
			{
				int randomX = random.Next(0, 500);
				int randomY = random.Next(0, 500);
				var enemy = Utils.CreateDummy(new Vector2(randomX, randomY), Scenes.Tanker);
				enemy.OnCharacterDeadEvent += enemy.DestroyCharacter;
			}

		}
		else
		{
			MobSpawner.StartSpawningEnemies();
		}
	}

	private void SwitchSpawnMode(bool buttonPressed)
	{
		if (buttonPressed)
		{
			GlobalConfigs.EnemySpawnMode = EnemySpawnMode.Dummy;
			SpawnModeButton.Text = "MODE: DUMMY";
			MobSpawner.StopSpawningEnemies();
		}
		else
		{
			SpawnModeButton.Text = "MODE: SPAWN";
			GlobalConfigs.EnemySpawnMode = EnemySpawnMode.Normal;
		}
	}

	public void SetExperience(int currentExp, int previousLevelMax, int expToLevelUp)
	{
		float expValue = (float) (currentExp - previousLevelMax) / (expToLevelUp - previousLevelMax);
		LoggingUtils.Debug($"{currentExp} - {expToLevelUp}");
		ExpBar.Value = Math.Round((double) (expValue * 100), 2);
	}

	public void SetLevel(int level = 1)
	{
		LevelLabel.Text = $"LEVEL {level}";
	}

	public void SetUpAugmentHUD()
	{
		AugmentHUD augmentHUD = (AugmentHUD) Scenes.AugmentHud.Instantiate();
		AddChild(augmentHUD);
		augmentHUD.PopulateAugmentCard();
	}

	public void TurnOffAugmentHUD()
	{
		var children = GetChildren();
		foreach (var c in children)
		{
			if (c is AugmentHUD augmentHUD)
			{
				RemoveChild(augmentHUD);
			}
		}
	}
}
