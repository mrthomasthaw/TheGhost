using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw
{
    public class AIGoalPatrol : AIGoal
    {

        public override void SetUp(AIBlackBoard blackBoard)
        {
            base.SetUp(blackBoard);
            //EndGoal.Add(EnumType.AIWorldStateKey.SecureArea.ToString(), true);
        }

        public override int CalculatePriority()
        {
            AIBBDSelectedPrimaryThreat bBDSelectedPrimaryThreat = blackBoard.GetOneBBData<AIBBDSelectedPrimaryThreat>(EnumType.AIBlackBoardKey.SelectedPrimaryThreat);
            if(bBDSelectedPrimaryThreat == null || ! bBDSelectedPrimaryThreat.IsStillValid)
            {
                return 10;
            }
            return 0;
        }
    }
}
