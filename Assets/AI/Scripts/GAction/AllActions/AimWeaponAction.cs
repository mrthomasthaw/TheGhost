using MrThaw;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimWeaponAction : GAction
{
    private Transform transform;
    private AIAnimationControl animationControl;
    private AIWeaponControl weaponControl;
    private SelectedThreatInfoData selectedThreatInfoData;

    public AimWeaponAction(AIAnimationControl animationControl, AIWeaponControl weaponControl, Transform transform)
    {
        this.animationControl = animationControl;
        this.transform = transform;
        this.weaponControl = weaponControl;
    }

    public override void SetUp(BlackBoardManager blackBoardManager)
    {
        base.SetUp(blackBoardManager);
        Preconditions.Add(AIWorldStateKey.HasPrimaryTarget.ToString(), true);
        Preconditions.Add(AIWorldStateKey.AimWeapon.ToString(), false);
        RequiredStatesToComplete = true;

        Effects.Add(AIWorldStateKey.AimWeapon.ToString(), true);
    }

    public override void OnActionStart()
    {
        Debug.Log("blackBoar " + blackBoardManager);
        selectedThreatInfoData = blackBoardManager.GetOneDataByKey<SelectedThreatInfoData>(BlackBoardKey.SelectedPrimaryThreat);
        if (selectedThreatInfoData != null && selectedThreatInfoData.IsStillValid) 
        {
            weaponControl.AimAtTarget(true, selectedThreatInfoData.ThreatTransform);
            animationControl.AnimateSingleAction(AnimationKey.AimWeapon);
        }
        else
        {
            AbortAction = true;
        }
    }

    public override bool OnActionPerform()
    {
        weaponControl.AimAtTarget(true, selectedThreatInfoData.ThreatTransform);
        return animationControl.IsAnimationFinished(AnimationKey.AimWeapon);
    }

    public override void OnActionComplete()
    {
        if (!selectedThreatInfoData.IsStillValid)
        {
            weaponControl.AimAtTarget(false, null);
        }

        selectedThreatInfoData = null;
    }

    
}
