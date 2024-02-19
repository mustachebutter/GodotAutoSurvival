public partial class Fireball : Projectile
{
	public StatusEffect StatusEffect { get; private set; }
	public override void _Ready()
	{
		base._Ready();
		StatusEffect = new Burn(
			this,
			"Status_DOT_Burn", "Burn", "Burns the target, ticks damage every x seconds",
			false, 0, 1.0f,
			1.0f, DamageTypes.Fire, 0.1f
		);
	}

	public override void HandleProjectileEffect(Enemy enemy)
	{
		base.HandleProjectileEffect(enemy);
		enemy.ApplyEffectToCharacter(StatusEffect);
		
	}
}
