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
    public List<StatusEffect> StatusEffectList { get; private set; } = new List<StatusEffect>();
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

    public override void _Ready()
    {
        base._Ready();
        Area2D = GetNode<Area2D>("Area2D");
		var circle = (CircleShape2D) Area2D.GetNode<CollisionShape2D>("CollisionShape2D").Shape;
		circle.Radius = AttackRange / 2;

        damageNumberComponent = GetNode<DamageNumberComponent>("DamageNumberComponent");
    }

    public bool DealDamageToCharacter(float damage = 0.0f, DamageTypes damageType = DamageTypes.Normal)
    {
        if (damage > 0)
        {
			Health -= damage;
            damageNumberComponent.UpdateText(damage.ToString(), damageType);
        }
        
        return IsDead;
    }

    public void ApplyEffectToCharacter(StatusEffect effect)
    {
        effect.Target = this;
        // Should do custom logic here
        // eg. Stackable status
        if (StatusEffectList == null) return;

        // Currently there is no stackable status effect yet
        // So we're doing it this way
        // Find out if the status effect is already applied
        var status = StatusEffectList.Find(x => x.StatusEffectName == effect.StatusEffectName);
        if (status != null)
        {
            // if (IsStackable)
            // {
            //     status.NumberOfStacks++;
            // }
        }
        else
        {
            StatusEffectList.Add(effect);
            effect.StartStatusEffect(this);
        }

        GD.PrintErr(StatusEffectList[0]);
        
        // This is the main timer for the buff/debuff
        effect.StartMainTimer();
    }
}