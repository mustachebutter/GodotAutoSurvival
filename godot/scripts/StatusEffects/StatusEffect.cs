using System;
using Godot;

public class StatusEffect
{
    public BaseCharacter Target { get; set; }
    public Timer MainTimer { get; private set; }
    public BaseCharacter SourceCharacter { get; set; }
    public StatusEffectData StatusEffectData { get; protected set; }
    public virtual void StartStatusEffect () 
    {
		if (Target.IsDead)
		{
			OnTargetDied();
			return;
		}
    }
    public virtual void HandleStatusEffect () { }
    public virtual void OnStatusEffectEnd () 
    {
        Target.StatusEffectComponent.ClearEffect(this);
        Target.VisualEffectComponent.ClearVisualEffect();
        Utils.DestroyTimer(MainTimer);
    }

    public virtual void OnTargetDied() { }

    //Entry point for Status Effect, which is invoked in Projectile or any source of damage.

    public void CreateMainTimer()
    {
        // This is the main timer for the buff/debuff
        MainTimer = Utils.CreateTimer(Target, OnStatusEffectEnd, StatusEffectData.Duration, true);
    }

    public void StartMainTimer()
    {
        try
        {
            ArgumentNullException.ThrowIfNull(MainTimer);
        }
        catch (ArgumentNullException e)
        {
            LoggingUtils.Error("Main Timer is not created! Please create one before starting");
            throw;
        }   
        
        MainTimer?.Start();
    }
}