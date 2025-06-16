using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw.Goap.AIActions
{
    public class AIActionFireWeapon : AIAction
    {
        private AIBBDSMBFireWeapon bbDFireWeapon = new AIBBDSMBFireWeapon(true);

        public override void SetUp(AIBlackBoard blackBoard)
        {
            base.SetUp(blackBoard);
            RepeatAction = true;
            Preconditions.Add("aim", true);

            Effects.Add("assaultPrimaryThreat", true);
        }

        public override void OnActionStart(AIBlackBoard blackBoard)
        {
            blackBoard.Add(bbDFireWeapon);
        }
    }
}
