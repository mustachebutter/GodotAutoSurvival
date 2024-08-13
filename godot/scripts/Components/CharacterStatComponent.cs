using Godot;

public partial class CharacterStatComponent : Node2D
{
    [ExportGroup("Offense")]
    [Export(PropertyHint.Range, "0, 999, 1")]
	public float Attack { get; set;} = 100.0f;
	[Export(PropertyHint.Range, "0, 999, 1")]
	public float AttackRange { get; set; } = 500.0f;
	[Export(PropertyHint.Range, "0, 2, 0.1")]
	public float AttackSpeed { get; set; } = 1.0f;

    [ExportGroup("Defense")]
	[Export(PropertyHint.Range, "0, 9999, 1")]
	public float Health { get; set;} = 0.0f;
	[Export(PropertyHint.Range, "0, 99, 1")]
	public float Defense { get; set; } = 50.0f;
	[Export(PropertyHint.Range, "0, 99, 1")]
	public float ElementalResistance { get; set; } = 50.0f;

    [ExportGroup("Utilities")]
	[Export(PropertyHint.Range, "0, 200, 1")]
	public float Speed { get; set; } = 100.0f;
    [Export(PropertyHint.Range, "0, 100, 1")]
	public float Crit { get; set; } = 1.0f;
	[Export(PropertyHint.Range, "0, 999, 1")]
	public float CritDamage { get; set; } = 100.0f;

    //TODO: Handling stats level
    //TODO: Get stats from file
}