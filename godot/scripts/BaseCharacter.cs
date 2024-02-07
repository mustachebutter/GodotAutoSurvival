using System.Collections.Generic;
using Godot;

public partial class BaseCharacter : CharacterBody2D
{
    [ExportGroup("Stats")]
    [Export(PropertyHint.Range, "0, 1000, 1")]
    private float _health = 20.0f;
    [Export(PropertyHint.Range, "0, 3, 0.1")]
    private float _attackSpeed = .5f;
    [Export(PropertyHint.Range, "0, 1000, 1")]
	private float _attackRange = 300.0f;
    [Export(PropertyHint.Range, "0, 1000, 1")]
	private float _speed = 100.0f;

    private bool _isDead = false;
    private List<StatusEffect> _statusEffectList = new List<StatusEffect>();


	public Area2D Area2D;


    public float Health { get; set;}
    public float AttackSpeed { get; set; }
    public float AttackRange { get; set; }
    public float Speed { get; set; }

    public bool IsDead
    {
        get
        {
            if (Health <= 0)
                return true;
            
            return false;
        }
        set { _isDead = value; }
    }

    public List<StatusEffect> StatusEffectList { get; set; }

    public override void _Ready()
    {
        base._Ready();
        Area2D = GetNode<Area2D>("Area2D");
		var circle = (CircleShape2D) Area2D.GetNode<CollisionShape2D>("CollisionShape2D").Shape;
		circle.Radius = AttackRange / 2;
    }

    public bool DealDamageToCharacter(float damage = 0.0f)
    {
        if (damage > 0)
			Health -= damage;

        return IsDead;
    }
}