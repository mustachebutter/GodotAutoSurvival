using Godot;

public partial class Fireball : Projectile
{
	public StatusEffect StatusEffect { get; private set; }
	public override void _Ready()
	{
		base._Ready();
		StatusEffect = new Burn(this, StatusEffectParsedData.GetData("Status_DOT_Burn"));
	}

	public override void HandleProjectileEffect(Enemy enemy)
	{
		base.HandleProjectileEffect(enemy);
		enemy.StatusEffectComponent.ApplyEffectToCharacter(StatusEffect);
	}

}
