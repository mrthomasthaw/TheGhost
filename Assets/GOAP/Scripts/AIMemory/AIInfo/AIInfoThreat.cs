using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw.Goap.AIMemory.AIInfo
{
    public class AIInfoThreat : AIMemoryData
    {
        //BE SURE TO UPDATE Equals AND HasCode Method WHEN ADDING NEW FIELD
        public Transform TargetTransform { get; set; }

        public bool IsStillValid { get; set; }

        public override EnumType.AIMemoryKey Key => EnumType.AIMemoryKey.ThreatInfo;

        public HealthControl TargetHealthControl { get; set; }

       

        public override string ToString()
        {
            return $"{{{nameof(TargetTransform)}={TargetTransform}, {nameof(Score)}={Score.ToString()}}}";
        }
    }

}
