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
            Preconditions.Add(EnumType.AIWorldStateKey.HasTarget.ToString(), true);
            Preconditions.Add(EnumType.AIWorldStateKey.Aim.ToString(), false);
            RequiredStatesToComplete = true;

            Effects.Add(EnumType.AIWorldStateKey.Aim.ToString(), true);
        }

        public override void OnActionStart(AIBlackBoard blackBoard)
        {
            AIBBDSelectedPrimaryThreat threat = blackBoard.GetOneBBData<AIBBDSelectedPrimaryThreat>(EnumType.AIBlackBoardKey.SelectedPrimaryThreat);
            if(threat != null && threat.IsStillValid)
            {
                AIBBDTurnToTransform turnToTransformBBD = new AIBBDTurnToTransform(threat.ThreatT);
                blackBoard.AddData(turnToTransformBBD);
                blackBoard.AddData(aimBBD);
            }
            else
            {
                AbortAction = true;
            }

        }
    }

}
