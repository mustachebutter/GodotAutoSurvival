using System;
using Godot;

public class Focus : StatusEffect
{
    //TODO: Build this into base StatusEffect
    private int _maxStacks = 5;
    private float _extraDamagePerStack = 0.05f;
    public float ExtraDamageMultiplier = 0.0f;
    public Focus(StatusEffectData statusEffectData)
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

        StatusEffectData = statusEffectData;
    }

    public override void StartStatusEffect()
    {
        base.StartStatusEffect();
        // TODO: This should be checked in the base StatusEffect
        var num = StatusEffectData.NumberOfStacks >= 5 ? 5 : (StatusEffectData.NumberOfStacks % 5);
        ExtraDamageMultiplier = num * _extraDamagePerStack;

        LoggingUtils.Debug($"Stacks: {num}");
        LoggingUtils.Debug($"ExtraDamageMultiplier: {ExtraDamageMultiplier}");
    }

    public override void HandleStatusEffect()
    {
        base.HandleStatusEffect();
    }

    public override void EndStatusEffect()
    {
        base.EndStatusEffect();
    }
}