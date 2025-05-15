using System.Collections.Generic;

public class BehaviorTree
{
    private List<BTNode> _nodes = new List<BTNode>();
    public void AddNode(BTNode node) => _nodes.Add(node);
    public void Run(Blackboard blackboard)
    {
        foreach (var node in _nodes)
        {
            if (!node.Execute(blackboard)) break;
        }
    }   
}