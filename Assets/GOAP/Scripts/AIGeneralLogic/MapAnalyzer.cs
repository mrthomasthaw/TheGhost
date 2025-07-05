using UnityEngine;

namespace MrThaw
{
    [ExecuteInEditMode]
    public class MapAnalyzer : MonoBehaviour
    {
        [Header("Tile Parameters")]
        public Vector3 checkStartPoint;

        public float rayLength = .65f;
        public float maxDistanceX = 15;
        public float maxDistanceZ = 15;
        public float checkSpace = .5f;
        public GameObject pointPrefab;

        //[Header("Distance generation parameters")]
        //[Space]
        //public float distanceDataCollectPerPointOverlapRadius = 10; // longer distance bigger the data

        [Space]
        [Header("Cover generation parameters")]
        public float coverEdgeCheckStep = .005f;

        public float sideEdgeRayMaxDistance = .75f;

        [Space]
        public float closeCoverRemovalDist = .075f;

        public float closeCoverEdgeRemovalDist = .095f;

        [Space]
        public CoverTargetLogic.CoverPointCheckParams coverCheckParams;

        public CoverTargetLogic.CoverCrouchCheckParams coverCrouchCheckParams;
    }
}