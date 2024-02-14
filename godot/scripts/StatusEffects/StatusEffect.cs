using System;
using Godot;

public class StatusEffect
{
    private Node2D _source;
    protected Timer _mainTimer;
    protected BaseCharacter _target;
    
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
    public void ApplyEffect(BaseCharacter target)
    {
        _target = target;
        // Should do custom logic here
        // eg. Stackable status
        if (_target?.StatusEffectList == null) return;

        // Currently there is no stackable status effect yet
        // So we're doing it this way
        // Find out if the status effect is already applied
        var status = _target.StatusEffectList.Find(x => x.StatusEffectName == StatusEffectName);
        if (status != null)
        {
            // if (IsStackable)
            // {
            //     status.NumberOfStacks++;
            // }
        }
        else
        {
            _target.StatusEffectList.Add(this);
            StartStatusEffect(_target);
        }
        
        // This is the main timer for the buff/debuff
        _mainTimer = Utils.CreateTimer(_target, OnStatusEffectEnd, Duration, true);
        _mainTimer?.Start();
    }
}