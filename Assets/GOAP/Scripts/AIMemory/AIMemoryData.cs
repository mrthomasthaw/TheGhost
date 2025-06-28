using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw.Goap.AIMemory
{
    public abstract class AIMemoryData
    {
        public abstract EnumType.AIMemoryKey Key { get; }

        public int Score { get; set; }
    }
}

