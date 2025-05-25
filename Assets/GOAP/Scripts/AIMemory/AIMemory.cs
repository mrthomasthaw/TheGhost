using MrThaw.Goap.AIMemory.AIInfo;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw.Goap.AIMemory
{
    public class AIMemory
    {
        public AIMemory()
        {
            Data = new List<AIMemoryData>();
        }

        public List<AIMemoryData> Data { get; set; }

        public AIInfoThreat GetThreatInfoByTransform(Transform transform)
        {
            if (transform == null) return null;
            return Data.OfType<AIInfoThreat>().Where(s => s.TargetTransform != null && s.TargetTransform == transform).FirstOrDefault();
        }
    }
}

