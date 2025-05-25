using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw.Goap.AIMemory.AIInfo
{
    public class AIInfoThreat : AIMemoryData
    {
        public Transform TargetTransform { get; set; }

        public override bool Equals(object obj)
        {
            return obj is AIInfoThreat threat &&
                   //Score == threat.Score &&
                   EqualityComparer<Transform>.Default.Equals(TargetTransform, threat.TargetTransform);
        }

        public override int GetHashCode()
        {
            int hashCode = -514700431;
            //hashCode = hashCode * -1521134295 + Score.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Transform>.Default.GetHashCode(TargetTransform);
            return hashCode;
        }

        public override string ToString()
        {
            return $"{{{nameof(TargetTransform)}={TargetTransform}, {nameof(Score)}={Score.ToString()}}}";
        }
    }

}
