using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MrThaw.Goap.AIMemory.AIInfo;

namespace MrThaw
{
    public class AIBBDSelectedPrimaryThreat : AIBlackBoardData
    {
        public string Name { get; set; }
        public Transform ThreatT { get; set; }

        public HealthControl TargetHealthControl { get; set; }

        public override EnumType.AIBlackBoardKey Key => EnumType.AIBlackBoardKey.SelectedPrimaryThreat;

        public AIBBDSelectedPrimaryThreat()
        {

        }

        public AIBBDSelectedPrimaryThreat(AIInfoThreat threatInfo)
        {
            Name = threatInfo.TargetTransform.name;
            ThreatT = threatInfo.TargetTransform;
            IsStillValid = threatInfo.IsStillValid;
            TargetHealthControl = threatInfo.TargetHealthControl;
        }

        public void UpdateThreatInfo(AIInfoThreat threatInfo)
        {
            Name = threatInfo.TargetTransform.name;
            ThreatT = threatInfo.TargetTransform;
            IsStillValid = threatInfo.IsStillValid;
            TargetHealthControl = threatInfo.TargetHealthControl;
        }

        public override string ToString()
        {
            return $"{{{nameof(Name)}={Name}, {nameof(ThreatT)}={ThreatT}, {nameof(IsStillValid)}={IsStillValid}}}";
        }
    }
}