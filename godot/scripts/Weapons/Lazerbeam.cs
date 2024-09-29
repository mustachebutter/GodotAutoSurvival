using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Lazerbeam : Beam
{
	public override void _Ready()
	{
		base._Ready();
		StatusEffect = new Focus(StatusEffectParsedData.GetData("Status_Focus"));
	}

	public override void HandleProjectileEffect(BaseCharacter source, Enemy hitEnemy)
	{
		base.HandleProjectileEffect(source, hitEnemy);
	}

	public override float CalculateTotalDamage()
	{
		var player = UtilGetter.GetMainPlayer();
		var focusStatus = (Focus) player.StatusEffectComponent.StatusEffectList.Find(x => x.StatusEffectData.StatusEffectId == "Status_Focus");
		var extraDamageMultiplier = focusStatus.ExtraDamageMultiplier;

		return base.CalculateTotalDamage() * (1 + extraDamageMultiplier);
	}
	public override void DealDamageToCharacter()
	{
		var player = UtilGetter.GetMainPlayer();
		player.StatusEffectComponent.ApplyEffectToCharacter(StatusEffect);

		base.DealDamageToCharacter();
	}
}
