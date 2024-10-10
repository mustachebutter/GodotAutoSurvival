using System.Linq;
using Godot;

public partial class CharacterLevelComponent : Node2D
{
    public CharacterLevelData CurrentCharacterLevel { get; set; }
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

    }

	public void GainExperience(int experience = 0)
	{
		Experience += experience;

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