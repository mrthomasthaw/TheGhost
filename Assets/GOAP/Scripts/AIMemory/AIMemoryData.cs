using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw.Goap.AIMemory
{
    public abstract class AIMemoryData
    {
        public abstract EnumType.AIMemoryKey Key { get; }

        public float Score { get; set; }
    }
}

