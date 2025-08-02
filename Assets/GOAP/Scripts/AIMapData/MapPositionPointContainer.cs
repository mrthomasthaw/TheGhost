using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw.AIMapData
{
    [System.Serializable]
    public class MapPositionPointContainer : MonoBehaviour
    {
#if UNITY_EDITOR
        public int pointDebugCount = 150;
        public int debugIndexStartFrom = 100;
        public bool debugCoverNormals = true;
        public bool debugCoverCrouchVal = true;
        public bool debugCoverEdges = true;

#endif

        public List<TacticalPositionPoint> TacticalPositionPoints {  get; set; }
    }
}
