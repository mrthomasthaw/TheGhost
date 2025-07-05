using System.Collections.Generic;
using UnityEngine;

namespace MrThaw
{
    [System.Serializable]
    public class MapData : MonoBehaviour
    {
#if UNITY_EDITOR
        public int pointDebugCount = 150;
        public int debugIndexStartFrom = 100;
        public bool debugCoverNormals = true;
        public bool debugCoverCrouchVal = true;
        public bool debugCoverEdges = true;

        [Space]
#endif

        public List<PointDataMono> allPointDataMonos;
    }

    [System.Serializable]
    public class PointData
    {
        public override string ToString()
        {
            return HaveCover + base.ToString();
        }

        public bool HaveCover
        {
            get
            {
                if (possibleCovers == null)
                    return false;
                return possibleCovers.Count > 0;
            }
        }

        public float posX, posY, posZ;

        public Vector3 Position
        {
            get { return new Vector3(posX, posY, posZ); }
            set { posX = value.x; posY = value.y; posZ = value.z; }
        }

        public List<PointCoverData> possibleCovers;

        //public List<DistanceData> distanceDataList;
        public int pointDataNo;

        [System.NonSerialized]
        public GameObject pointGoTempNonSerialized;

        public PointData(int _pointDataNo, Vector3 staticPosition, GameObject _go)
        {
            pointDataNo = _pointDataNo;
            Position = staticPosition;
            pointGoTempNonSerialized = _go;
        }

        //public float GetMoveDistanceToPoint(PointData pointData)
        //{
        //    foreach (var distData in distanceDataList)
        //    {
        //        if (distData.pointDataIndexNo == pointData.pointDataNo)
        //            return distData.distanceToPoint;
        //    }
        //    return -1;
        //}
    }

    [System.Serializable]
    public class PointCoverData
    {
        public bool isEdge = false;
        public bool isLeftEdge = false;

        public bool IsEqual(PointCoverData cd)
        {
            return cd.CoverNormal == CoverNormal && cd.CoverPosition == CoverPosition;
        }

        public PointCoverData(Vector3 _normal, Vector3 _pos, float _coverHeight)
        {
            CoverPosition = _pos;
            CoverNormal = _normal;
            coverHeight = _coverHeight;
        }

        public PointCoverData(Vector3 _normal, Vector3 _pos)
        {
            CoverPosition = _pos;
            CoverNormal = _normal;
            coverHeight = 1;
        }

        public PointCoverData(PointCoverData cd)
        {
            CoverPosition = cd.CoverPosition;
            CoverNormal = cd.CoverNormal;
            coverHeight = cd.coverHeight;
        }

        public float coverPosX, coverPosY, coverPosZ;

        public Vector3 CoverPosition
        {
            get { return new Vector3(coverPosX, coverPosY, coverPosZ); }
            set { coverPosX = value.x; coverPosY = value.y; coverPosZ = value.z; }
        }

        public float coverNormalX, coverNormalY, coverNormalZ;

        public Vector3 CoverNormal
        {
            get
            {
                return new Vector3(coverNormalX, coverNormalY, coverNormalZ);
            }
            set { coverNormalX = value.x; coverNormalY = value.y; coverNormalZ = value.z; }
        }

        public float coverHeight = 0;
        public int coverUniqueNo = 0;
    }

    //[System.Serializable]
    //public class DistanceData
    //{
    //    public int pointDataIndexNo; // Unique number for points
    //    public float distanceToPoint;
    //    public DistanceData(int _pointDataIndex, float _distanceToPoint)
    //    {
    //        pointDataIndexNo = _pointDataIndex;
    //        distanceToPoint = _distanceToPoint;
    //    }

    //}
}