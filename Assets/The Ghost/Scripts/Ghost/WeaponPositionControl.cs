using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw
{
    public class WeaponPositionControl : MonoBehaviour
    {
        public Transform rightHandObj, leftHandObj;
        public Transform secondHandHoldingPoint;



        public Weapon RightWeapon { get; set; }
        public Weapon LeftWeapon { get; set; }

        public ShoulderSetting Shoulder
        {
            get { return shoulder; }
        }

        private Transform rightWeaponT, leftWeaponT;

        [SerializeField]
        private DeathEventSO deathEventSO;

        [SerializeField] public ShoulderSetting shoulder;

        [SerializeField]
        private WeaponHandPositionSetting _currentWeaponHandPositionSetting;

        public WeaponHandPositionSetting CurrentWeaponHandPositionSetting
        {
            get { return _currentWeaponHandPositionSetting; }
        }

        public IKControl IKControl 
        {
            get
            {
                if (iKControl == null)
                    return GetComponent<IKControl>();

                return iKControl;
            }
        }

        private CharacterAimState characterActionState;

        private IKControl iKControl;

        private bool aim;

        private bool death;

        public bool IkActive;

        private void OnEnable()
        {
            deathEventSO.OnEventRaised += OnDeath;
        }

        private void OnDisable()
        {
            deathEventSO.OnEventRaised -= OnDeath;
        }

        // Start is called before the first frame update
        void Start()
        {
            rightHandObj = CommonUtil.FindDeepestChildByName(transform, "Right Hand Obj").transform;
            leftHandObj = CommonUtil.FindDeepestChildByName(transform, "Left Hand Obj").transform;
            iKControl = GetComponent<MrThaw.IKControl>();
        }

        // Update is called once per frame
        void Update()
        {
       
            if (death || ! IkActive) 
            {
                return;
            }

            switch (shoulder)
            {
                case ShoulderSetting.Right:
                    HandleHandsPositionRightSide();
                    break;
                case ShoulderSetting.Left:
                    HandleHandsPositionLeftSide();
                    break;
            }
        }

        private void HandleHandsPositionLeftSide()
        {
            if (_currentWeaponHandPositionSetting == null || secondHandHoldingPoint == null)
                return;


            leftHandObj.localPosition = _currentWeaponHandPositionSetting.leftShoulderSide.leftHandAimPos;
            leftHandObj.localEulerAngles = _currentWeaponHandPositionSetting.leftShoulderSide.leftHandAimEulerAngle;

            rightHandObj.position = secondHandHoldingPoint.position;
            rightHandObj.rotation = secondHandHoldingPoint.rotation;

            secondHandHoldingPoint.localPosition = _currentWeaponHandPositionSetting.leftShoulderSide.rightHandAimPos;
            secondHandHoldingPoint.localEulerAngles = _currentWeaponHandPositionSetting.leftShoulderSide.rightHandAimEulerAngle;
        }

        private void HandleHandsPositionRightSide()
        {
            if (_currentWeaponHandPositionSetting == null || secondHandHoldingPoint == null)
                return;


            rightHandObj.localPosition = _currentWeaponHandPositionSetting.rightShoulderSide.rightHandAimPos;
            rightHandObj.localEulerAngles = _currentWeaponHandPositionSetting.rightShoulderSide.rightHandAimEulerAngle;


            leftHandObj.position = secondHandHoldingPoint.position;
            leftHandObj.rotation = secondHandHoldingPoint.rotation;


            secondHandHoldingPoint.localPosition = _currentWeaponHandPositionSetting.rightShoulderSide.leftHandAimPos;
            secondHandHoldingPoint.localEulerAngles = _currentWeaponHandPositionSetting.rightShoulderSide.leftHandAimEulerAngle;
        }



        private void SwitchHands()
        {
            rightHandObj = iKControl.RightHandObj;
            leftHandObj = iKControl.LeftHandObj;
        }

        public void UpdateLeftAndRightWeapon(Weapon left, Weapon right)
        {
            RightWeapon = right;
            LeftWeapon = left;

            rightWeaponT = RightWeapon.transform;
            leftWeaponT = LeftWeapon.transform;
        }

        public void UpdateCurrentWeapon()
        {
            if (shoulder == ShoulderSetting.Left)
            {
                _currentWeaponHandPositionSetting = LeftWeapon.WeaponHandPositionSetting;
                secondHandHoldingPoint = LeftWeapon.SecondHandHoldingPoint;
            }
            else
            {
                _currentWeaponHandPositionSetting = RightWeapon.WeaponHandPositionSetting;
                secondHandHoldingPoint = RightWeapon.SecondHandHoldingPoint;
            }
        }

        public void HandleShoulderChange()
        {
            shoulder = shoulder == ShoulderSetting.Right ? ShoulderSetting.Left : ShoulderSetting.Right;

            iKControl.SwitchAimPivot(shoulder);

            if (shoulder == ShoulderSetting.Right)
            {
                SwitchHands();
                secondHandHoldingPoint = rightWeaponT.GetChild(1);
                rightWeaponT.gameObject.SetActive(true);
                leftWeaponT.gameObject.SetActive(false);
            }
            else
            {

                SwitchHands();
                secondHandHoldingPoint = leftWeaponT.GetChild(1);
                leftWeaponT.gameObject.SetActive(true);
                rightWeaponT.gameObject.SetActive(false);
            }
        }

        //This meant to be called in every frame
        public void HandleWeaponAim(bool input)
        {
            aim = input;

            if(aim)
            {
                if (shoulder == ShoulderSetting.Right)
                {
                    iKControl.RHandWeight = Mathf.Lerp(iKControl.RHandWeight, 1, Time.deltaTime * 14f);
                    iKControl.LHandWeight = 1f;
                }
                else
                {
                    iKControl.LHandWeight = Mathf.Lerp(iKControl.LHandWeight, 1, Time.deltaTime * 14f);
                    iKControl.RHandWeight = 1f;
                }
                    
            }
            else
            {
                if (shoulder == ShoulderSetting.Right)
                {
                    iKControl.RHandWeight = Mathf.Lerp(iKControl.RHandWeight, 0, Time.deltaTime * 20f);
                    iKControl.LHandWeight = 1f;
                }
                else
                {
                    iKControl.LHandWeight = Mathf.Lerp(iKControl.LHandWeight, 0, Time.deltaTime * 20f);
                    iKControl.RHandWeight = 1f;
                }                   
            }
        }

        public void OnDeath(GameObject sender)
        {
            if (sender == this.gameObject)
                death = true;
        }

    }

}
