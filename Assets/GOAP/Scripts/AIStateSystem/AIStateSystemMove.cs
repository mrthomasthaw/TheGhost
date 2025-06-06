using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MrThaw
{
    public class AIStateSystemMove : AIStateSystem
    {
        private Vector2 smoothDeltaPosition;
        private Vector2 velocity;
        private MoveProps moveProps;
        private Transform turnToTransform;
        private Transform moveToTransform;
        private Transform aiTransform;
        private float defaultReachTolerance = .25f;
        private float reachTolerance;
        private bool isStateActivated = false;
        private NavMeshAgent agent;
        private Animator animator;
        private bool layer2Enabled = false;

       
        public Vector3 moveToPosition { get; private set; }
        public EnumType.AIMoveType moveType { get; private set; }
        public Vector3 turnToPosition { get; private set; }
        public bool shouldTurnToPosition { get; private set; }
        public Transform MoveToTransform { get => moveToTransform; set => moveToTransform = value; }
        public bool IsStateActivated { get => isStateActivated; set => isStateActivated = value; }

        //public PatrolRoute PatrolRoute { get; set; }

        public AIStateSystemMove(MoveProps locProps, NavMeshAgent _agent, Animator _animator, Transform _aiTransform)
        {
            moveProps = locProps;
            agent = _agent;
            animator = _animator;
            aiTransform = _aiTransform;
        }

        public override void OnStart(AIBlackBoard blackBoard)
        {
            base.OnStart(blackBoard);
            OnStart();
        }

        public override void OnStart()
        {
            agent.updatePosition = false;
            //ai.Blackboard.Add(new AIBBDPatrolRoute(PatrolRoute));
            moveToPosition = agent.nextPosition;
        }

        public override void OnActionActivate(AIBlackBoard blackBoard)
        {
            //Debug.Log(CommonUtil.stringJoin(blackBoard.Datas));
            reachTolerance = defaultReachTolerance;
            AIBBDMoveTo moveToBBD = blackBoard.GetBBData<AIBBDMoveTo>();
            AIBBDTurnTo turnToBBD = blackBoard.GetBBData<AIBBDTurnTo>();
            SetMoveNTurnToStartMove(moveToBBD, turnToBBD, blackBoard);
            if (moveToTransform)
                moveToPosition = moveToTransform.position;
            agent.destination = moveToPosition;

            //Debug.Log("After activation : " + CommonUtil.stringJoin(blackBoard.Datas));
        }

        public override void OnUpdate(AIBlackBoard blackBoard)
        {

            if (turnToTransform)
                turnToPosition = turnToTransform.position;
            if (MoveToTransform)
                moveToPosition = MoveToTransform.position;


            if (IsStateActivated)
            {
                Move();
            }
            else
            {
                DontMoveWithAgent();
                animator.SetFloat("VelX", 0, .1f, Time.deltaTime);
                animator.SetFloat("VelY", 0, .1f, Time.deltaTime);
                animator.SetFloat("Speed", 0);
            }
            if (layer2Enabled)
                animator.SetLayerWeight(3,
                        Mathf.Lerp(animator.GetLayerWeight(3),
                        1, Time.deltaTime * moveProps.layer3EnableSpeed));
            else
                animator.SetLayerWeight(3,
                        Mathf.Lerp(animator.GetLayerWeight(3),
                        0, Time.deltaTime * moveProps.layer3DisableSpeed));
        }

        public override void OnActionExit(AIBlackBoard blackboard)
        {
            // Reset this state system
            turnToTransform = null;
            moveToTransform = null;
            isStateActivated = false;
            shouldTurnToPosition = false;
            moveToPosition = agent.nextPosition;
        }

        public override void OnAnimatorMove()
        {
            Vector3 position = animator.rootPosition;
            position.y = agent.nextPosition.y;
            aiTransform.position = position;
        }

        private bool ReachedDestination(float nearTolerance = 0)
        {
            if (agent.remainingDistance < agent.radius + nearTolerance && !agent.pathPending)
                return true;
            return false;
        }

        public override bool HasStateFinished()
        {
            if (isStateActivated)
                return ReachedDestination(reachTolerance);
            return true;
        }

        public void Move()
        {
            Transform transform = aiTransform;
            agent.destination = moveToPosition;
            bool turnedMovement = false;
            if (shouldTurnToPosition)
            {
                agent.updateRotation = false; 
                turnedMovement = true;
            }
            else
            {
                agent.updateRotation = true;
                turnedMovement = false;
            }

            #region Unity Manual Code

            Vector3 worldDeltaPosition = agent.nextPosition - transform.position;

            // Map 'worldDeltaPosition' to local space
            float dx = Vector3.Dot(transform.right, worldDeltaPosition);
            float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
            Vector2 deltaPosition = new Vector2(dx, dy);


            // Low-pass filter the deltaMove
            float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
            smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

            // Update velocity if delta time is safe
            if (Time.deltaTime > 1e-5f)
                velocity = smoothDeltaPosition / Time.deltaTime;

            bool shouldMove = velocity.magnitude > 0.5f && agent.remainingDistance > agent.radius;

            // Pull agent towards character[Modded pull multiplier]
            if (worldDeltaPosition.magnitude > agent.radius)
                agent.nextPosition = transform.position + .01f * worldDeltaPosition;

            // Set transform's y to agent
            transform.position = new Vector3(transform.position.x, agent.nextPosition.y, transform.position.z);

            #endregion Unity Manual Code

            Vector3 desiredDir = (-transform.position + new Vector3(agent.nextPosition.x, transform.position.y, agent.nextPosition.z)).normalized * 2;

            float angle = 0;
            if (turnedMovement)
            {
                angle = Vector3.Angle(transform.forward, (turnToPosition - transform.position).normalized);
                angle = angle * Vector3.Dot(transform.right, (turnToPosition - transform.position).normalized);
                Vector3 rotVector = -transform.position + new Vector3(turnToPosition.x, transform.position.y, turnToPosition.z);
                if (rotVector != Vector3.zero)
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(rotVector), Time.deltaTime * moveProps.idleTurnToSmooth);
            }

            float speedAnim = Vector2.SqrMagnitude(new Vector2(animator.GetFloat("VelX"), animator.GetFloat("VelY")));
            if (turnedMovement)
            {
                if (speedAnim < moveProps.legsStopTurnAtSqrM && Mathf.Abs(angle) > moveProps.legsStartTurnAtAngle)
                {
                    animator.SetFloat("LegsAngle", angle * moveProps.legsTurnAngleMult, moveProps.legsTurnAngleDamp, Time.deltaTime);
                    animator.SetBool("LegTurn", true);
                    layer2Enabled = true;
                }
                else
                {
                    animator.SetFloat("LegsAngle", 0, moveProps.legsTurnAngleDamp, Time.deltaTime);
                    animator.SetBool("LegTurn", false);
                    layer2Enabled = false;
                }
            }
            else
            {
                animator.SetFloat("LegsAngle", 0, moveProps.legsTurnAngleDamp, Time.deltaTime);
                animator.SetBool("LegTurn", false);
                layer2Enabled = false;
            }

            Quaternion refShift = new Quaternion(transform.rotation.x, transform.rotation.y * -1f, transform.rotation.z, transform.rotation.w);
            Vector3 moveDirection = refShift * desiredDir;

            Debug.DrawLine(transform.position, moveDirection.normalized, Color.yellow);

            float locomotionDamp = moveProps.velocityAnimDamp;
            float velocityLimit = moveProps.animatorWalkSpeed;

            switch (moveType)
            {
                case EnumType.AIMoveType.Walk:
                    velocityLimit = moveProps.animatorWalkSpeed;
                    agent.speed = moveProps.agentWalkSpeed;
                    agent.angularSpeed = moveProps.agentAngularSpeedWalk;
                    break;

                case EnumType.AIMoveType.Run:
                    velocityLimit = moveProps.animatorRunSpeed;
                    agent.speed = moveProps.agentRunSpeed;
                    agent.angularSpeed = moveProps.agentAngularSpeedRun;
                    break;

                case EnumType.AIMoveType.Sprint:
                    velocityLimit = moveProps.animatorSprintSpeed;
                    agent.speed = moveProps.agentSprintSpeed;
                    agent.angularSpeed = moveProps.agentAngularSpeedSprint;
                    break;

                default:
                    break;
            }
            float xVelocity = moveDirection.x, yVelocity = moveDirection.z;
            // Limit velocity
            if (xVelocity > 0)
                xVelocity = xVelocity > velocityLimit ? velocityLimit : xVelocity;
            else if (xVelocity < 0)
                xVelocity = -xVelocity > velocityLimit ? -velocityLimit : xVelocity;
            if (yVelocity > 0)
                yVelocity = yVelocity > velocityLimit ? velocityLimit : yVelocity;
            else if (yVelocity < 0)
                yVelocity = -yVelocity > velocityLimit ? -velocityLimit : yVelocity;
            if (!shouldMove)
            {
                xVelocity = 0;
                yVelocity = 0;
            }


            animator.SetFloat("VelX", xVelocity, locomotionDamp, Time.deltaTime);
            animator.SetFloat("VelY", yVelocity, locomotionDamp, Time.deltaTime);
        }

        public void DontMoveWithAgent()
        {
            if (agent)
            {
                Transform transform = aiTransform;

                agent.destination = aiTransform.position;

                bool turnedMovement = shouldTurnToPosition; agent.updateRotation = false;

                #region Unity Manual Code

                Vector3 worldDeltaPosition = agent.nextPosition - transform.position;

                // Map 'worldDeltaPosition' to local space
                float dx = Vector3.Dot(transform.right, worldDeltaPosition);
                float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
                Vector2 deltaPosition = new Vector2(dx, dy);

                // Low-pass filter the deltaMove
                float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
                smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

                // Update velocity if delta time is safe
                if (Time.deltaTime > 1e-5f)
                    velocity = smoothDeltaPosition / Time.deltaTime;

                bool shouldMove = velocity.magnitude > 0.1f && agent.remainingDistance > agent.radius;

                // Move agent to transform
                if (worldDeltaPosition.magnitude > agent.radius)
                    agent.nextPosition = transform.position + 0f * worldDeltaPosition;

                // Set transform's y to agent
                transform.position = new Vector3(transform.position.x, agent.nextPosition.y, transform.position.z);

                #endregion Unity Manual Code

                Vector3 desiredDir = (-transform.position + new Vector3(agent.nextPosition.x, transform.position.y, agent.nextPosition.z)).normalized * 2;

                float angle = 0;
                if (turnedMovement)
                {
                    angle = Vector3.Angle(transform.forward, (turnToPosition - transform.position).normalized);
                    angle = angle * Vector3.Dot(transform.right, (turnToPosition - transform.position).normalized);
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-transform.position + new Vector3(turnToPosition.x, transform.position.y, turnToPosition.z)), Time.deltaTime * moveProps.idleTurnToSmooth);
                }

                float speedAnim = Vector2.SqrMagnitude(new Vector2(animator.GetFloat("VelX"), animator.GetFloat("VelY")));
                if (turnedMovement)
                {
                    if (speedAnim < moveProps.legsStopTurnAtSqrM && Mathf.Abs(angle) > moveProps.legsStartTurnAtAngle)
                    {
                        animator.SetFloat("LegsAngle", angle * moveProps.legsTurnAngleMult, moveProps.legsTurnAngleDamp, Time.deltaTime);
                        animator.SetBool("LegTurn", true);
                        layer2Enabled = true;
                    }
                    else
                    {
                        animator.SetFloat("LegsAngle", 0, moveProps.legsTurnAngleDamp, Time.deltaTime);
                        animator.SetBool("LegTurn", false);
                        layer2Enabled = false;
                    }
                }
                else
                {
                    animator.SetFloat("LegsAngle", 0, moveProps.legsTurnAngleDamp, Time.deltaTime);
                    animator.SetBool("LegTurn", false);
                    layer2Enabled = false;
                }

                Quaternion refShift = new Quaternion(transform.rotation.x, transform.rotation.y * -1f, transform.rotation.z, transform.rotation.w);
                Vector3 moveDirection = refShift * desiredDir;

                float locomotionDamp = moveProps.velocityAnimDamp;

                EnumType.AIMoveType moveType = EnumType.AIMoveType.Walk;

                float velocityLimit = moveProps.animatorWalkSpeed;

                switch (moveType)
                {
                    case EnumType.AIMoveType.Walk:
                        velocityLimit = moveProps.animatorWalkSpeed;
                        agent.speed = moveProps.agentWalkSpeed;
                        agent.angularSpeed = moveProps.agentAngularSpeedWalk;
                        break;

                    case EnumType.AIMoveType.Run:
                        velocityLimit = moveProps.animatorRunSpeed;
                        agent.speed = moveProps.agentRunSpeed;
                        agent.angularSpeed = moveProps.agentAngularSpeedRun;
                        break;

                    case EnumType.AIMoveType.Sprint:
                        velocityLimit = moveProps.animatorSprintSpeed;
                        agent.speed = moveProps.agentSprintSpeed;
                        agent.angularSpeed = moveProps.agentAngularSpeedSprint;
                        break;

                    default:
                        break;
                }
                float xVelocity = moveDirection.x, yVelocity = moveDirection.z;
                // Limit velocity
                if (xVelocity > 0)
                    xVelocity = xVelocity > velocityLimit ? velocityLimit : xVelocity;
                else if (xVelocity < 0)
                    xVelocity = -xVelocity > velocityLimit ? -velocityLimit : xVelocity;
                if (yVelocity > 0)
                    yVelocity = yVelocity > velocityLimit ? velocityLimit : yVelocity;
                else if (yVelocity < 0)
                    yVelocity = -yVelocity > velocityLimit ? -velocityLimit : yVelocity;

                if (!shouldMove)
                {
                    xVelocity = 0;
                    yVelocity = 0;
                }


                animator.SetFloat("VelX", xVelocity, locomotionDamp, Time.deltaTime);
                animator.SetFloat("VelY", yVelocity, locomotionDamp, Time.deltaTime);
            }
        }

       

        public void SetMoveNTurnToStartMove(AIBBDMoveTo moveToBBD, AIBBDTurnTo turnToBBD, AIBlackBoard blackboard)
        {
            if (moveToBBD != null)
            {
                isStateActivated = true;
                moveType = moveToBBD.MoveType;
                reachTolerance = moveToBBD.ReachTolerance;
                if (moveToBBD.GetType() == typeof(AIBBDMoveToPosition))
                {
                    var bbd = moveToBBD as AIBBDMoveToPosition;
                    moveToPosition = bbd.Position;
                }
                else if (moveToBBD.GetType() == typeof(AIBBDMoveToTransform))
                {
                    var bbd = moveToBBD as AIBBDMoveToTransform;
                    moveToTransform = bbd.Transform;
                }

                blackboard.RemoveBBData(moveToBBD);
            }
            if (turnToBBD != null)
            {
                isStateActivated = true;
                shouldTurnToPosition = true;
                if (turnToBBD.GetType() == typeof(AIBBDTurnToPosition))
                {
                    var bbd = turnToBBD as AIBBDTurnToPosition;
                    turnToPosition = bbd.Position;
                }
                else if (turnToBBD.GetType() == typeof(AIBBDTurnToTransform))
                {
                    var bbd = turnToBBD as AIBBDTurnToTransform;
                    turnToTransform = bbd.Transform;
                }

                blackboard.RemoveBBData(turnToBBD);
            }
        }
    }

    [System.Serializable]
    public class MoveProps
    {
        public float velocityAnimDamp = .25f;
        public float idleTurnToSmooth = 1.4f;

        [UnityEngine.Space]
        [Range(0, 1)]
        public float animatorWalkSpeed = .4f;

        public float agentWalkSpeed = 3.15f;
        public float agentAngularSpeedWalk = 70;

        [UnityEngine.Space]
        [Range(0, 1)]
        public float animatorRunSpeed = .8f;

        public float agentRunSpeed = 5.5f;
        public float agentAngularSpeedRun = 120f;

        [UnityEngine.Space]
        [Range(0, 1)]
        public float animatorSprintSpeed = 1f;

        public float agentSprintSpeed = 7;
        public float agentAngularSpeedSprint = 120;

        [UnityEngine.Space]
        public float legsStopTurnAtSqrM = .1f;

        public float legsStartTurnAtAngle = 5f;

        public float legsTurnAngleMult = 1.5f;
        public float legsTurnAngleDamp = 0.1f;

        [Space]
        public float layer3EnableSpeed = 6f;

        public float layer3DisableSpeed = 6f;
    }
}
