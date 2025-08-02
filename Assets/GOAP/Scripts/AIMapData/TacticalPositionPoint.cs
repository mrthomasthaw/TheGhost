using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw.AIMapData
{
    public class TacticalPositionPoint : MonoBehaviour
    {
        public int Id { get; set; }
        public Vector3 Position {  get; private set; }

        public GameObject GameObject { get; private set; }


    }
}
