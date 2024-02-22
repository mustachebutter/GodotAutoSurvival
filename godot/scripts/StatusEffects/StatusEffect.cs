using System;
using Godot;

public class StatusEffect
{
    public BaseCharacter Target { get; set; }
    public Timer MainTimer { get; private set; }
    public Node2D Source { get; protected set; }
    public string StatusEffectId { get; protected set; } = "";
    public string StatusEffectName { get; protected set; } = "";
    public string StatusEffectDesc { get; protected set; } = "";
    public string VisualEffectName { get; protected set; } = "default";

    public bool IsStackable { get; protected set; } = false;
    public int NumberOfStacks { get; protected set; } = 0;

    public float Duration { get; protected set; } = 0.0f;

    public virtual void StartStatusEffect (BaseCharacter target) { }
    public virtual void HandleStatusEffect () { }
    public virtual void OnStatusEffectEnd () { }

    //Entry point for Status Effect, which is invoked in Projectile or any source of damage.

    public void StartMainTimer()
    {
        // This is the main timer for the buff/debuff
        MainTimer = Utils.CreateTimer(Target, OnStatusEffectEnd, Duration, true);
        MainTimer?.Start();
    }
}