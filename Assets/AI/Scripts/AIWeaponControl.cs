using MrThaw;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIWeaponControl : MonoBehaviour
{
    private Animator animator;
    private WeaponPositionControl weaponPositionControl;
    private WeaponInventory weaponInventory;

    [SerializeField]
    private WeaponInventoryData weaponInventoryData;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();;

        weaponPositionControl = GetComponent<WeaponPositionControl>();
        weaponPositionControl.IKControl.SetLookObj(CommonUtil.FindDeepestChildByName(transform, "LookObj"));

        weaponInventory = new WeaponInventory(animator, transform, CommonUtil.FindDeepestChildByName(transform, "WeaponUnEquipPointHandGun"), animator.GetBoneTransform(HumanBodyBones.LeftHand),
                animator.GetBoneTransform(HumanBodyBones.RightHand), CommonUtil.FindDeepestChildByName(transform, "AimPivotL"), CommonUtil.FindDeepestChildByName(transform, "AimPivotR"),
                GetComponent<WeaponPositionControl>(), weaponInventoryData);

        weaponInventory.SetUp();
    }

    public void Aim(bool aim)
    {
        weaponPositionControl.HandleWeaponAim(aim);
    }

    public void FireWeapon(bool fire)
    {
        weaponInventory.CurrentWeapon.Shoot(fire);
    }

    public void LookAtTarget(Transform threatT)
    {
        Vector3 threatPos = threatT.position;
        Vector3 origin = transform.position;

        Vector3 dir = threatPos - origin;
        dir.Normalize();

        Quaternion lookRot = Quaternion.LookRotation(dir);
        Quaternion smoothRotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * 12f);
        smoothRotation.z = 0;
        smoothRotation.x = 0;
        transform.rotation = smoothRotation;
    }

}
