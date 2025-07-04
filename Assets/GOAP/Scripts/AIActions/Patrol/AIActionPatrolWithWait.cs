
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw.Goap.AIActions
{ 
    public class AIActionPatrolWithWait : AIAction
    {
        public EnumType.AIPatrolType patrolType;
        public float reachTolerance = .3f;
        public Vector2 idleInBetweenTimer = new Vector2(4f, 7f);
        public float idleActivateChancePerc = 0f;

        private List<Vector3> patrolPoints;
        private int currentIndex = 0;
        private AIBBDMoveToPosition moveToBbD;
        private float _tempIdleTimer;
        private bool isIdleActive = false;

        public override void SetUp(AIBlackBoard blackBoard)
        {
            base.SetUp(blackBoard);

            RequiredStatesToComplete = true;

            Preconditions.Add(EnumType.AIWorldStateKey.HasTarget.ToString(), false);
            Preconditions.Add(EnumType.AIWorldStateKey.Aim.ToString(), false);

            //Effects.Add(EnumType.AIWorldStateKey.SecureArea.ToString(), true);


            if (blackBoard.GetOneBBData<AIBBDPatrolRoute>(EnumType.AIBlackBoardKey.PatrolRoute) == null || blackBoard.GetOneBBData<AIBBDPatrolRoute>(EnumType.AIBlackBoardKey.PatrolRoute).PatrolRoute == null)
                idleActivateChancePerc = 100;
            else
                patrolPoints = blackBoard.GetOneBBData<AIBBDPatrolRoute>(EnumType.AIBlackBoardKey.PatrolRoute).PatrolRoute.patrolPoints;
        }


        public override void OnActionStart(AIBlackBoard blackBoard)
        {
            //Debug.Log("Current Patrol Index " + currentIndex);
            if (Random.Range(0, 100) > idleActivateChancePerc)
            {
                //Debug.Log("Patrol");
                // Patrol
                ChangePatrolPoint();

                moveToBbD = new AIBBDMoveToPosition(EnumType.AIMoveType.Walk,
                    reachTolerance, patrolPoints[currentIndex],
                    true, true);


                blackBoard.AddData(moveToBbD);
                isIdleActive = false;
            }
            else
            {
                //Debug.Log("Idle");
                // Idle
                _tempIdleTimer = Random.Range(idleInBetweenTimer.x, idleInBetweenTimer.y);
                isIdleActive = true;
            }
        }

        public override bool OnActionPerform(AIBlackBoard blackBoard)
        {
            if (isIdleActive)
                _tempIdleTimer -= Time.deltaTime;

            return HasFinished();
        }

        public override void OnActionComplete(AIBlackBoard blackBoard)
        {
            base.OnActionComplete(blackBoard);
            if (!isIdleActive)
            {
                blackBoard.RemoveBBData(moveToBbD);
                moveToBbD = null;
            }
        }

        public bool HasFinished()
        {
            if (isIdleActive)
            {
                if (_tempIdleTimer < 0)
                    return true;
                else
                    return false;
            }
            return true;
        }

        private void ChangePatrolPoint()
        {
            int index = 0;
            switch (patrolType)
            {
                case EnumType.AIPatrolType.Sequenced:
                    index = (currentIndex + 1) % patrolPoints.Count; 
                    break;

                case EnumType.AIPatrolType.Random:
                    index = Random.Range(0, patrolPoints.Count);
                    break;

                default:
                    break;
            }
            if (index == currentIndex)
                ChangePatrolPoint();
            else
                currentIndex = index;
        }
    }
}