using System;
using Godot;

public partial class BaseCharacter : CharacterBody2D
{
	public Area2D Area2D;
	protected CircleShape2D _circle;
	private Godot.Vector2 _textOffset = new Godot.Vector2(0.0f, 0.0f);
	private Label _healthLabel;
	private bool _isDead = false;
	private bool _hasTriggeredOnDead = false;
	public delegate void OnCharacterDeadHandler();
	public event OnCharacterDeadHandler OnCharacterDeadEvent;
	public string AnimationLibraryName = "";

	public CharacterStatComponent CharacterStatComponent { get; private set; }
	public DamageNumberComponent DamageNumberComponent { get; private set; }
	public StatusEffectComponent StatusEffectComponent { get; private set; }
	public VisualEffectComponent VisualEffectComponent { get; private set; }

	public bool IsDead
	{
		get
		{
			if (CharacterStatComponent.GetCompleteStatFromName("Health").totalValue <= 0)
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
		_circle = (CircleShape2D) Area2D.GetNode<CollisionShape2D>("CollisionShape2D").Shape;
		_circle.ResourceLocalToScene = true;
		_circle.Radius = CharacterStatComponent.GetCompleteStatFromName("AttackRange").totalValue / 2;

		_healthLabel = GetNode<Label>("HealthLabel");
	}

	protected string GetAnimation(string name)
	{
		return $"{AnimationLibraryName}/{name}";
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		_healthLabel.Text = $"{CharacterStatComponent.GetCompleteStatFromName("Health").totalValue}"; 
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
		LoggingUtils.Error("Destroying characters");
		Random random = new Random();

		var orb = (ExperienceOrb) Scenes.ExperienceOrb.Instantiate();
		orb.Position = Position + new Vector2(random.Next(1, 20), random.Next(1, 20));
		orb.Scale = new Vector2(0.1f, 0.1f);
		UtilGetter.GetMotherNode().AddChild(orb);

		QueueFree();
	}

}
