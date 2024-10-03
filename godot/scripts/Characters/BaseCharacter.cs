using System;
using System.Collections.Generic;
using System.Numerics;
using Godot;

public partial class BaseCharacter : CharacterBody2D
{
	private Godot.Vector2 _textOffset = new Godot.Vector2(0.0f, 0.0f);
	private Label _healthLabel;
	private bool _isDead = false;
	private bool _hasTriggeredOnDead = false;
	public delegate void OnCharacterDeadHandler();
	public event OnCharacterDeadHandler OnCharacterDeadEvent;

	public CharacterStatComponent CharacterStatComponent { get; private set; }
	public DamageNumberComponent DamageNumberComponent { get; private set; }
	public StatusEffectComponent StatusEffectComponent { get; private set; }
	public VisualEffectComponent VisualEffectComponent { get; private set; }

	public Area2D Area2D;

	public bool IsDead
	{
		get
		{
			if (CharacterStatComponent.CharacterStatData.Health.Value <= 0)
				return true;
			
			return false;
		}
		private set { _isDead = value; }
	}

	public override void _Ready()
	{
		base._Ready();
		StatusEffectComponent = GetNode<StatusEffectComponent>("StatusEffectComponent");
		VisualEffectComponent = GetNode<VisualEffectComponent>("VisualEffectComponent");
		CharacterStatComponent = GetNode<CharacterStatComponent>("CharacterStatComponent");

		Area2D = GetNode<Area2D>("Area2D");
		var circle = (CircleShape2D) Area2D.GetNode<CollisionShape2D>("CollisionShape2D").Shape;
		circle.Radius = CharacterStatComponent.CharacterStatData.AttackRange.Value / 2;

		_healthLabel = GetNode<Label>("HealthLabel");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		_healthLabel.Text = $"{CharacterStatComponent.CharacterStatData.Health.Value}"; 
	}

	public void DealDamageToCharacter(float damage = 0.0f, DamageTypes damageType = DamageTypes.Normal)
	{
		if (damage > 0)
		{			
			CharacterStatComponent.ReduceStat("Health", damage);
			DamageNumberComponent = (DamageNumberComponent) Scenes.UiDamageNumber.Instantiate();
			AddChild(DamageNumberComponent);

			DamageNumberComponent.OffsetText(_textOffset);
			_textOffset.X += 2.0f;

			DamageNumberComponent.UpdateText(damage.ToString(), damageType);
		}
		
		CharacterBelowZeroHealth();
	}

	public void CharacterBelowZeroHealth()
	{
		if(IsDead && !_hasTriggeredOnDead)
		{
			OnCharacterDeadEvent?.Invoke();
			_hasTriggeredOnDead = true;
		}

	}

	public void DestroyCharacter()
	{
		QueueFree();
	}

}
