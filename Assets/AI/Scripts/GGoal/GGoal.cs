using MrThaw;
using MrThaw.Goap.AIActions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GGoal
{
    protected BlackBoardManager blackBoardManager;

    private Dictionary<string, object> endGoal = new Dictionary<string, object>();

    private int priority;

    public Dictionary<string, object> EndGoal { get => endGoal; protected set => endGoal = value; }

    public List<AIAction> actionList;

    public int minPriority;

    public int maxPriority;

    public int Priority { get => priority; protected set => priority = value; }

    public virtual void SetUp(BlackBoardManager blackBoardManager)
    {
        this.blackBoardManager = blackBoardManager;
    }

    public abstract int CalculatePriority();
}
