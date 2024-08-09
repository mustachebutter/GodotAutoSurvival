using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Lazerbeam : Beam
{
	public override void _Ready()
	{
		base._Ready();
	}

    public override void HandleProjectileEffect(Enemy hitEnemy)
    {
        base.HandleProjectileEffect(hitEnemy);
    }
}
