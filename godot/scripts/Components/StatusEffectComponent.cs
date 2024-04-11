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
		currentStatusEffect.CreateMainTimer();
		// Should do custom logic here
		// eg. Stackable status
		if (StatusEffectList == null) return;

		// Currently there is no stackable status effect yet
		// So we're doing it this way
		// Find out if the status effect is already applied
		var status = StatusEffectList.Find(x => x.StatusEffectId == currentStatusEffect.StatusEffectId);

		// If there's already a status effect with that name in the list
		if (status != null)
		{
			GD.Print($"If - {status.StatusEffectName}");

			// if (IsStackable)
			// {
			//     status.NumberOfStacks++;
			// }

			// If not stackable then just refresh the status effect
			if (!status.IsStackable)
			{
				// Refresh the timer, I'm not sure if this is needed or not
				// currentStatusEffect.StartMainTimer();
			}
		}
		else
		{
			GD.Print($"Else - {currentStatusEffect.StatusEffectName}");
			StatusEffectList.Add(currentStatusEffect);
			Target.VisualEffectComponent.PlayVisualEffect(currentStatusEffect.VisualEffectName, true);
		}

		currentStatusEffect.StartStatusEffect(Target);
		// This is the main timer for the buff/debuff
		currentStatusEffect.StartMainTimer();

	}

	public void ClearEffect(StatusEffect currentStatusEffect)
	{
		// IF there are more than 1 stack then slowly fall off OR expires all of them
		// TODO: Some cool design decision here, just gonna assume 1 stack all time for now.

		var status = StatusEffectList.Find(x => x.StatusEffectId == currentStatusEffect.StatusEffectId);

		if (status != null)
		{
			StatusEffectList.Remove(currentStatusEffect);
		}
	}

}
