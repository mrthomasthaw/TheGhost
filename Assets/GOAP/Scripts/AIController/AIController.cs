using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MrThaw;
using MrThaw.Goap.AISensors;
using MrThaw.Goap.AIMemory;
using MrThaw.Goap.AISystem;
using UnityEngine.AI;
using MrThaw.Goap.AIActions;
using System;
using MrThaw.Goap.AIWorldState;

public class AIController : MonoBehaviour
{
    private Queue<AIAction> Plan = new Queue<AIAction>();

    private Queue<AIAction> nextPlan = new Queue<AIAction>();

    private AIAction currentAction;

    private AIPlanner planner;

    public AIMemory Memory { get; private set; }
    public AIBlackBoard Blackboard { get; private set; }

    public List<AIGoal> goalList = new List<AIGoal>();

    public List<AIAction> actionList = new List<AIAction>();

    public PatrolRoute patrolRoute;

    //public Dictionary<string, object> WorldState 
    //{
    //    get {
    //        return worldState;
    //    } 
    //}

    public AIWorldState AgentWorldState { get { return agentWorldState; } }
    public bool Replan { get => replan; set => replan = value; }

    List<AIStateSystem> states = new List<AIStateSystem>();

    List<AISystem> systems = new List<AISystem>();

    //private Dictionary<string, object> worldState;
    private AIWorldState agentWorldState;
    private Animator animator;
    private NavMeshAgent agent;
    private Transform Transform;

    public Transform targetT;

    [SerializeField]
    private MrThaw.MoveProps moveProps;

    [SerializeField]
    private MrThaw.HumanLookIKProps lookIKProps;

    [SerializeField]
    private WeaponInventoryData inventoryData;

    private MrThaw.WeaponInventory weaponInventory;

    public WeaponInventory WeaponInventory { get { return weaponInventory; } }

    private WeaponPositionControl weaponPositionControl;

    List<AISensor> sensorList = new List<AISensor>();

    List<CustomSMB> customStateMachineBehaviours = new List<CustomSMB>();


    float timer = 2f;

    public Transform head;
    public LayerMask obstacleLayer;

    private float systemUpdateTime = 2f;

    private bool replan;
    

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        Transform = this.transform;

        weaponPositionControl = GetComponent<WeaponPositionControl>();
        weaponPositionControl.IKControl.SetLookObj(CommonUtil.FindDeepestChildByName(transform, "LookObj"));

        weaponInventory = new WeaponInventory(animator, transform, CommonUtil.FindDeepestChildByName(transform, "WeaponUnEquipPointHandGun"), animator.GetBoneTransform(HumanBodyBones.LeftHand),
                animator.GetBoneTransform(HumanBodyBones.RightHand), CommonUtil.FindDeepestChildByName(transform, "AimPivotL"), CommonUtil.FindDeepestChildByName(transform, "AimPivotR"),
                GetComponent<WeaponPositionControl>(), inventoryData);

        weaponInventory.SetUp();

        Blackboard = new MrThaw.AIBlackBoard();

        Blackboard.AddData(new AIBBDPatrolRoute(patrolRoute));



        actionList.ForEach(x => x.SetUp(Blackboard));

        states.Add(new AIStateSystemMove(moveProps, agent, animator, Transform));
        states.Add(new AIStateSystemAnimate(animator, weaponPositionControl));
        //states.Add(new AIStateSystemHumanLookIK(lookIKProps, animator, Transform));

        states.ForEach(x => x.OnStart(Blackboard));

        Memory = new AIMemory();
        sensorList.Add(new AISensorVision(head, transform, Memory, obstacleLayer));

        systems.Add(new AISystemPrimaryThreatSelection(Memory, Blackboard));

        goalList.Add(new AIGoalIdle());
        goalList.Add(new AIGoalEliminateThreat());

        actionList.Add(new AIActionPatrolWithWait());
        actionList.Add(new AIActionAim());
        actionList.Add(new AIActionFireWeapon());

       
        SetUpWorldStates();

