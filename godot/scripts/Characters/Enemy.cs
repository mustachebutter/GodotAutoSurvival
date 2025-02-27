using Godot;

public partial class Enemy : BaseCharacter
{
	private Label _label;
	private Player _player;
	public override void _Ready()
	{
		base._Ready();
		_label = GetNode<Label>("Label2");
		_label.Text = Name;
		_player = UtilGetter.GetMainPlayer();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (GlobalConfigs.EnemySpawnMode.Equals(EnemySpawnMode.Normal))
			MoveTowardsThePlayer();
	}

	public void MoveTowardsThePlayer()
	{
		Vector2 direction = _player.GlobalPosition - GlobalPosition;
		direction = direction.Normalized();
		var speed = CharacterStatComponent.GetStatFromName("Speed").Value;

		Velocity = direction * speed;
		MoveAndSlide();
	}

}
