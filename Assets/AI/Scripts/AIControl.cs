using MrThaw;
using MrThaw.Goap.AIWorldState;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AIControl : MonoBehaviour
{

    private Queue<GAction> actionSequence = new Queue<GAction>();

    private List<GSensor> sensorList = new List<GSensor>();

    private GPlanner planner;

    private GAction currentAction;

    private GWorldState agentWorldState;

    private BlackBoardManager blackBoardManager;

    private Transform headT;

    private LayerMask obstacleLayer;

    private bool Replan;

    private float timer;

    private void Start()
    {
        SetUpWorldStates();

        blackBoardManager = new BlackBoardManager();

        VisionSensor visionSensor = new VisionSensor(GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head), 
            transform, blackBoardManager, obstacleLayer, agentWorldState);

        sensorList = new List<GSensor>
        {
            visionSensor
        };

        sensorList.ForEach(s  => s.SetUp());

        AIWeaponControl weaponControl = GetComponent<AIWeaponControl>();
        AIAnimationControl animationControl = GetComponent<AIAnimationControl>();
        
        IdleAction idleAction = new IdleAction(weaponControl);
        AimWeaponAction aimWeaponAction = new AimWeaponAction(animationControl, weaponControl, transform);
        FireWeaponAction fireWeaponAction = new FireWeaponAction(weaponControl);
        UnAimWeaponAction unAimWeaponAction = new UnAimWeaponAction(weaponControl, animationControl);

        List<GAction> actionList = new List<GAction>
        {
            idleAction,
            aimWeaponAction,
            unAimWeaponAction,
            fireWeaponAction
        };

        currentAction = actionList.ToArray()[0];

        IdleGoal idleGoal = new IdleGoal();
        EliminateThreatGoal eliminateThreatGoal = new EliminateThreatGoal();

        List<GGoal> goalList = new List<GGoal>
        {
            idleGoal,
            eliminateThreatGoal
        };

        planner = new GPlanner(agentWorldState, goalList, actionList, blackBoardManager);

    }

    private void SetUpWorldStates()
    {
        agentWorldState = new GWorldState();
        agentWorldState.Add(AIWorldStateKey.HasPrimaryTarget.ToString(), false);
        agentWorldState.Add(AIWorldStateKey.AimWeapon.ToString(), false);
    }

    void Update()
    {
        Debug.Log("current action : " + currentAction);


        Debug.Log("WorldState : " + agentWorldState.PrintWorldStates());
        if (Replan)
        {
            Debug.Log("Replan");

            UpdateSensors(); // Update replan

            //The sensors should be updated before calculating the goal
            planner.CalculateGoalPriority();

            actionSequence = planner.CalculateActionPlan();
            Debug.Log("New plan : " + CommonUtil.StringJoin(actionSequence));

            if (currentAction != null)
                currentAction.AbortAction = true;

            Replan = false; // reset replan
        }

        planner.PrintCurrentGoal();


        UpdateSensors(); // Update replan

        //UpdateSystems();

        UpdateActionSequence(); // use replan

        //states.ForEach(s => s.OnUpdate(Blackboard));

    }

    private void UpdateActionSequence()
    {

        //NOTE NEED TO UPDATE WORLD STATES HERE

        if (currentAction == null)
        {
            Debug.Log("Assign action");
            if (actionSequence.Count == 0)
            {
                Debug.Log("No action to execute");
                Replan = true;
                return;
            }

            if (actionSequence.Peek().RepeatAction)
            {
                currentAction = actionSequence.Peek();
            }
            else
            {
                currentAction = actionSequence.Dequeue();
            }

            currentAction.OnActionStart();
            if (!currentAction.AbortAction)
            {
                //states.ForEach(s => s.OnActionActivate());
            }
        }
        else
        {
            if (currentAction.AbortAction)
            {
                Debug.Log("Abort action");
                //Plan.Clear();
                ExitCurrentAction();
                //Stop the whole action sequence
            }
            else if (currentAction.OnActionPerform()) // is the action completed
            {
                Debug.Log("On action complete");
                if (currentAction.RequiredStatesToComplete)
                {
                    bool allStateComplete = AllStateComplete();

                    if (!allStateComplete)
                        return;
                }


                Debug.Log("All state completed");
                ExitCurrentAction();
            }

            //if current action is repeatable, it should update the worldstates
        }

        Debug.Log(agentWorldState.PrintWorldStates());
        //NOTE NEED TO UPDATE WORLD STATES HERE

    }

    private void UpdateSensors()
    {
        if (timer <= 0)
        {
            sensorList.ForEach(x => x.OnUpdate());
            timer = 2f;
        }

        timer -= Time.deltaTime;
    }

    private void ExitCurrentAction()
    {
        currentAction.OnActionComplete(); // when the action is completed
        agentWorldState.CopyWorldStates(currentAction.Effects);
        currentAction = null;

        //foreach (AIStateSystem state in states)
        //{
        //    state.OnActionExit(Blackboard);
        //}
    }

    private bool AllStateComplete()
    {
        bool allStateComplete = true;
        //foreach (AIStateSystem state in states)
        //{
        //    if (!state.HasStateFinished())
        //    {
        //        allStateComplete = false;
        //        break;
        //    }
        //}

        return allStateComplete;
    }
}
