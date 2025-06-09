using System;
using System.Collections.Generic;
public enum EnemyState
{
    Idle,
    Charging,
    ChargingAtPlayer,
    Cooldown,
}

public abstract class BTNode
{
    public abstract bool Execute(float delta);
}

public class WaitNode : BTNode
{
    private float _waitDuration;
    private float _timeElased = 0.0f;


    public WaitNode(float waitDuration)
    {
        _waitDuration = waitDuration;
    }

    public override bool Execute(float delta)
    {
        if (_timeElased <= _waitDuration)
        {
            _timeElased += delta;
            return false;
        }

        _timeElased = 0.0f;
        return true;
    }
}

public class ConditionalNode : BTNode
{
    private Func<bool> _condition;
    public ConditionalNode(Func<bool> condition)
    {
        _condition = condition;
    }
    public override bool Execute(float delta)
    {
        return _condition();
    }
}

public class ConditionalControllerNode : ConditionalNode
{
    private BTNode _childSequenceNode;
    public ConditionalControllerNode(Func<bool> condition, BTNode child) : base(condition)
    {
        _childSequenceNode = child;
    }

    public override bool Execute(float delta)
    {
        if (base.Execute(delta))
        {
            return _childSequenceNode.Execute(delta);
        }
        else
        {
            return false;
        }
        
    }
}


public class ActionNode : BTNode
{
    private Func<float, bool> _action;

    public ActionNode(Func<float, bool> action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action), "Action cannot be null!");
        }

        _action = action;
    }

    public override bool Execute(float delta)
    {
        try
        {
            return _action(delta);
        }
        catch
        {
            LoggingUtils.Error("Trying to execute a null action in an ActionNode");
            return false;
        }
    }
}

public class SelectorNode : BTNode
{
    private List<BTNode> _children = new List<BTNode>();
    public SelectorNode(List<BTNode> children)
    {
        _children = children;
    }
    public override bool Execute(float delta)
    {
        foreach (var child in _children)
        {
            if (child.Execute(delta))
                return true;
        }
        return false;
    }

}

public class SequenceNode : BTNode
{
    private List<BTNode> _children = new List<BTNode>();
    private int _currentIndex = 0;

    public SequenceNode(List<BTNode> children)
    {
        _children = children;
    }

    public override bool Execute(float delta)
    {
        while (_currentIndex < _children.Count)
        {
            bool childDone = _children[_currentIndex].Execute(delta);

            if (childDone)
                _currentIndex++;
            else
                return false;
        }

        _currentIndex = 0;
        return true;
    }
}