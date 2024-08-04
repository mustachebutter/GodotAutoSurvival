using Godot;

public partial class Weapon : CharacterBody2D
{
    public StatusEffect StatusEffect { get; protected set; }
    public WeaponData WeaponData { get; set;}
}