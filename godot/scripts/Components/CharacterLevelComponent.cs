using System.Linq;
using Godot;

public partial class CharacterLevelComponent : Node2D
{
    public CharacterLevelData CurrentCharacterLevel { get; set; }
    public Area2D ExpSuctionArea2D { get; set; }
	public int Experience { get; set; }
    public override void _Ready()
    {
        base._Ready();
        var firstCharacterLevel = DataParser.GetCharacterLevelDatabase()[0].DeepCopy();
        CurrentCharacterLevel = firstCharacterLevel;
		Experience = 0;
        
        var mainHUD = UtilGetter.GetMainHUD();
        mainHUD.SetExperience(Experience, CurrentCharacterLevel.ExperienceToLevelUp);
        mainHUD.SetLevel(CurrentCharacterLevel.Level);

        ExpSuctionArea2D = GetNode<Area2D>("ExpSuctionArea2D");
        var circle = (CircleShape2D) GetNode<CollisionShape2D>("ExpSuctionArea2D/CollisionShape2D").Shape;
        circle.Radius = 150.0f;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        var bodies = ExpSuctionArea2D.GetOverlappingBodies();

        if (bodies != null)
        {
            foreach (var bd in bodies)
            {
                if (bd is ExperienceOrb experienceOrb)
                {
                    GainExperience(experienceOrb.ExperienceValue);
                    experienceOrb.QueueFree();
                }
            }
        }
    }

    public void GainExperience(int experience = 0)
	{
        LoggingUtils.Debug($"Gained EXP - {experience}");
		Experience += experience;

        LoggingUtils.Debug($"Current EXP - {Experience}");

		if (Experience >= CurrentCharacterLevel.ExperienceToLevelUp)
		{
			LevelUp();
		}

		UtilGetter.GetMainHUD().SetExperience(Experience, CurrentCharacterLevel.ExperienceToLevelUp);
	}

	public void LevelUp(int levelToUpgrade = 1)
	{
		var characterLevelDatabase = DataParser.GetCharacterLevelDatabase();
		int maxLevel = characterLevelDatabase.LastOrDefault().Level;

		if (CurrentCharacterLevel.Level + levelToUpgrade > maxLevel)
		{
			LoggingUtils.Error("#################################");
			LoggingUtils.Error("Max Level reached. Cannot level up anymore!");
			LoggingUtils.Error($"Current Level = {CurrentCharacterLevel.Level}, Level To Upgrade = {levelToUpgrade}");
			return;
		}

		CharacterLevelData nextLevel = characterLevelDatabase[CurrentCharacterLevel.Level + levelToUpgrade];
		CurrentCharacterLevel = nextLevel.DeepCopy();
		UtilGetter.GetMainHUD().SetLevel(CurrentCharacterLevel.Level);
	}
}