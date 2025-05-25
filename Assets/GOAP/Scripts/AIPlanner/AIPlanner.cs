using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MrThaw.Goap.AIActions;

namespace MrThaw
{
    public class AIPlanner
    {
        public List<AIGoal> goalList = new List<AIGoal>();

        public List<AIAction> allActionList = new List<AIAction>();

        public Dictionary<string, object> currentWorldStates;

        public AIGoal currentGoal;

        public AIBlackBoard blackBoard;

        public void SetUp(Dictionary<string, object> worldStates, List<AIGoal> _goalList, List<AIAction> _allActionList, AIBlackBoard blackBoard)
        {
            currentWorldStates = new Dictionary<string, object>(worldStates); // These must be a copy from aiController

            goalList = _goalList;
            goalList.ForEach(g => g.SetUp(blackBoard));

            allActionList = _allActionList;
            allActionList.ForEach(a => a.SetUp(blackBoard));

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

        public Queue<AIAction> CalculateActionPlan()
        {
            List<AIAction> clonedActionList = new List<AIAction>(allActionList);

            List<AIActionNode> leaves = new List<AIActionNode>();
            AIActionNode rootNode = new AIActionNode(0, null, null, currentWorldStates);
            bool foundPath = BuildActionGraph(clonedActionList, leaves, rootNode, currentGoal.EndGoal);

            if(! foundPath)
            {
                Debug.Log("No suitable action found");
                return new Queue<AIAction>();
            }

            AIActionNode cheapestNode = leaves.OrderBy(l => l.Cost).First();
            AIActionNode n = cheapestNode;

            List<AIAction> selectedActionList = new List<AIAction>();
            while(n != null)
            {
                if(n.Action != null)
                    selectedActionList.Insert(0, n.Action);

                n = n.ParentNode;
            }

            Queue<AIAction> q = new Queue<AIAction>(selectedActionList);
            return q;
        }

        private bool BuildActionGraph(List<AIAction> actionList, List<AIActionNode> leaves, AIActionNode rootNode, Dictionary<string, object> goalStates)
        {
            bool foundPath = false;
            foreach(AIAction action in actionList)
            {
                if (CommonUtil.MatchWorldStates(action.Preconditions, rootNode.WorldStates))
                {
                    Dictionary<string, object> newPostEffects = new Dictionary<string, object>(rootNode.WorldStates);
                    CommonUtil.CopyWorldStates(action.Effects, newPostEffects);

                    AIActionNode newNode = new AIActionNode(rootNode.Cost + action.cost, rootNode, action, newPostEffects);

                    if(CommonUtil.MatchWorldStates(goalStates, newNode.WorldStates))
                    {
                        leaves.Add(newNode);
                        foundPath = true;
                    }
                    else
                    {
                        List<AIAction> newActionList = new List<AIAction>(actionList);
                        newActionList.Remove(action);

                        bool found = BuildActionGraph(newActionList, leaves, newNode, goalStates);

                        if (found)
                            foundPath = true;
                    }
                }
            }

            return foundPath;
        }

        private void DebugActionNode(List<AIActionNode> collectedActionNodeList)
        {
            int i = 0;
            foreach (AIActionNode node in collectedActionNodeList)
            {
                List<string> actionNameList = new List<string>();
                AIActionNode nextNode = node;
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

        //private void BuildActionTree(List<AIAction> actionList, List<AIActionNode> collectedActionNodeList, Dictionary<string, object> goal, AIActionNode rootNode)
        //{
        //    int i = 0;
        //    bool foundSomething = false;
        //    foreach (AIAction action in actionList)
        //    {
        //        if (action.CheckEffects(goal))
        //        {
        //            foundSomething = true;
        //            Debug.Log(++i + " Action found : " + action.GetType().Name);

        //            List<AIAction> updatedActionList = new List<AIAction>(actionList);
        //            updatedActionList.Remove(action);

        //            Dictionary<string, object> tempGoal = new Dictionary<string, object>(goal);                
        //            CopyWorldStates(action.Preconditions, tempGoal);

        //            int parentNodeCost = rootNode == null ? 0 : rootNode.Cost;
        //            AIActionNode selectedNode = new AIActionNode(action.cost + parentNodeCost, rootNode, action);

        //            BuildActionTree(updatedActionList, collectedActionNodeList, tempGoal, selectedNode);
        //        }
        //    }

        //    if(! foundSomething && rootNode != null) // Found something in above but not in this level
        //        collectedActionNodeList.Add(rootNode);
        //}

        
        
    }
}
