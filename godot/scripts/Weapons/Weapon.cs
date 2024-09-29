using Godot;

public partial class Weapon : CharacterBody2D
{
	public BaseCharacter SourceCharacter { get; set; }
    public StatusEffect StatusEffect { get; protected set; }
    public WeaponData WeaponData { get; set;}
    public virtual void HandleProjectileEffect() { }
	public virtual void HandleProjectileEffect(BaseCharacter source, Enemy hitEnemy) 
    { 
        SourceCharacter = source;
    }
    
	public virtual float CalculateTotalDamage() {
        if (SourceCharacter == null)
        {
            LoggingUtils.Error("Source character for weapon is null, will have 0 character damage");
        }
        float characterDamage = SourceCharacter == null ? 
			0
			: SourceCharacter.CharacterStatComponent.CharacterStatData.Attack.Value;

		return WeaponData.Damage + characterDamage;
	}
}