        planner = new AIPlanner();
        planner.SetUp(agentWorldState, goalList, actionList, Blackboard);

        Plan.Enqueue(actionList[0]);

        customStateMachineBehaviours.Add(animator.GetBehaviour<WeaponTargetingSMB>());

        customStateMachineBehaviours.ForEach(c => c.SetUp(animator, Blackboard));
    }

    private void SetUpWorldStates()
    {
        //worldState = new Dictionary<string, object>();
        //worldState.Add("aiAlertType", EnumType.AIAlertType.Normal);
        //worldState.Add("aim", false);
        agentWorldState = new AIWorldState();
        agentWorldState.Add("aiAlertType", EnumType.AIAlertType.Normal);
        agentWorldState.Add("aim", false);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("current acti " + currentAction);
        Blackboard.PrintDataDict();
        if(replan)
        {
            Debug.Log("Replan");
            planner.CalculateGoalPriority();

            Plan = planner.CalculateActionPlan();
            Debug.Log("New plan : " + CommonUtil.StringJoin(Plan));

            currentAction.AbortAction = true;

            replan = false; // reset replan
        }

        planner.PrintCurrentGoal();

        //OnUpdate();

        UpdateSensors(); // Update replan

        UpdateSystems();

        UpdateActionSequence(); // use replan

        states.ForEach(s => s.OnUpdate(Blackboard));

    }

    private void ResetReplan()
    {
        if (replan)
            replan = false;
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

    private void UpdateSystems()
    {
        if (systemUpdateTime < 0)
        {
            systems.ForEach(x => x.OnUpdate(this));
            systemUpdateTime = 2f;
        }

        systemUpdateTime -= Time.deltaTime;
    }

    private void UpdateActionSequence()
    {

        //NOTE NEED TO UPDATE WORLD STATES HERE
       
        if (currentAction == null)
        {
            Debug.Log("Assign action");
            if (Plan.Count == 0)
            {
                Debug.Log("No action to execute");
                return;
            }

            if(Plan.Peek().RepeatAction)
            {
                currentAction = Plan.Peek();
            }
            else
            {
                currentAction = Plan.Dequeue();
            }

            currentAction.OnActionStart(Blackboard);
            if(! currentAction.AbortAction)
            {
                states.ForEach(s => s.OnActionActivate(Blackboard));
            }
        }
        else
        {
            if(currentAction.AbortAction)
            {
                Debug.Log("Abort action");
                //Plan.Clear();
                currentAction.OnActionComplete(Blackboard);
                currentAction = null;

                foreach (AIStateSystem state in states)
                {
                    state.OnActionExit(Blackboard);
                }
                //Stop the whole action sequence
            }
            else if(currentAction.OnActionPerform(Blackboard)) // is the action completed
            {
                Debug.Log("On action complete");
                if (currentAction.RequiredStatesToComplete)
                {
                    bool allStateComplete = AllStateComplete();

                    if (!allStateComplete)
                        return;
                }


                Debug.Log("All state completed");
                currentAction.OnActionComplete(Blackboard); // when the action is completed

                agentWorldState.CopyWorldStates(currentAction.Effects);
                currentAction = null;
            }
        }

        Debug.Log(agentWorldState.PrintWorldStates());
        //NOTE NEED TO UPDATE WORLD STATES HERE
        
    }

    private bool AllStateComplete()
    {
        bool allStateComplete = true;
        foreach (AIStateSystem state in states)
        {
            if (!state.HasStateFinished())
            {
                allStateComplete = false;
                break;
            }
        }

        return allStateComplete;
    }

    //    private void OnUpdate()
    //    {
    //        bool replanned = false;
    //        bool actionChanged = false;

    //        if (!replanned && Plan != null && Plan.Count > 0)
    //        {
    //            //Debug.Log(Plan.Peek().GetType());
    //            if (!Plan.Peek().IsActivated) // action activation
    //            {
    //                actionChanged = true;
    //                if (Plan.Peek().CanActivate(Blackboard))
    //                {

    //                    Plan.Peek().Activate(Blackboard);
    //                    Plan.Peek().LastActivatedAt = Time.time;
    //                    Plan.Peek().IsActivated = true;
    //                    foreach (var state in states)
    //                        state.OnActionActivate(Blackboard);
    //                    Plan.Peek().OnActionCantActivate(Blackboard);
    //                }
    //                else // can't activate this action
    //                {
    //                    Plan.Clear();
    //                    foreach (var state in states)
    //                        state.OnActionExit(Blackboard);
    //                }
    //            }
    //            else if (Plan.Peek().IsActivated)
    //            {
    //                //if (AIWorldState.ConditionsMatch(Plan.Peek().actionPreCons, WorldState) &&
    //                //    AIWorldState.ConditionsMatch(Plan.Peek().sensorPreCons, WorldState) &&
    //                if (Plan.Peek().IsStillValid(Blackboard) &&
    //                    Plan.Peek().CanApplyToWorld(WorldState))
    //                {
    //                    Plan.Peek().OnUpdate(Blackboard);
    //                    if (HasFinished(Plan.Peek()))
    //                    {
    ////#if UNITY_EDITOR
    ////                        if (brain.actionDebug)
    ////                            brain.ShowDebugMessage("Completed Action" + Plan.Peek());
    ////#endif
    //                        actionChanged = true;
    //                        Plan.Peek().IsActivated = false;
    //                        Plan.Peek().OnActionFinishedSuccessfully(Blackboard);
    //                        Plan.Peek().LastExitTime = Time.time;

    //                        SetWorldStatePostEffects(Plan.Peek());

    //                        foreach (var state in states)
    //                            state.OnActionExit(Blackboard);

    //                        if (Plan.Peek().RepeatType == EnumType.AIActionRepeatType.Once)
    //                            Plan.Dequeue();
    //                    }
    //                }
    //                else
    //                {
    ////#if UNITY_EDITOR
    ////                    if (brain.actionDebug)
    ////                        brain.ShowDebugMessage("Deactivated Action 'Not Valid' " + Plan.Peek());
    ////#endif
    //                    actionChanged = true;
    //                    Plan.Peek().OnActionDeactivated(Blackboard);
    //                    Plan.Peek().IsActivated = false;
    //                    Plan.Peek().LastExitTime = Time.time;

    //                    Plan.Clear();
    //                    foreach (var state in states)
    //                        state.OnActionExit(Blackboard);
    //                }
    //            }
    //        }



    //        if (!replanned && !actionChanged)
    //        {
    //            foreach (var state in states)
    //            {
    //                state.OnUpdate(Blackboard);
    //            }
    //        }


    //    }

    private void OnAnimatorMove()
    {
        foreach (var state in states)
        {
            state.OnAnimatorMove();
        }
    }

    //    private void PlanActions()
    //    {
    //        bool needToReplanBySystem = true;
    //        if (needToReplanBySystem || Plan == null || Plan.Count == 0)
    //        {
    //            if (Plan == null || Plan.Count == 0)
    //            {
    //                if (nextPlan != null)
    //                {
    //                    Plan = nextPlan;
    //                    nextPlan = null;
    //                }
    //                else
    //                {
    //#if UNITY_EDITOR
    //                    //Note that the code inside the UNITY_EDITOR block only execute during the editor mode
    //                    Debug.Log("unity editor");
    //                    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    //                    sw.Start();
    //#endif

    //                    Plan = planner.CalculateActionPlan(this, WorldState, out currentGoal);
    //#if UNITY_EDITOR
    //                    if (brain.plannerDebug)
    //                    {
    //                        sw.Stop();
    //                        brain.ShowDebugMessage("Planned (Active), Took " + sw.ElapsedMilliseconds + " ms to calculate a plan, Goal is " + currentGoal);
    //                    }
    //#endif
    //                }

    //                replanned = true;
    //                lastPlanTime = Time.time;
    //            }
    //            else if (Plan != null && Plan.Peek().IsInterruptable(this) && HaveUnInterruptableState(this))
    //            {
    //                if (Time.time - lastPlanTime > brain.minPlanInterval)
    //                {
    //#if UNITY_EDITOR
    //                    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    //                    sw.Start();
    //#endif

    //                    Queue<AIAction> planNew = planner.CalculatePlan(this, WorldState, out currentGoal);

    //                    bool samePlan = true;
    //                    if (planNew != null && Plan.Count == planNew.Count)
    //                        for (int i = 0; i < Plan.Count && i < planNew.Count; i++)
    //                        {
    //                            if (Plan.ElementAt(i) != planNew.ElementAt(i))
    //                            {
    //                                samePlan = false;
    //                                break;
    //                            }
    //                        }
    //                    else
    //                        samePlan = false;
    //#if UNITY_EDITOR
    //                    if (brain.plannerDebug)
    //                    {
    //                        sw.Stop();
    //                        if (samePlan)
    //                            brain.ShowDebugMessage("Planned (Not Active), Took " + sw.ElapsedMilliseconds + " ms to calculate a plan, Goal is " + currentGoal);
    //                        else
    //                            brain.ShowDebugMessage("Planned (Active : Next Update), Took " + sw.ElapsedMilliseconds + " ms to calculate a plan, Goal is " + currentGoal);
    //                    }
    //#endif
    //                    if (!samePlan)
    //                    {
    //                        replanned = true;

    //                        nextPlan = planNew;

    //                        if (Plan.Peek().IsActivated)
    //                        {
    //                            Plan.Peek().OnActionDeactivated(Blackboard);
    //                            Plan.Peek().IsActivated = false;
    //                            Plan.Peek().LastExitTime = Time.time;
    //                        }
    //                        foreach (var state in states)
    //                            state.OnActionExit(Blackboard);

    //                        Plan.Clear();
    //                    }
    //                }
    //            }
    //        }
    //    }

    //private void SetWorldStatePostEffects(AIAction action)
    //{
    //    if (action.RepeatType != EnumType.AIActionRepeatType.Repetitive)
    //        CommonUtil.CopyWorldStates(action.Effects, WorldState);
    //}

    //private bool HasFinished(AIAction action)
    //{
    //    switch (action.FinishType)
    //    {
    //        case EnumType.AIActionFinishType.ActionOnly:
    //            return action.HasFinished();

    //        case EnumType.AIActionFinishType.StateOnly:
    //            foreach (var state in states)
    //                if (!state.HasStateFinished())
    //                    return false;
    //            return true;

    //        case EnumType.AIActionFinishType.Both:
    //            if (action.HasFinished())
    //            {
    //                foreach (var state in states)
    //                    if (!state.HasStateFinished())
    //                        return false;
    //                return true;
    //            }
    //            else
    //                return false;
    //    }
    //    return false;
    //}    //private void SetWorldStatePostEffects(AIAction action)
    //{
    //    if (action.RepeatType != EnumType.AIActionRepeatType.Repetitive)
    //        CommonUtil.CopyWorldStates(action.Effects, WorldState);
    //}

    //private bool HasFinished(AIAction action)
    //{
    //    switch (action.FinishType)
    //    {
    //        case EnumType.AIActionFinishType.ActionOnly:
    //            return action.HasFinished();

    //        case EnumType.AIActionFinishType.StateOnly:
    //            foreach (var state in states)
    //                if (!state.HasStateFinished())
    //                    return false;
    //            return true;

    //        case EnumType.AIActionFinishType.Both:
    //            if (action.HasFinished())
    //            {
    //                foreach (var state in states)
    //                    if (!state.HasStateFinished())
    //                        return false;
    //                return true;
    //            }
    //            else
    //                return false;
    //    }
    //    return false;
    //}
}
