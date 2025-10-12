using MrThaw;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWeaponAction : GAction
{
    private AIWeaponControl weaponControl;

    public FireWeaponAction(AIWeaponControl weaponControl)
    {
        this.weaponControl = weaponControl;
    }

    public override void SetUp(BlackBoardManager blackBoardManager)
    {
        base.SetUp(blackBoardManager);
        Preconditions.Add(AIWorldStateKey.HasPrimaryTarget.ToString(), true);
        Preconditions.Add(AIWorldStateKey.Aim.ToString(), true);


        Effects.Add(AIWorldStateKey.AssaultTarget.ToString(), true);
    }

    public override bool OnActionPerform()
    {
        SelectedThreatInfoData selectedThreatInfoData = blackBoardManager.GetOneDataByKey<SelectedThreatInfoData>(BlackBoardKey.SelectedPrimaryThreat);
        if (selectedThreatInfoData != null && selectedThreatInfoData.IsStillValid)
        {
            weaponControl.LookAtTarget(selectedThreatInfoData.ThreatTransform);
            weaponControl.Aim(true);
            weaponControl.FireWeapon(true);
        }

        return selectedThreatInfoData == null || ! selectedThreatInfoData.IsStillValid;
    }

    public override void OnActionComplete()
    {
        weaponControl.Aim(false);
        weaponControl.FireWeapon(false);
    }
}
