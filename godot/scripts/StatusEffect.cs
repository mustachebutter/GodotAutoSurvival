using Godot;

public class StatusEffect
{
    private Node2D _source;
    
    public string StatusEffectId { get; private set; } = "";
    public string StatusEffectName { get; private set; } = "";
    public string StatusEffectDesc { get; private set; } = "";

    public bool IsStackable { get; private set; } = false;
    public int NumberOfStacks { get; private set; } = 0;
    StatusEffect(Node2D source)
    {
        _source = source;
    }

    public void ApplyEffect(BaseCharacter target)
    {
        // Should do custom logic here
        // eg. Stackable status
        if (target?.StatusEffectList == null) return;

        // Currently there is no stackable status effect yet
        // So we're doing it this way
        // Find out if the status effect is already applied
        var status = target.StatusEffectList.Find(x => x.StatusEffectName == StatusEffectName);
        if (status != null)
        {
            if (IsStackable)
            {
                status.NumberOfStacks++;
            }
        }
        else
        {
            target.StatusEffectList.Add(this);
        }
    }
}