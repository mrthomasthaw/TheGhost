using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw.Goap.AIActions
{
    public abstract class AIAction
    {
        public int cost;

        public bool AbortAction { get; set; }

        public bool RequiredStatesToComplete { get; set; }

        public Dictionary<string, object> Preconditions { get; protected set; }
        public Dictionary<string, object> Effects { get; protected set; }


        public bool IsPreconditionsMatch(Dictionary<string, object> worldStates)
        {
            foreach(KeyValuePair<string, object> state in worldStates)
            {
                if (!Preconditions.ContainsKey(state.Key)) return false;

                if (!Preconditions[state.Key].Equals(state.Value)) return false;
            }
            return true;
        }

        public virtual void SetUp(AIBlackBoard blackBoard) 
        {
            Preconditions = new Dictionary<string, object>();
            Effects = new Dictionary<string, object>();
        }

        public virtual void OnActionStart(AIBlackBoard blackBoard) { }

        // return true if the action is completed
        public virtual bool OnActionPerform(AIBlackBoard blackBoard) { return true; }

        public virtual void OnActionComplete(AIBlackBoard blackBoard) { }


    }
}
