using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw.Goap.AIActions
{  
    public class AIActionScanOrQueryPatrolPoint : AIAction
    {

        public override void SetUp(AIBlackBoard blackBoard)
        {
            Effects.Add("hasPatrolPointToSecure", true);
        }

    }
}
