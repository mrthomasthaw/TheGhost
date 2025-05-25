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

        public AIBBDSelectedPrimaryThreat()
        {

        }

        public AIBBDSelectedPrimaryThreat(AIInfoThreat threatInfo)
        {
            Name = threatInfo.TargetTransform.name;
            ThreatT = threatInfo.TargetTransform;
        }

        public void UpdateThreatInfo(AIInfoThreat threatInfo)
        {
            Name = threatInfo.TargetTransform.name;
            ThreatT = threatInfo.TargetTransform;
        }

        public override string ToString()
        {
            return $"{{{nameof(Name)}={Name}, {nameof(ThreatT)}={ThreatT}}}";
        }
    }
}