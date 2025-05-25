using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponChangeAnimationEvent : MonoBehaviour
{
    Animator animator;

    [HideInInspector]
    public WeaponEquipSettingData weaponEquipSettingData;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnWeaponChangeFinished()
    {
        switch(weaponEquipSettingData.weaponEquipState)
        {
            case WeaponEquipState.Disarmed:
                animator.CrossFadeInFixedTime("Armed Locomotion", 0.2f);
                weaponEquipSettingData.weaponEquipState = WeaponEquipState.Armed;
                break;
            case WeaponEquipState.Armed:
                animator.CrossFadeInFixedTime("Disarmed Locomotion", 0.2f);
                weaponEquipSettingData.weaponEquipState = WeaponEquipState.Disarmed;
                break;
        }
    }
}
