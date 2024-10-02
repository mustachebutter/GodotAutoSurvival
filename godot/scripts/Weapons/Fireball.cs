using Godot;

public partial class Fireball : Projectile
{
	public override void _Ready()
	{
		base._Ready();
		StatusEffect = new Burn(StatusEffectParsedData.GetData("Status_DOT_Burn"));
	}

	public override void HandleProjectileEffect(BaseCharacter source, Enemy enemy)
	{
		base.HandleProjectileEffect(source, enemy);
		enemy.StatusEffectComponent.ApplyEffectToCharacter(StatusEffect, source, enemy);
	}

}
