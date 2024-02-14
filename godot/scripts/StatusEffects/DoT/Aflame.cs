using Godot;

public class Aflame : DotStatusEffect
{
    public Aflame(
        Node2D source,
        string statusEffectId,
        string statusEffectName,
        string statusEffectDesc,
        bool isStackable,
        int numberOfStacks,
        float duration,
        float damage,
        float tickPerSec
    ) : base (
        source,
        statusEffectId,
        statusEffectName,
        statusEffectDesc,
        isStackable,
        numberOfStacks,
        duration,
        damage,
        tickPerSec
    )
    {    }

    public override void OnTargetDied()
    {
        base.OnTargetDied();
        // When the target died, spread to other closeby targets
    }

}