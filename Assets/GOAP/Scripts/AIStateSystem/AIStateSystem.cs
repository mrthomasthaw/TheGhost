using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw
{
    public class AIStateSystem
    {
        public virtual void OnStart()
        {

        }

        public virtual void OnStart(AIBlackBoard blackBoard)
        {
        }

        public virtual void OnActionActivate(AIBlackBoard blackBoard)
        {
        }


        public virtual void OnUpdate(AIBlackBoard blackboard)
        {

        }

        public virtual bool HasStateFinished() { return false; }

        public virtual void OnActionExit(AIBlackBoard blackboard)
        {
        }

        public virtual void OnAnimatorMove()
        {
        }

        public virtual void OnAnimatorIK(AIBlackBoard blackBoard, int layerNo)
        {
        }

        public virtual void OnAnimatorMove(AIBlackBoard blackBoard)
        {
        }

        public virtual bool HasStateFinished(AIBlackBoard blackBoard)
        {
            return true;
        }

        public virtual bool IsInterruptable(AIBlackBoard blackBoard)
        {
            return true;
        }


    }
}
