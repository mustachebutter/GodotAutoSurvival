using Godot;

public partial class HUDController : Node
{
    private MainHUD _mainHUD;
    private DeadHUD _deadHUD;
    private Player _player;

    public MainHUD MainHUD
    {
        get => _mainHUD;
        private set
        {
            _mainHUD = value;
        }
    }

    public DeadHUD DeadHUD
    {
        get => _deadHUD;
        private set
        {
            _deadHUD = value;
        }
    }

    public override async void _Ready()
    {
        LoggingUtils.Debug("In HUD Controller");

        MainHUD = GetNode<MainHUD>("MainHUD");
        DeadHUD = GetNode<DeadHUD>("DeadHUD");

        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        _player = UtilGetter.GetMainPlayer();

        _mainHUD.SetDebugStats(_player.CharacterStatComponent);

        var characterLevelComponent = _player.CharacterLevelComponent;
        _mainHUD.SetExperience(
            characterLevelComponent.Experience,
            characterLevelComponent.PreviousLevelMax,
            characterLevelComponent.CurrentCharacterLevel.ExperienceToLevelUp
        );
        _mainHUD.SetLevel(characterLevelComponent.CurrentCharacterLevel.Level);

        var weaponComponent = _player.WeaponComponent;
        _mainHUD.SetDebugWeapon(weaponComponent.WeaponData[weaponComponent.Weapons[0]]);
        weaponComponent.OnChangingWeaponUpdateHUD += (int index) =>
        {
            _mainHUD.SetDebugWeapon(weaponComponent.WeaponData[weaponComponent.Weapons[index]]);
        };
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }
}