using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw
{
    public class AIGoalIdle : AIGoal
    {
        public override void SetUp(AIBlackBoard blackBoard)
        {
            base.SetUp(blackBoard);
            minPriority = 0;
            maxPriority = 10;
            Priority = 1;
        }

        public override int CalculatePriority()
        {
            AIBBDSelectedPrimaryThreat primaryThreat = blackBoard.GetOneBBData<AIBBDSelectedPrimaryThreat>(EnumType.AIBlackBoardKey.SelectedPrimaryThreat);
            Priority = primaryThreat == null || ! primaryThreat.IsStillValid ? maxPriority : minPriority;
            return Priority;
        }
    }
}

