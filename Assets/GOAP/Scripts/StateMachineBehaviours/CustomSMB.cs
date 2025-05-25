using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MrThaw;

public class CustomSMB : StateMachineBehaviour
{
    protected AIBlackBoard blackBoard;

    public virtual void SetUp(Animator animator,  AIBlackBoard blackBoard)
    {
        this.blackBoard = blackBoard;
    }
}
