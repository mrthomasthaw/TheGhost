using System.Collections.Generic;
using UnityEngine;

namespace MrThaw
{
    public class PointDataMono : MonoBehaviour
    {
        public int pointDataNo;
        public PointHolder pointHolder = new PointHolder();
        public PointData pointData;

        public class PointHolder
        {
            private List<int> holders;

            public PointHolder()
            {
                holders = new List<int>();
            }

            public void AddUserToPoint(int holderId)
            {
                holders.Add(holderId);
            }

            public void RemoveUser(int holderId)
            {
                holders.Remove(holderId);
            }

            public bool IsOccupied { get { return !(holders == null || holders.Count == 0); } }
        }
    }
}