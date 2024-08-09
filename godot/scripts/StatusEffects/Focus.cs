using System;
using Godot;

public class Focus : StatusEffect
{
    //TODO: Build this into base StatusEffect
    private int _maxStacks = 5;
    private float _extraDamagePerStack = 0.05f;
    public float ExtraDamageMultiplier = 0.0f;
    public Focus(Node2D source, StatusEffectData statusEffectData)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(statusEffectData);
        }
        catch (ArgumentNullException e)
        {
            LoggingUtils.Error($"{nameof(Focus)} Status effect data is null");
            throw;
        }
        Source = source;
        StatusEffectData = statusEffectData;
    }

    public override void StartStatusEffect()
    {
        base.StartStatusEffect();
        // TODO: This should be checked in the base StatusEffect
        var num = StatusEffectData.NumberOfStacks % 5 == 0 ? 5 : (StatusEffectData.NumberOfStacks % 5);
        var extraDamageMultiplier = num * _extraDamagePerStack;


    }

    public override void HandleStatusEffect()
    {
        base.HandleStatusEffect();
    }

    public override void OnStatusEffectEnd()
    {
        base.OnStatusEffectEnd();
    }
}