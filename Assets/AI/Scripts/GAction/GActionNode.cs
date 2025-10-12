using MrThaw.Goap.AIActions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GActionNode
{
    public int Cost { get; set; }

    public Dictionary<string, object> WorldStates { get; set; }

    public GActionNode ParentNode { get; set; }
    public GAction Action { get; set; }

    public GActionNode() { }

    public GActionNode(int cost, GActionNode parentNode, GAction action, Dictionary<string, object> worldStates)
    {
        this.Cost = cost;
        this.ParentNode = parentNode;
        this.Action = action;
        this.WorldStates = worldStates;
    }
}
