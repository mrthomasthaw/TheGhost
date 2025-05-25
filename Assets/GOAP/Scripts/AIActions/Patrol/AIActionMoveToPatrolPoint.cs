using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw.Goap.AIActions
{
    
    public class AIActionMoveToPatrolPoint : AIAction
    {
        public override void SetUp(AIBlackBoard blackBoard)
        {            
            Preconditions.Add("hasPatrolPointToSecure", true);

            Effects.Add("reachedPatrolPoint", true);
        }


    }
}
