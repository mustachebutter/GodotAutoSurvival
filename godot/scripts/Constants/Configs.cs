using System;

public enum EnemySpawnMode
{
    Dummy,
    Normal,
}
public static class GlobalConfigs
{
    public static int MAX_CHARACTER_LEVEL = 100;
    public static int MAX_STAT_LEVEL = 30;
    public static int MAX_WEAPON_LEVEL = 12;
    
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
    public static readonly string[] WEAPON_STATS = { "WStat_Damage", "WStat_AttackSpeed", "WStat_Speed" };
}