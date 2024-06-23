using System.Collections.Generic;
using System.Linq;
using Godot;

[Tool]
public partial class WeaponComponent : Node2D
{
    private Timer _timer;
    public Projectile Projectile;
    private Player _player;
	private Dictionary<string, ProjectileData> projectileData = new Dictionary<string, ProjectileData>();
	private List<string> projectiles = new List<string>();
	int index = 0;
	MainHUD MainHUD { get; set; }

    public override void _Ready()
    {
        base._Ready();
        _player = GetParent<Player>();
		projectileData = ProjectileParsedData.GetAllData();
		projectiles = projectileData.Keys.Where(x => x != "Weapon_Default").ToList();
		MainHUD = UtilGetter.GetSceneTree().Root.GetNode<MainHUD>("Node2D/MainHUD");
		MainHUD.SetDebugWeapon(projectileData[projectiles[index]]);
    }

    private void CreateProjectile()
	{
		Node2D closestTarget = Utils.FindClosestTarget(_player.Position, _player.Area2D);

		if (closestTarget == null) return;

		string currentProjectile = projectiles[index];
		Projectile = currentProjectile switch
		{
			"Weapon_Zap" => (Zap) projectileData[currentProjectile].ProjectileScene.Instantiate(),
			"Weapon_Fireball" => (Fireball) projectileData[currentProjectile].ProjectileScene.Instantiate(),
			_ => (Projectile) projectileData[currentProjectile].ProjectileScene.Instantiate()
		};

		Projectile.ProjectileData = projectileData[currentProjectile];
		// Add the projectile to the main scene instead
		GetTree().Root.GetNode("Node2D").GetNode("ProjectileParentNode").AddChild(Projectile);
		Projectile.Position = _player.Position;
		_player.FireProjectileAtTarget(closestTarget, Projectile);
	}

    public void StartTimer(float seconds = 0.0f)
	{
		if (seconds > 0)
		{	
			CreateProjectile();
			_timer = Utils.CreateTimer(this, OnTimerTimeout, seconds, false);
			_timer?.Start();

		}
	}

	public void SwitchNextWeapon()
	{
		if (index < projectiles.Count() - 1)
		{
			index++;
		}
		else
		{
			index = 0;
		}

		MainHUD.SetDebugWeapon(projectileData[projectiles[index]]);
	}
	private void OnTimerTimeout()
	{
		CreateProjectile();
	}


}