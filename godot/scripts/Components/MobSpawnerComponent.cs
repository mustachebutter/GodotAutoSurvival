using System;
using Godot;

public partial class MobSpawnerComponent : Node2D
{
    private Timer _timer;
    public override void _Ready()
    {
        base._Ready();
        _timer = Utils.CreateTimer(this, SpawnEnemies, 5.0f, false);
    }

    public void StartSpawningEnemies()
    {
        _timer?.Start();
    }
    
    public void StopSpawningEnemies()
    {
        _timer?.Stop();
    }

    public void SpawnEnemies()
    {
        Main mainNode = (Main) GetParent();
        for (int i = 0; i < 10; i++)
        {
            Vector2 randomSpawnPosition = mainNode.GetRandomOutOfViewportPosition();
            Utils.CreateDummy(randomSpawnPosition, Scenes.Grunt);
        }

        for (int i = 0; i < 2; i++)
        {
            Vector2 randomSpawnPosition = mainNode.GetRandomOutOfViewportPosition();
            Utils.CreateDummy(randomSpawnPosition, Scenes.Tanker);
        }

    }
}