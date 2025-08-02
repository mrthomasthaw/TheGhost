using MrThaw.AIMapData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MrThaw.Goap.AIMemory.AIInfo
{
    public class AIInfoTacticalPosition : AIMemoryData
    {
        public bool IsStillValid { get; set; }

        public override EnumType.AIMemoryKey Key => EnumType.AIMemoryKey.TacticalPositionInfo;

        public TacticalPositionPoint TacticalPositionPoint { get; set; }

        public int Id { get; set; }

        public Vector3 Position { get; set; }

    }
}
