using System;
using Godot;

public class StatusEffect : IDisposable
{
    public BaseCharacter Target { get; set; }
    public Timer MainTimer { get; private set; }
    public Node2D Source { get; protected set; }
    public StatusEffectData StatusEffectData { get; set; }
    public virtual void StartStatusEffect () { }
    public virtual void HandleStatusEffect () { }
    public virtual void OnStatusEffectEnd () 
    {
        Target.StatusEffectComponent.ClearEffect(this);
        Dispose();
        Target.VisualEffectComponent.ClearVisualEffect();
        Target = null;
        Utils.DestroyTimer(MainTimer);
    }

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

    public virtual void Dispose() { }
}