using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnAimWeaponAction : GAction
{
    private AIWeaponControl weaponControl;

    private AIAnimationControl animationControl;

    public UnAimWeaponAction(AIWeaponControl weaponControl, AIAnimationControl animationControl )
    {
        this.weaponControl = weaponControl;
        this.animationControl = animationControl;
    }

    public override void SetUp(BlackBoardManager blackBoardManager)
    {
        base.SetUp(blackBoardManager);

        Preconditions.Add(AIWorldStateKey.HasPrimaryTarget.ToString(), false);
        Preconditions.Add(AIWorldStateKey.AimWeapon.ToString(), true);

        Effects.Add(AIWorldStateKey.AimWeapon.ToString(), false);
    }

    public override void OnActionStart()
    {
        animationControl.AnimateSingleAction(AnimationKey.PutDownWeapon);
        weaponControl.AimAtTarget(false, null);
    }

    public override bool OnActionPerform()
    {
        return animationControl.IsAnimationFinished(AnimationKey.PutDownWeapon);
    }
}
