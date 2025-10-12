using MrThaw;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GAction
{
    protected BlackBoardManager blackBoardManager;

    public int cost;

    public bool AbortAction { get; set; }

    public bool RepeatAction { get; set; }

    public bool RequiredStatesToComplete { get; set; }

    public Dictionary<string, object> Preconditions { get; protected set; }
    public Dictionary<string, object> Effects { get; protected set; }


    public bool IsPreconditionsMatch(Dictionary<string, object> worldStates)
    {
        foreach (KeyValuePair<string, object> state in worldStates)
        {
            if (!Preconditions.ContainsKey(state.Key)) return false;

            if (!Preconditions[state.Key].Equals(state.Value)) return false;
        }
        return true;
    }

    public virtual void SetUp(BlackBoardManager blackBoardManager)
    {
        Preconditions = new Dictionary<string, object>();
        Effects = new Dictionary<string, object>();
        this.blackBoardManager = blackBoardManager;
    }

    public virtual void OnActionStart() { }

    // return true if the action is completed
    public virtual bool OnActionPerform() { return true; }

    public virtual void OnActionComplete()
    {
        AbortAction = false;
    }
}
