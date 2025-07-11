using System;
using System.Collections.Generic;
using Godot;

public partial class BaseCharacter : CharacterBody2D
{
	public Area2D Area2D;
	protected CircleShape2D _attackRangeCircle;
	private Godot.Vector2 _textOffset = new Godot.Vector2(0.0f, 0.0f);
	private Label _healthLabel;
	private bool _isDead = false;
	public delegate void OnCharacterDeadHandler();
	public event OnCharacterDeadHandler OnCharacterDeadEvent;
	public string AnimationLibraryName = "";
	public AnimationPlayer AnimationPlayer { get; set; }

	public CharacterStatComponent CharacterStatComponent { get; private set; }
	public DamageNumberComponent DamageNumberComponent { get; private set; }
	public StatusEffectComponent StatusEffectComponent { get; private set; }
	public VisualEffectComponent VisualEffectComponent { get; private set; }
	protected BTNode _animationBehaviorTree { get; set; }
	
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

		//NOTE: This is done here for all characters but Enemies have overriden stats 
		// so it needs to be set again in SetUpEnemy()
		_attackRangeCircle = (CircleShape2D)Area2D.GetNode<CollisionShape2D>("CollisionShape2D").Shape;
		_attackRangeCircle.ResourceLocalToScene = true;
		_attackRangeCircle.Radius = CharacterStatComponent.GetCompleteStatFromName("AttackRange").totalValue / 2;

		_healthLabel = GetNode<Label>("HealthLabel");
		
		AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		AnimationPlayer.AnimationFinished += OnAnimationFinished;
	}

	public string GetAnimation(string name)
	{
		return $"{AnimationLibraryName}/{name}";
	}

	public void AssignAnimationLibrary(string name, AnimationLibrary animationLibrary)
	{
		AnimationLibraryName = name;
		var animationList = AnimationPlayer.GetAnimationLibraryList();
		if (animationList.Count == 0)
		{
			AnimationPlayer.AddAnimationLibrary(name, animationLibrary);
		}
	}

	protected BTNode CreatePlayAnimationNode(AnimationPlayer animationPlayer, string name)
	{
		BTNode playAnimationNode = new ActionNode((float delta) =>
		{
			animationPlayer.Play(GetAnimation(name));
			return BTNodeState.Success;
		});

		return playAnimationNode;
	}
	
	protected BTNode CreateDebugNode(BTNode child, string message)
	{
		BTNode debugNode = new SequenceNode(new List<BTNode>()
		{
			new ActionNode((_) => {
				LoggingUtils.Debug(message);
				return BTNodeState.Success;
			}),
			child,
		});

		return debugNode;
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
	}

	protected virtual void OnAnimationFinished(StringName anim_name)
	{
		if (anim_name == GetAnimation("die"))
		{
			Perish();
		}
	}

	public virtual void Perish()
	{
		LoggingUtils.Error("Character dying :(");
		OnCharacterDeadEvent?.Invoke();
	}

}
