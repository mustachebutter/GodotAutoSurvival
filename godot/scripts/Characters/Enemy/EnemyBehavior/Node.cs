using System;
using System.Collections.Generic;
using System.Diagnostics;
public enum BTNodeState
{
    Success,
    Running,
    Failure,
}
public abstract class BTNode
{
    public abstract BTNodeState Execute(float delta);
    public abstract void Reset();
}

public class WaitUntilNode : BTNode
{
    private Func<bool> _condition;

    public WaitUntilNode(Func<bool> condition)
    {
        _condition = condition;
    }

    public override BTNodeState Execute(float delta)
    {
        return _condition() ? BTNodeState.Success : BTNodeState.Running;
    }

    public override void Reset()
    {
    }

}

public class ConditionalNode : BTNode
{
    private Func<bool> _condition;
    public ConditionalNode(Func<bool> condition)
    {
        if (condition == null)
        {
            throw new ArgumentNullException(nameof(condition), "Condition function cannot be null!");
        }
        _condition = condition;
    }
    public override BTNodeState Execute(float delta)
    {
        if (_condition())
        {
            return BTNodeState.Success;
        }
        else
        {
            return BTNodeState.Failure;
        }
    }

    public override void Reset()
    {
    }

}

// aka. Inverter or Repeater node
// SuccessIfConditionMetDecorator 
public class ConditionalControllerNode : ConditionalNode
{
    private BTNode _child;
    public ConditionalControllerNode(Func<bool> condition, BTNode child) : base(condition)
    {
        if (child == null)
        {
            throw new ArgumentNullException(nameof(child), "Child node cannot be null");
        }
        _child = child;
    }

    public override BTNodeState Execute(float delta)
    {
        BTNodeState conditionResult = base.Execute(delta);
        if (conditionResult == BTNodeState.Success)
        {
            return _child.Execute(delta);
        }
        else
        {
            _child.Reset();
            return BTNodeState.Failure;
        }
    }

    public override void Reset()
    {
        _child.Reset();
    }

}


public class ActionNode : BTNode
{
    private Func<float, BTNodeState> _action;

    public ActionNode(Func<float, BTNodeState> action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action), "Action cannot be null!");
        }

        _action = action;
    }

    public override BTNodeState Execute(float delta)
    {
        try
        {
            return _action(delta);
        }
        catch (Exception ex)
        {
            LoggingUtils.Error($"ActionNode: Error executing action: {ex.Message}");
            return BTNodeState.Failure;
        }
    }
    
    // ActionNode usually has no internal state to reset itself unless it's a long-running task
    // that needs to track its own progress (e.g., a "MoveToTarget" action that returns Running)
    // If it *does* have internal state, override Reset() here.

    public override void Reset()
    {
        throw new NotImplementedException();
    }

}

public class SelectorNode : BTNode
{
    private List<BTNode> _children = new List<BTNode>();
    private int _currentIndex = 0;

    public SelectorNode(List<BTNode> children)
    {
        _children = children;
    }

    public override BTNodeState Execute(float delta)
    {
        // Every time it goes in processing the current child
        if (_currentIndex < _children.Count)
        {
            BTNodeState childStatus = _children[_currentIndex].Execute(delta);
            switch (childStatus)
            {
                case BTNodeState.Success:
                    _currentIndex = 0;
                    return BTNodeState.Success;
                case BTNodeState.Running:
                    return BTNodeState.Running;
                case BTNodeState.Failure:
                    _currentIndex++;
                    break;
            }
        }

        // It gets here mean the previous child failed so process the rest.
        while (_currentIndex < _children.Count)
        {
            BTNodeState childStatus = _children[_currentIndex].Execute(delta);
            switch (childStatus)
            {
                case BTNodeState.Success:
                case BTNodeState.Running:
                    return childStatus;
                case BTNodeState.Failure:
                    _currentIndex++;
                    break;
            }
        }

        _currentIndex = 0;
        return BTNodeState.Failure;
    }

    public override void Reset()
    {
        _currentIndex = 0;
        foreach (var child in _children)
        {
            child.Reset();
        }
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

    public override BTNodeState Execute(float delta)
    {
        if (_currentIndex >= _children.Count)
        {
            _currentIndex = 0;
        }

        BTNodeState childStatus = _children[_currentIndex].Execute(delta);

        switch (childStatus)
        {
            case BTNodeState.Success:
                _currentIndex++;
                if (_currentIndex >= _children.Count)
                {
                    _currentIndex = 0;
                    return BTNodeState.Success;
                }
                else
                {
                    return BTNodeState.Running;
                }
            case BTNodeState.Running:
                return BTNodeState.Running;
            case BTNodeState.Failure:
                _currentIndex = 0;
                return BTNodeState.Failure;
            default:
                return BTNodeState.Failure;
        }
    }

    public override void Reset()
    {
        _currentIndex = 0;
        foreach (var child in _children)
        {
            child.Reset();
        }
    }
}

public class DoOnceNode : BTNode
{
    private bool _done = false;
    private BTNode _child;

    public DoOnceNode(BTNode child)
    {
        _child = child;
    }
    public override BTNodeState Execute(float delta)
    {
        if (!_done)
        {
            _child.Execute(delta);
            _done = true;
            Reset();
        }

        return BTNodeState.Success;
    }

    public override void Reset()
    {
        _done = false;
    }
}

public class WaitNTick : BTNode
{
    private string _name = "";
    private int _numberOfTicks = 0;
    private int _ticksToWait = 0;

    public WaitNTick(int ticksToWait, string name)
    {
        _name = name;
        _ticksToWait = ticksToWait;
    }

    public override BTNodeState Execute(float delta)
    {
        if (_ticksToWait <= 0)
        {
            LoggingUtils.Error("No ticks to wait, skipping");
            return BTNodeState.Success;
        }

        _numberOfTicks++;
        LoggingUtils.Debug($"[{_name}]Waiting {_numberOfTicks} tick");

        if (_numberOfTicks >= _ticksToWait)
        {
            Reset();
            return BTNodeState.Success;
        }

        return BTNodeState.Running;
    }

    public override void Reset()
    {
        _numberOfTicks = 0;
    }

}
