using Godot;

public partial class Enemy : BaseCharacter
{
	private Label _label;
	public override void _Ready()
	{
		base._Ready();
		_label = GetNode<Label>("Label2");
		_label.Text = Name;
	}
}
