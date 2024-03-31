using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Godot;

public partial class VfxBurnExplosion : Node2D
{
    public AnimatedSprite2D AnimatedSprite2D;
    public Area2D Area2D;
    public CollisionShape2D CollisionShape2D;

    public override void _Ready()
    {
        base._Ready();
        AnimatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        Area2D = GetNode<Area2D>("Area2D");
        CollisionShape2D = Area2D.GetNode<CollisionShape2D>("CollisionShape2D");
    }

    public List<Enemy> ScanForEnemies(BaseCharacter ignoredCharacter)
    {
        List<Enemy> enemies = new List<Enemy>();
        var overlappedNodes = Area2D.GetOverlappingBodies();
        if(overlappedNodes == null)
        {
            GD.Print("Found no enemies close by");
            return enemies;
        }

        foreach (var node in overlappedNodes)
        {
            if (node != ignoredCharacter)
            {
                enemies.Add((Enemy) node);
                GD.Print($"Found enemy - {node.Name}");
            }
        }

        return enemies;
    }

    public void PlayVisualEffects()
    {
        AnimatedSprite2D.Play();
    }

    

}