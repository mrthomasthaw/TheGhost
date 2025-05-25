using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MrThaw.Goap.AIActions;

namespace MrThaw
{
    //[CreateAssetMenu(fileName = "AIGoal", menuName = "ScriptableObjects /GOAP/AIGoal", order = 2)]
    public abstract class AIGoal
    {
        protected AIBlackBoard blackBoard;

        private Dictionary<string, object> endGoal = new Dictionary<string, object>();

        private int priority;

        public Dictionary<string, object> EndGoal { get => endGoal; protected set => endGoal = value; }

        public List<AIAction> actionList;

        public int minPriority;

        public int maxPriority;

        public int Priority { get => priority; protected set => priority = value; }

        public virtual void SetUp(AIBlackBoard blackBoard)
        {
            this.blackBoard = blackBoard;
        }

        public abstract int CalculatePriority();
    }
}

