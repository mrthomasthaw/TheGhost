using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw.Goap.AIMemory
{
    public abstract class AIMemoryData
    {
        //BE SURE TO UPDATE Equals AND HasCode Method WHEN ADDING NEW FIELD IN ITS SUBCLASSES
        public abstract EnumType.AIMemoryKey Key { get; }

        public float Score { get; set; }
    }
}

