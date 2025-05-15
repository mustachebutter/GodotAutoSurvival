using System;
public enum EnemyState
{
    Idle,
    Charging,
    ChargingAtPlayer,
    Cooldown,
}

public abstract class BTNode
{
    public abstract bool Execute(Blackboard blackboard);
}

public class ConditionalNode : BTNode
{
    private string _key;
    private object _expectedValue;

    public ConditionalNode(string key, object value)
    {
        _key = key;
        _expectedValue = value;
    }
    public override bool Execute(Blackboard blackboard)
    {
        return blackboard.GetValue<object>(_key)?.Equals(_expectedValue) ?? false;
    }
}

public class ActionNode : BTNode
{
    private Action _action;

    public ActionNode(Action action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action), "Action cannot be null!");
        }

        _action = action;
    }

    public override bool Execute(Blackboard blackboard)
    {
        try
        {
            _action?.Invoke();
            return true;
        }
        catch
        {
            LoggingUtils.Error("Trying to execute a null action in an ActionNode");
            return false;
        }
    }
}