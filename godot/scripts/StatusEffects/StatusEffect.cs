using System;
using Godot;

public class StatusEffect
{
    private Node2D _source;
    public BaseCharacter Target { get; set; }
    public Timer MainTimer { get; private set; }
    public string StatusEffectId { get; private set; } = "";
    public string StatusEffectName { get; private set; } = "";
    public string StatusEffectDesc { get; private set; } = "";

    public bool IsStackable { get; private set; } = false;
    public int NumberOfStacks { get; private set; } = 0;

    public float Duration { get; private set; } = 0.0f;

    public StatusEffect(
        Node2D source,
        string statusEffectId,
        string statusEffectName,
        string statusEffectDesc,
        bool isStackable,
        int numberOfStacks,
        float duration
    )
    {
        _source = source;
        StatusEffectId = statusEffectId;
        StatusEffectName = statusEffectName;
        StatusEffectDesc = statusEffectDesc;
        IsStackable = isStackable;
        NumberOfStacks = numberOfStacks;
        Duration = duration;
    }
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