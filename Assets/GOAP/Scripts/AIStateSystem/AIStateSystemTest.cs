using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIStateSystemTest : MonoBehaviour
{
    private MrThaw.AIStateSystemMove moveState;
    private Animator animator;
    private NavMeshAgent agent;

    public Transform targetT;

    [SerializeField]
    private MrThaw.MoveProps moveProps;

    private MrThaw.AIBlackBoard blackBoard;

    public Transform Transform;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        Transform = this.transform;

        blackBoard = new MrThaw.AIBlackBoard();
        moveState = new MrThaw.AIStateSystemMove(moveProps, agent, animator, Transform);
        moveState.OnStart();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Pressed F");

            //Note : this will be added in action
            blackBoard.AddData<MrThaw.AIBBDMoveToPosition>(
                new MrThaw.AIBBDMoveToPosition(MrThaw.EnumType.AIMoveType.Walk, 0.15f, targetT.position, true, true));

            moveState.OnActionActivate(blackBoard);
        }


        moveState.OnUpdate(blackBoard);
    }


    private void OnAnimatorMove()
    {
        moveState.OnAnimatorMove();
    }

    
}
