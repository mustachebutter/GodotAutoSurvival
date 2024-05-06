using Godot;

[Tool]
public partial class WeaponComponent : Node2D
{
    private Timer _timer;

    public string ProjectileName = "Default";
    public Projectile Projectile;
	// private PackedScene _projectileScene = Scenes.ProjectileFireball;
	private PackedScene _projectileScene = Scenes.ProjectileZap;
    private Player _player;

    public override void _Ready()
    {
        base._Ready();
        _player = GetParent<Player>();
    }

    private void CreateProjectile()
	{
		Node2D closestTarget = Utils.FindClosestTarget(Position, _player.Area2D);

		if (closestTarget == null) return;

		// _projectile = (Fireball) _projectileScene.Instantiate();
		_projectile = (Zap) _projectileScene.Instantiate();
		_projectile.OnEnemyKilledEvent += HandleEnemyDead;
		AddChild(_projectile);
		// _player.FireProjectileAtTarget(closestTarget, _projectile, ProjectileTypes.Fireball);
		_player.FireProjectileAtTarget(closestTarget, _projectile, ProjectileTypes.Zap);
	}

    private void StartTimer(float seconds = 0.0f)
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