using Godot;

public partial class Enemy : CharacterBody2D
{
	[Export]
	private float _health = 20.0f;
	private Label _label;

	public float Health 
	{
		get { return _health; }
		set { _health = value; }
	}

	public bool IsDead = false;

	public override void _Ready()
	{
		_label = GetNode<Label>("Label");
		_label.Text = Name;
	}

	public bool DealDamageTo(float damage = 0.0f)
	{
		if (Health > 0)
			Health -= damage;
		
		if (Health <= 0)
			IsDead = true;

		return IsDead;
	}
}
