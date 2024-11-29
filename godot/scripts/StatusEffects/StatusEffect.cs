using System;
using Godot;

public class StatusEffect
{
    public BaseCharacter Target { get; set; }
    public Timer MainTimer { get; private set; }
    public BaseCharacter SourceCharacter { get; set; }
    public StatusEffectData StatusEffectData { get; protected set; }
    // Multiplier for status effect damage
    public const float STATUS_EFFECT_DAMAGE_MULTIPLIER = 0.1f;

    // ######################################################
    // CONSTRUCTOR
    // ######################################################
    #region CONSTRUCTOR

    #endregion

    // ######################################################
    // STATUS EFFECT TIMELINE
    // ######################################################
    #region STATUS EFFECT TIMELINE
    public virtual void StartStatusEffect ()
    {
        Target.OnCharacterDeadEvent -= OnTargetDied;
		Target.OnCharacterDeadEvent += OnTargetDied;
    }
    public virtual void HandleStatusEffect () { }
    public virtual void EndStatusEffect () 
    {
        Target.StatusEffectComponent.ClearEffect(this);
        Target.VisualEffectComponent.ClearVisualEffect();
        Utils.DestroyTimer(MainTimer);
    }
    #endregion
    
    // ######################################################
    // EVENT HANDLING
    // ######################################################
    #region EVENT HANDLING
    public virtual void OnTargetDied() { }
    #endregion

    // ######################################################
    // HELPER METHODS
    // ######################################################
    #region HELPER METHODS
	public virtual float CalculateTotalDamage()
	{
		if (SourceCharacter == null)
		{
			LoggingUtils.Error("Source character for status effect is null, will have 0 character damage");
		}

		float characterDamage = SourceCharacter == null ? 
			0
			: SourceCharacter.CharacterStatComponent.GetCompleteStatFromName("Attack").totalValue;

		return (float) Math.Round(
			StatusEffectData.Damage + (characterDamage * STATUS_EFFECT_DAMAGE_MULTIPLIER),
			2
		);
	}

    public void CreateMainTimer()
    {
        // This is the main timer for the buff/debuff
        MainTimer = Utils.CreateTimer(Target, EndStatusEffect, StatusEffectData.Duration, true);
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
    #endregion

    // ######################################################
    // DECONSTRUCTOR/ CLEAN UP
    // ######################################################
    #region DECONSTRUCTOR/ CLEAN UP
    #endregion
}