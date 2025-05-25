using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw
{
    public class AIBBDMoveTo : AIBlackBoardData
    {
        public EnumType.AIMoveType MoveType { get; private set; }
        public float ReachTolerance { get; private set; }
        public bool RMTurn { get; set; }
        public bool RMMove { get; set; }

        public AIBBDMoveTo(EnumType.AIMoveType _moveType, float _reachTolerance, bool useRMTurn = true, bool useRMMove = true)
        {
            MoveType = _moveType;
            ReachTolerance = _reachTolerance;
            RMTurn = useRMTurn;
            RMMove = useRMMove;
        }
    }

    public class AIBBDMoveToPosition : AIBBDMoveTo
    {
        public Vector3 Position { get; private set; }

        public AIBBDMoveToPosition(EnumType.AIMoveType _moveType, float _reachTolerance, Vector3 _position, bool useRMTurn = true, bool useRMMove = true) :
            base(_moveType, _reachTolerance, useRMTurn, useRMMove)
        {
            Position = _position;
        }
    }

    public class AIBBDMoveToTransform : AIBBDMoveTo
    {
        public Transform Transform { get; private set; }

        public AIBBDMoveToTransform(EnumType.AIMoveType _moveType, float _reachTolerance, Transform _transform, bool useRMTurn = true, bool useRMMove = true) :
            base(_moveType, _reachTolerance, useRMTurn, useRMMove)
        {
            Transform = _transform;
        }
    }
}
