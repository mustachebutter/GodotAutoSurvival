using Godot;

public partial class Weapon : CharacterBody2D
{
    public StatusEffect StatusEffect { get; protected set; }
    public WeaponData WeaponData { get; set;}
    public virtual void HandleProjectileEffect() { }
	public virtual void HandleProjectileEffect(Enemy hitEnemy) { }

}