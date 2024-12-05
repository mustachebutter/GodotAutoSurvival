public partial class Grunt : Enemy
{
    public override void _Ready()
    {
        base._Ready();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        DetectPlayer();
    }

    public void DetectPlayer()
    {
        var bodies = Area2D.GetOverlappingBodies();
        if (bodies == null)
        {
            LoggingUtils.Info("No bodies scanned with Area2D");
            return;
        }

        foreach (var bd in bodies)
        {
            if (bd is Player)
            {
                LoggingUtils.Debug("Found player");
            }
        }
    }
}