using System.Collections.Generic;
using Godot;

[Tool]
public partial class WeaponComponent : Node2D
{
    private Timer _timer;

    public string ProjectileName = "Default";
    public Projectile Projectile;
    private Player _player;
	private Dictionary<string, ProjectileData> projectileData = new Dictionary<string, ProjectileData>();
	string currentProjectile = "Weapon_Zap";

    public override void _Ready()
    {
        base._Ready();
        _player = GetParent<Player>();
		projectileData = ProjectileParsedData.GetAllData();
    }

    private void CreateProjectile()
	{
		Node2D closestTarget = Utils.FindClosestTarget(Position, _player.Area2D);

		if (closestTarget == null) return;

		// _projectile = (Fireball) _projectileScene.Instantiate();
		Projectile = currentProjectile switch
		{
			"Weapon_Zap" => (Zap) projectileData[currentProjectile].ProjectileScene.Instantiate(),
			"Weapon_Fireball" => (Fireball) projectileData[currentProjectile].ProjectileScene.Instantiate(),
			_ => (Projectile) projectileData[currentProjectile].ProjectileScene.Instantiate()
		};

		AddChild(Projectile);
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

	private void OnTimerTimeout()
	{
		CreateProjectile();
	}


}