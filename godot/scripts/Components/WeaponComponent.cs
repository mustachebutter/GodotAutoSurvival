using System.Collections.Generic;
using Godot;

[Tool]
public partial class WeaponComponent : Node2D
{
    private Timer _timer;
    public Projectile Projectile;
    private Player _player;
	private Dictionary<string, ProjectileData> projectileData = new Dictionary<string, ProjectileData>();
	string currentProjectile = "Weapon_Fireball";

    public override void _Ready()
    {
        base._Ready();
        _player = GetParent<Player>();
		projectileData = ProjectileParsedData.GetAllData();
    }

    private void CreateProjectile()
	{
		Node2D closestTarget = Utils.FindClosestTarget(_player.Position, _player.Area2D);

		if (closestTarget == null) return;

		Projectile = currentProjectile switch
		{
			"Weapon_Zap" => (Zap) projectileData[currentProjectile].ProjectileScene.Instantiate(),
			"Weapon_Fireball" => (Fireball) projectileData[currentProjectile].ProjectileScene.Instantiate(),
			_ => (Projectile) projectileData[currentProjectile].ProjectileScene.Instantiate()
		};

		Projectile.ProjectileData = projectileData[currentProjectile];
		GD.Print(closestTarget.Name);
		AddChild(Projectile);
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

	private void OnTimerTimeout()
	{
		CreateProjectile();
	}


}