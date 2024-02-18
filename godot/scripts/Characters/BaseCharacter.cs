using System.Collections.Generic;
using Godot;

public partial class BaseCharacter : CharacterBody2D
{
    [ExportGroup("Stats")]
    [Export(PropertyHint.Range, "0, 1000, 1")]
    public float Health { get; private set;} = 100.0f;
    [Export(PropertyHint.Range, "0, 3, 0.1")]
    public float AttackSpeed { get; private set; } = 0.5f;
    [Export(PropertyHint.Range, "0, 1000, 1")]
    public float AttackRange { get; private set; } = 300.0f;
    [Export(PropertyHint.Range, "0, 1000, 1")]
    public float Speed { get; private set; } = 100.0f;

    private bool _isDead = false;
    private List<StatusEffect> _statusEffectList = new List<StatusEffect>();
    private DamageNumberComponent damageNumberComponent;

	public Area2D Area2D;

    public bool IsDead
    {
        get
        {
            if (Health <= 0)
                return true;
            
            return false;
        }
        private set { _isDead = value; }
    }

    public List<StatusEffect> StatusEffectList { get; set; }

    public override void _Ready()
    {
        base._Ready();
        Area2D = GetNode<Area2D>("Area2D");
		var circle = (CircleShape2D) Area2D.GetNode<CollisionShape2D>("CollisionShape2D").Shape;
		circle.Radius = AttackRange / 2;

        damageNumberComponent = GetNode<DamageNumberComponent>("DamageNumberComponent");
    }

    public bool DealDamageToCharacter(float damage = 0.0f)
    {
        if (damage > 0)
        {
			Health -= damage;
            damageNumberComponent.UpdateText(damage.ToString());
        }
        
        return IsDead;
    }
}