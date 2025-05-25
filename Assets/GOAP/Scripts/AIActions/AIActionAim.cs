using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw.Goap.AIActions
{
    public class AIActionAim : AIAction
    {
        private AIBBDAnimate aimBBD = new AIBBDAnimate(AIAnimationDict.AnimationDict.Aim);

        public override void SetUp(AIBlackBoard blackBoard)
        {
            base.SetUp(blackBoard);
            Preconditions.Add("aim", false);
            RequiredStatesToComplete = true;

            Effects.Add("aim", true);
        }

        public override void OnActionStart(AIBlackBoard blackBoard)
        {
            AIBBDSelectedPrimaryThreat threat = blackBoard.GetBBData<AIBBDSelectedPrimaryThreat>();
            if(threat != null)
            {
                AIBBDTurnToTransform turnToTransformBBD = new AIBBDTurnToTransform(threat.ThreatT);
                blackBoard.Add(turnToTransformBBD);
                blackBoard.Add(aimBBD);
            }

        }
    }

}
