using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomStateMachineBehaviour : StateMachineBehaviour
{
    protected AnimationStateInfo animationFinishingInfo;

    public void SetUp(AnimationStateInfo animationFinishingInfo)
    {
        this.animationFinishingInfo = animationFinishingInfo;
    }
}
