using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw.Goap.AIActions
{

    public class AIActionNode
    {
        public int Cost { get; set; }

        public Dictionary<string, object> WorldStates { get; set; }

        public AIActionNode ParentNode { get; set; }
        public AIAction Action { get; set; }

        public AIActionNode() { }

        public AIActionNode(int cost, AIActionNode parentNode, AIAction action, Dictionary<string, object> worldStates)
        {
            this.Cost = cost;
            this.ParentNode = parentNode;
            this.Action = action;
            this.WorldStates = worldStates;
        }
    }
}