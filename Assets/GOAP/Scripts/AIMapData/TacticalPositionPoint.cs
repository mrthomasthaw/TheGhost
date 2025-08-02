using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw.AIMapData
{
    public class TacticalPositionPoint : MonoBehaviour
    {
        [SerializeField]
        private int id;

        [SerializeField]
        private Vector3 position;

        public int Id { get { return id; } }
        public Vector3 Position { get { return position; } }


        public void SetData(int id, Vector3 position)
        {
            this.id = id;
            this.position = position;
        }
    }
}
