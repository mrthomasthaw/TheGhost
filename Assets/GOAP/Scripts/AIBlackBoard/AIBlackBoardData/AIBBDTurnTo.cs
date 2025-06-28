using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw
{
    public class AIBBDTurnTo : AIBlackBoardData
    {
        public override EnumType.AIBlackBoardKey Key => EnumType.AIBlackBoardKey.TurnTo;
    }

    public class AIBBDTurnToPosition : AIBBDTurnTo
    {
        public Vector3 Position { get; private set; }

        public AIBBDTurnToPosition(Vector3 _position)
        {
            Position = _position;
        }
    }

    public class AIBBDTurnToTransform : AIBBDTurnTo
    {
        public Transform Transform { get; private set; }

        public AIBBDTurnToTransform(Transform _transform)
        {
            Transform = _transform;
        }
    }
}
