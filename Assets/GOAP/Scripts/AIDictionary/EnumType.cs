using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw
{
    public class EnumType
    {
        public enum AIPatrolType
        {
            Sequenced,
            Random
        }

        public enum AIActionRepeatType 
        {
            Repetitive,
            Once
        }

        public enum AIActionFinishType
        {
            ActionOnly,
            StateOnly,
            Both
        }

        public enum AIMoveType
        {
            Idle,
            Walk,
            Run,
            Sprint
        }

        public enum AIAlertType
        {
            Normal, // If the alert type is normal, it will set the one of the following goal priority high [Idle, Patrol]
            Cautious, // If the alert type is cautious, it will set one of the following goal priority high [Search Area]
            Danger // If the alert type is danger, it will set one of the following goal priority high [Kill Target, Take Cover]
        }

        public enum AIBlackBoardKey
        {
            Animate,   
            SelectedPrimaryThreat,
            FireWeapon,
            MoveTo, 
            TurnTo,
            LookAt,
            PatrolRoute,
            OverallDamageToBodyConfidence
        }

        public enum AIMemoryKey
        {
            ThreatInfo,
        }

        public enum AIWorldStateKey
        {
            AssaultTarget,
            Aim,
            SecureArea,
            AlertType,
            HasTarget
        }
    }
}
