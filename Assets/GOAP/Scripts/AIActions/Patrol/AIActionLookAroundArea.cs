using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw.Goap.AIActions
{
    public class AIActionLookAroundArea : AIAction
    {
        public override void SetUp(AIBlackBoard blackBoard)
        {
            Preconditions.Add("reachedPatrolPoint", true);

            Effects.Add("secureArea", true);
        }

    }
}
