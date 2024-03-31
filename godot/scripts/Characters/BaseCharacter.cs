using System.Collections.Generic;
using Godot;

public partial class BaseCharacter : CharacterBody2D
{
	[ExportGroup("Stats")]
	[Export(PropertyHint.Range, "0, 1000, 1")]
	public float Health { get; private set;} = 100.0f;
	[Export(PropertyHint.Range, "0, 3, 0.1")]
	public float AttackSpeed { get; set; } = 0.5f;
	[Export(PropertyHint.Range, "0, 1000, 1")]
	public float AttackRange { get; private set; } = 300.0f;
	[Export(PropertyHint.Range, "0, 1000, 1")]
	public float Speed { get; private set; } = 100.0f;

	private float _textOffset = 0.0f;
	private bool _isDead = false;
	public delegate void OnCharacterDeadHandler();
	public event OnCharacterDeadHandler OnCharacterDeadEvent;

	public DamageNumberComponent DamageNumberComponent { get; private set; }
	public StatusEffectComponent StatusEffectComponent { get; private set; }
	public VisualEffectComponent VisualEffectComponent { get; private set; }

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

		StatusEffectComponent = GetNode<StatusEffectComponent>("StatusEffectComponent");
		// damageNumberComponent = GetNode<DamageNumberComponent>("DamageNumberComponent");
		VisualEffectComponent = GetNode<VisualEffectComponent>("VisualEffectComponent");

		StatusEffectComponent.Target = this;
	}

	public void DealDamageToCharacter(float damage = 0.0f, DamageTypes damageType = DamageTypes.Normal)
	{
		if (damage > 0)
		{
			Health -= damage;
			DamageNumberComponent = (DamageNumberComponent) Scenes.UiDamageNumber.Instantiate();
			AddChild(DamageNumberComponent);

			DamageNumberComponent.Translate(new Vector2(_textOffset, 0.0f));
			_textOffset += 5.0f;

			DamageNumberComponent.UpdateText(damage.ToString(), damageType);
		}
		
		if(IsDead)
		{
			OnCharacterDeadEvent.Invoke();
			QueueFree();
		}
	}
}
