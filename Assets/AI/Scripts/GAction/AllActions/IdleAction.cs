using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleAction : GAction 
{
    private AIWeaponControl weaponControl;

    public IdleAction(AIWeaponControl weaponControl)
    {
        this.weaponControl = weaponControl;
    }

    public override void SetUp(BlackBoardManager blackBoardManager)
    {
        base.SetUp(blackBoardManager);

        Preconditions.Add(AIWorldStateKey.AimWeapon.ToString(), false);
        Preconditions.Add(AIWorldStateKey.HasPrimaryTarget.ToString(), false);
    }

}
