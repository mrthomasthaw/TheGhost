using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw
{
    public class AIBBDLookAt : AIBlackBoardData
    {
        public bool HeadOnlyLook { get; private set;}

        public override EnumType.AIBlackBoardKey Key => EnumType.AIBlackBoardKey.LookAt;
    }

    public class AIBBDLookAtPosition : AIBBDLookAt
    {
        public Vector3 Position { get; private set; }

        public AIBBDLookAtPosition(Vector3 _position)
        {
            Position = _position;
        }
    }

    public class AIBBDLookAtTransform : AIBBDLookAt
    {
        public Transform Transform { get; private set; }

        public AIBBDLookAtTransform(Transform _transform)
        {
            Transform = _transform;
        }
    }
}
