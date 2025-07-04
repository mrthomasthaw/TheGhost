using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw.Goap.AIActions
{
    public class AIActionUnAim : AIAction
    {
        private AIBBDAnimate bbDUnAimAnimate = new AIBBDAnimate(AIAnimationDict.AnimationDict.UnAim);

        public override void SetUp(AIBlackBoard blackBoard)
        {
            base.SetUp(blackBoard);
            Preconditions.Add(EnumType.AIWorldStateKey.HasTarget.ToString(), false);
            Preconditions.Add(EnumType.AIWorldStateKey.Aim.ToString(), true);

            Effects.Add(EnumType.AIWorldStateKey.Aim.ToString(), false);
        }

        public override void OnActionStart(AIBlackBoard blackBoard)
        {
            blackBoard.AddData(bbDUnAimAnimate);
        }
    }
}

