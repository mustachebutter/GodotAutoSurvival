using Godot;

public partial class CharacterStatComponent : Node2D
{
    [ExportGroup("Stats")]
	[Export(PropertyHint.Range, "0, 1000, 1")]
	public float Health { get; set;} = 100.0f;
	[Export(PropertyHint.Range, "0, 3, 0.1")]
	public float AttackSpeed { get; set; } = 0.5f;
	[Export(PropertyHint.Range, "0, 1000, 1")]
	public float AttackRange { get; set; } = 300.0f;
	[Export(PropertyHint.Range, "0, 1000, 1")]
	public float Speed { get; set; } = 100.0f;
}