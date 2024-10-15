public enum EnemySpawnMode
{
    Dummy,
    Normal,
}
public static class GlobalConfigs
{
    public static EnemySpawnMode EnemySpawnMode { get; set; } = EnemySpawnMode.Dummy;
    public static bool IsGamePaused { get; set; } = false;
}