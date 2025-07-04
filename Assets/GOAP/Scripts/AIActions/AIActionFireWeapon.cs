using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw.Goap.AIActions
{
    public class AIActionFireWeapon : AIAction
    {
        private AIBBDSMBFireWeapon bbDFireWeapon = new AIBBDSMBFireWeapon();
        private AIBBDSelectedPrimaryThreat bbDSelectedPrimaryThreat = null;

        public override void SetUp(AIBlackBoard blackBoard)
        {
            base.SetUp(blackBoard);
            Preconditions.Add(EnumType.AIWorldStateKey.HasTarget.ToString(), true);
            Preconditions.Add(EnumType.AIWorldStateKey.Aim.ToString(), true);


            Effects.Add(EnumType.AIWorldStateKey.AssaultTarget.ToString(), true);
        }

        public override void OnActionStart(AIBlackBoard blackBoard)
        {
            bbDSelectedPrimaryThreat = blackBoard.GetOneBBData<AIBBDSelectedPrimaryThreat>(EnumType.AIBlackBoardKey.SelectedPrimaryThreat);

            AbortAction = bbDSelectedPrimaryThreat == null || ! bbDSelectedPrimaryThreat.IsStillValid;
            bbDFireWeapon.IsStillValid = true; // To enable action
            blackBoard.AddData(bbDFireWeapon);
        }

        public override bool OnActionPerform(AIBlackBoard blackBoard)
        {
            bool isValidThreat = bbDSelectedPrimaryThreat != null && bbDSelectedPrimaryThreat.IsStillValid;

            if(! isValidThreat)
            {
                bbDFireWeapon.IsStillValid = false;
                blackBoard.RemoveBBData(bbDFireWeapon);
                return true; // End the action
            }
            return false;
        }
    }
}
