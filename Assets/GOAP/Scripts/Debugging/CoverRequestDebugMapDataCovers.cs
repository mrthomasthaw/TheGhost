using UnityEngine;

namespace MrThaw
{
    public class CoverRequestDebugMapDataCovers : MonoBehaviour
    {
        public bool showDebugLogCanCover = true;
        public MapData mapData;
        private MapAnalyzer analyzer;

        private void Start()
        {
            analyzer = GetComponent<MapAnalyzer>();
        }

        private void Update()
        {
            if (mapData)
            {
                foreach (var dMono in mapData.GetComponentsInChildren<PointDataMono>())
                {
                    Vector3 coverPosition = Vector3.zero; Vector3 coverNormal = Vector3.zero;

                    bool canCover = CoverTargetLogic.CheckCoverAroundPosition(analyzer.coverCheckParams, dMono.transform.position, ref coverNormal, ref coverPosition,
                            dMono.transform.position);
                    if (showDebugLogCanCover)
                        Debug.Log(canCover);
                }
            }
        }
    }
}