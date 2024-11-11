using System;

public enum EnemySpawnMode
{
    Dummy,
    Normal,
}
public static class GlobalConfigs
{
    public static EnemySpawnMode EnemySpawnMode { get; set; } = EnemySpawnMode.Dummy;
    private static bool _isGamePaused = false;
    public static bool IsGamePaused 
    { 
        get => _isGamePaused;
        set
        {
            if (_isGamePaused != value)
            {
                _isGamePaused = value;
                OnGamePausedChanged?.Invoke(_isGamePaused);
            }
        }
    }

    public static event Action<bool> OnGamePausedChanged;

    public static readonly string[] STATS = { "Health", "Attack", "AttackRange", "AttackSpeed", "Speed", "Crit", "CritDamage", "Defense", "ElementalResistance", };

}