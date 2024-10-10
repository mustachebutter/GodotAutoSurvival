using System.Collections.Generic;
using System.Linq;
using Godot;

[Tool]
public partial class WeaponComponent : Node2D
{
	private Timer _timer;
	public Weapon Weapon;
	private Player _player;
	private Dictionary<string, WeaponData> weaponData = new Dictionary<string, WeaponData>();
	private List<string> weapons = new List<string>();
	int index = 0;
	MainHUD MainHUD { get; set; }

	public override void _Ready()
	{
		base._Ready();
		_player = GetParent<Player>();
		weaponData = DataParser.GetAllData();
		weapons = weaponData.Keys.Where(x => x != "Weapon_Default").ToList();
		MainHUD = UtilGetter.GetSceneTree().Root.GetNode<MainHUD>("MotherNode/MainHUD");
		MainHUD.SetDebugWeapon(weaponData[weapons[index]]);
	}

	public void StartTimer(float seconds = 0.0f)
	{
		if (seconds > 0)
		{	
			StartWeapon();
			_timer = Utils.CreateTimer(this, OnTimerTimeout, seconds, false);
			_timer?.Start();

		}
	}

	private void StartWeapon()
	{
		Node2D closestTarget = Utils.FindClosestTarget(_player.Position, _player.Area2D);

		if (closestTarget == null) return;

		string currentWeapon = weapons[index];
		Weapon = currentWeapon switch
		{
			"Weapon_Zap" => (Zap) weaponData[currentWeapon].ProjectileScene.Instantiate(),
			"Weapon_Fireball" => (Fireball) weaponData[currentWeapon].ProjectileScene.Instantiate(),
			"Weapon_Lazer" => (Lazerbeam) weaponData[currentWeapon].ProjectileScene.Instantiate(),
			_ => (Weapon) weaponData[currentWeapon].ProjectileScene.Instantiate()
		};

		Weapon.WeaponData = weaponData[currentWeapon];

		// Add the projectile to the main scene instead
		if (Weapon.WeaponData.WeaponType == WeaponTypes.Projectile)
			GetTree().Root.GetNode("MotherNode").GetNode("ProjectileParentNode").AddChild(Weapon);
		else
			_player.AddChild(Weapon);

		Weapon.GlobalPosition = _player.GlobalPosition;

		_player.FireProjectileAtTarget(closestTarget, Weapon);
	}


	public void SwitchNextWeapon()
	{
		if (index < weapons.Count() - 1)
		{
			index++;
		}
		else
		{
			index = 0;
		}

		MainHUD.SetDebugWeapon(weaponData[weapons[index]]);
	}
	private void OnTimerTimeout()
	{
		StartWeapon();
	}


}
