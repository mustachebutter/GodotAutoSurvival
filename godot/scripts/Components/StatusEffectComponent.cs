using System.Collections.Generic;
using Godot;

[Tool]
public partial class StatusEffectComponent : Node2D
{
	public BaseCharacter Target { get; set; }
	public List<StatusEffect> StatusEffectList { get; private set; } = new List<StatusEffect>();

	public void ApplyEffectToCharacter(StatusEffect currentStatusEffect)
	{
		currentStatusEffect.Target = Target;
		if (currentStatusEffect.MainTimer == null)
			currentStatusEffect.CreateMainTimer();
		// Should do custom logic here
		// eg. Stackable status
		if (StatusEffectList == null) return;

		// Currently there is no stackable status effect yet
		// So we're doing it this way
		// Find out if the status effect is already applied
		var status = StatusEffectList.Find(x => x.StatusEffectData.StatusEffectId == currentStatusEffect.StatusEffectData.StatusEffectId);

		// If there's already a status effect with that name in the list
		if (status != null)
		{
			if (status.StatusEffectData.IsStackable)
			{
				status.StatusEffectData.NumberOfStacks++;
				// !!!IMPORTANT
				// If DOT DO NOT USE STACKABLE
				status.StartStatusEffect();
			}
		}
		else
		{
			StatusEffectList.Add(currentStatusEffect);
			Target.VisualEffectComponent.PlayVisualEffect(currentStatusEffect.StatusEffectData.VisualEffectName, true);
			// Do logic of the status effect. Only DOT has special logic for now.
			// Realistically, we want to do this once!
			currentStatusEffect.StartStatusEffect();
			status = currentStatusEffect;
		}

		// This is the main timer for the buff/debuff
		status.StartMainTimer();

	}

	public void ClearEffect(StatusEffect currentStatusEffect)
	{
		// IF there are more than 1 stack then slowly fall off OR expires all of them
		// TODO: Some cool design decision here, just gonna assume 1 stack all time for now.

		var status = StatusEffectList.Find(x => x.StatusEffectData.StatusEffectId == currentStatusEffect.StatusEffectData.StatusEffectId);

		if (status != null)
		{
			StatusEffectList.Remove(currentStatusEffect);
		}
	}

}
