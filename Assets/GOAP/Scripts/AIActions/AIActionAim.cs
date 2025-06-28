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
            AIBBDSelectedPrimaryThreat threat = blackBoard.GetOneBBData<AIBBDSelectedPrimaryThreat>(EnumType.AIBlackBoardKey.SelectedPrimaryThreat);
            if(threat != null)
            {
                AIBBDTurnToTransform turnToTransformBBD = new AIBBDTurnToTransform(threat.ThreatT);
                blackBoard.AddData(turnToTransformBBD);
                blackBoard.AddData(aimBBD);
            }

        }
    }

}
