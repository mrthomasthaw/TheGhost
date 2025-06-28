using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw {
    public class AIGoalEliminateThreat : AIGoal
    {

        public override void SetUp(AIBlackBoard blackBoard)
        {
            base.SetUp(blackBoard);
            minPriority = 0;
            maxPriority = 10;

            EndGoal.Add("assaultPrimaryThreat", true);
        }

        public override int CalculatePriority()
        {
            AIBBDSelectedPrimaryThreat primaryThreat = blackBoard.GetOneBBData<AIBBDSelectedPrimaryThreat>(EnumType.AIBlackBoardKey.SelectedPrimaryThreat);
            if(primaryThreat != null) 
            {
                Priority = 10;
            }
            else
            {
                Priority = 0;
            }

            Priority = Mathf.Clamp(Priority, minPriority, maxPriority);
            return Priority;
        }
    }
}
