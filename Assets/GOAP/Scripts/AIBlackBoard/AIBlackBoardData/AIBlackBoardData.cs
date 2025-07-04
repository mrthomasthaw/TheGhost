using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw
{
    public abstract class AIBlackBoardData
    {
        public abstract EnumType.AIBlackBoardKey Key { get; }

        public bool IsStillValid { get; set; }


    }
}
