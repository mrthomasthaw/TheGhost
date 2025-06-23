
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MrThaw
{
    public class WeaponInvControl : MonoBehaviour
    {
        [SerializeField]
        public WeaponInventoryData inventoryData;

        [SerializeField]
        private DeathEventSO deathEventSO;

        private WeaponInventory weaponInventory;

        private WeaponPositionControl weaponPositionControl;

        private CameraWeaponRecoilHandler cameraWeaponRecoilHandler;

        private Animator animator;

        private AimInputHelper aimInputHelper;

        private bool enableAim;

        private bool death;

        public bool debugAim;

        public bool lockHand;

        private void OnEnable()
        {
            deathEventSO.OnEventRaised += OnDeath;
        }

        private void OnDisable()
        {
            deathEventSO.OnEventRaised += OnDeath;
        }


        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
            weaponPositionControl = GetComponent<WeaponPositionControl>();
            weaponPositionControl.IKControl.SetLookObj(Camera.main.transform.GetChild(0));
            weaponInventory = new WeaponInventory(animator, transform, CommonUtil.FindDeepestChildByName(transform, "WeaponUnEquipPointHandGun"), animator.GetBoneTransform(HumanBodyBones.LeftHand),
                animator.GetBoneTransform(HumanBodyBones.RightHand), CommonUtil.FindDeepestChildByName(transform, "AimPivotL"), CommonUtil.FindDeepestChildByName(transform, "AimPivotR"),
                GetComponent<WeaponPositionControl>(), inventoryData);

            weaponInventory.SetUp();
            weaponPositionControl.IKControl.UseCameraDirForAim = true;

            cameraWeaponRecoilHandler = FindObjectOfType<CameraWeaponRecoilHandler>();
            aimInputHelper = new AimInputHelper();
            aimInputHelper.DebugAim = debugAim;

            SetUpListenersForAllWeapon();
        }

        // Update is called once per frame
        void Update()
        {
            if (death) return;

            if (Input.GetButtonDown("NextWeapon"))
                weaponInventory.SwitchWeapon();

            if (Input.GetButtonDown("ShoulderChange"))
            {
                weaponPositionControl.HandleShoulderChange();
                weaponInventory.SwitchShoulder();
            }

            if (debugAim) 
            {
                enableAim = true;
            }
            else
            {
                aimInputHelper.HandleHipFireAndAimInputs(ref enableAim, Time.deltaTime);
            }


            weaponPositionControl.HandleWeaponAim(enableAim);
            weaponInventory.CurrentWeapon.Shoot(Input.GetButton("Shoot"));
        }

        public void SetUpListenersForAllWeapon()
        {
            weaponInventory.AllWeaponList.ForEach(w =>
            {
                if (w.onWeaponShoot == null || w.onWeaponShoot != null 
                && !w.onWeaponShoot.GetInvocationList().Contains((Action)cameraWeaponRecoilHandler.CalculateRecoil))
                {
                    //w.onWeaponShoot += StartRecoil;
                    w.onWeaponShoot += cameraWeaponRecoilHandler.CalculateRecoil;
                }
            });
        }

        public void OnDeath(GameObject sender)
        {
            if (sender == this.gameObject)
                death = true;
        }
    }
}
