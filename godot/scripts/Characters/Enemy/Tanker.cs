using Godot.Collections;

public partial class Tanker : Enemy
{
	#region GODOT
	public override void _Ready()
	{
		base._Ready();
		AssignAnimationLibrary("Enemy_AnimationLibrary", SavedAnimationLibrary.EnemyAnimationLibrary);

		var overrideStats = new Dictionary<string, float>
		{ 
			{ "AttackRange", 150.0f },
			{ "Speed", 40.0f },
			{ "Health", 200.0f },
		};

		SetUpEnemy(overrideStats, out float attackRange);

		StartAttackTimer();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}
	#endregion
	
	#region ACTION
	public override void Attack()
	{
		// Swing and do hit detection
		base.Attack();
	}

    public void Ability_Charge()
    {
        // If the ability is on cool down then can't do it! 
        if (DetectedPlayer(Area2D, out Player player))
        {
            // Charge up
            // Dash towards target
            // If hit players then knock them back
            // Ability goes on cooldown
        }
    }

	public override void ResetAttack()
	{
		base.ResetAttack();

	}
	#endregion

	#region EVENT HANDLING
	#endregion

	#region HELPERS
	#endregion

	#region CLEANUP
	#endregion

}
