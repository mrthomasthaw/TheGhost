using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponReadySMB : CustomStateMachineBehaviour
{

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animationFinishingInfo.HaveAimingFinished = true;
    }

}
