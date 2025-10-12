using MrThaw;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GPlanner
{
    public List<GGoal> goalList = new List<GGoal>();

    public List<GAction> allActionList = new List<GAction>();

    //public Dictionary<string, object> currentWorldStates;

    public GGoal currentGoal;

    public BlackBoardManager blackBoardManager;

    private GWorldState agentWorldState;

    public GPlanner(GWorldState _agentWorldState, List<GGoal> _goalList, List<GAction> _allActionList, BlackBoardManager blackBoardManager)
    {
        //currentWorldStates = new Dictionary<string, object>(worldStates); // These must be a copy from aiController

        agentWorldState = _agentWorldState;
        goalList = _goalList;
        goalList.ForEach(g => g.SetUp(blackBoardManager));

        allActionList = _allActionList;
        allActionList.ForEach(a => a.SetUp(blackBoardManager));

        currentGoal = goalList[0];
    }

    public void CalculateGoalPriority()
    {
        currentGoal = goalList.OrderByDescending(g => g.CalculatePriority()).First();

        Debug.Log(CommonUtil.StringJoin(goalList));
        Debug.Log("New current goal : " + currentGoal);
    }

    public void PrintCurrentGoal()
    {
        if (currentGoal != null)
        {
            Debug.Log(currentGoal.GetType().Name);
        }
        else
        {
            Debug.Log("Current Goal not found");
        }
    }

    public Queue<GAction> CalculateActionPlan()
    {
        List<GAction> clonedActionList = new List<GAction>(allActionList);

        List<GActionNode> leaves = new List<GActionNode>();
        GActionNode rootNode = new GActionNode(0, null, null, agentWorldState.WorldStates);
        bool foundPath = BuildActionGraph(clonedActionList, leaves, rootNode, currentGoal.EndGoal);

        if (!foundPath)
        {
            Debug.Log("No suitable action found");
            return new Queue<GAction>();
        }

        GActionNode cheapestNode = leaves.OrderBy(l => l.Cost).First();
        GActionNode n = cheapestNode;

        List<GAction> selectedActionList = new List<GAction>();
        while (n != null)
        {
            if (n.Action != null)
                selectedActionList.Insert(0, n.Action);

            n = n.ParentNode;
        }

        Queue<GAction> q = new Queue<GAction>(selectedActionList);
        return q;
    }

    private bool BuildActionGraph(List<GAction> actionList, List<GActionNode> leaves, GActionNode rootNode, Dictionary<string, object> goalStates)
    {
        bool foundPath = false;
        foreach (GAction action in actionList)
        {
            if (CommonUtil.MatchWorldStates(action.Preconditions, rootNode.WorldStates))
            {
                Dictionary<string, object> newPostEffects = new Dictionary<string, object>(rootNode.WorldStates);
                CommonUtil.CopyWorldStates(action.Effects, newPostEffects);

                GActionNode newNode = new GActionNode(rootNode.Cost + action.cost, rootNode, action, newPostEffects);

                if (CommonUtil.MatchWorldStates(goalStates, newNode.WorldStates))
                {
                    leaves.Add(newNode);
                    foundPath = true;
                }
                else
                {
                    List<GAction> newActionList = new List<GAction>(actionList);
                    newActionList.Remove(action);

                    bool found = BuildActionGraph(newActionList, leaves, newNode, goalStates);

                    if (found)
                        foundPath = true;
                }
            }
        }

        return foundPath;
    }

    private void DebugActionNode(List<GActionNode> collectedActionNodeList)
    {
        int i = 0;
        foreach (GActionNode node in collectedActionNodeList)
        {
            List<string> actionNameList = new List<string>();
            GActionNode nextNode = node;
            do
            {
                if (nextNode != null && nextNode.Action != null)
                {

                    actionNameList.Add(nextNode.Action.GetType().Name);
                    //Debug.Log(nextNode.Action.GetType().Name);
                }


                nextNode = nextNode.ParentNode;
            } while (nextNode != null);

            i++;
            Debug.Log(i + " " + string.Join("  , ", actionNameList));
        }
    }
}
