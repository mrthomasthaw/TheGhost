using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw
{
    public class WeaponInventory
    {

        [SerializeField] private Dictionary<string, Weapon[]> weaponRightAndLeftPairs = new Dictionary<string, Weapon[]>();
        [SerializeField] private Weapon[] currentWeaponPairs = new Weapon[2];



        [SerializeField] private Transform unEquipPointHandGun, unEquipPointRifle;

        [SerializeField] private Transform leftHandBone, rightHandBone;




        [SerializeField] private int slot;

        [SerializeField] private Transform aimPivotR, aimPivotL;

        private Transform m_transform;

        [SerializeField] private Weapon _currentWeapon;

        private Animator animator;

        public Weapon CurrentWeapon
        {
            get { return _currentWeapon; }
            private set { _currentWeapon = value; } // Private setter
        }

        public List<Weapon> AllWeaponList { get; private set; }

        private WeaponPositionControl weaponPositionControl;


        private ShoulderSetting shoulderSetting;

        private WeaponInventoryData inventoryData;


        public WeaponInventory(Animator animator, 
            Transform transform, Transform weaponUnEquipT, Transform leftHandT, Transform rightHandT, 
            Transform aimPivotL, Transform aimPivotR, WeaponPositionControl weaponPositionControl, WeaponInventoryData inventoryData)
        {
            this.m_transform = transform;
            this.unEquipPointHandGun = weaponUnEquipT;
            this.leftHandBone = leftHandT;
            this.rightHandBone = rightHandT;
            this.aimPivotL = aimPivotL;
            this.aimPivotR = aimPivotR;
            this.animator = animator;
            this.weaponPositionControl = weaponPositionControl;
            this.inventoryData = inventoryData;
        }

        public void SetUp()
        {
            AllWeaponList = new List<Weapon>();
            InstantiateWeaponPrefabs();
            CheckCurrentWeaponToEquip();
        }


        public void SwitchWeapon()
        {
            if (slot < inventoryData.availableWeaponNames.Count - 1)
                slot++;
            else
                slot = 0;

            inventoryData.currentWeaponName = inventoryData.availableWeaponNames[slot];

            UnEquipWeapon();
            EquipWeapon();
        }

        private void InstantiateWeaponPrefabs()
        {
            for (int x = 0; x < inventoryData.weaponPrefabs.Length; x++)
            {
                if(inventoryData.availableWeaponNames.Contains(inventoryData.weaponPrefabs[x].name))
                {
                    Weapon weaponRight = MonoBehaviour.Instantiate(inventoryData.weaponPrefabs[x], m_transform.position, Quaternion.identity);
                    Weapon weaponLeft = MonoBehaviour.Instantiate(inventoryData.weaponPrefabs[x], m_transform.position, Quaternion.identity);

                    weaponRight.transform.SetParent(unEquipPointHandGun);
                    weaponLeft.transform.SetParent(unEquipPointHandGun);

                    weaponRight.name = weaponRight.name.Replace("(Clone)", "");
                    weaponLeft.name = weaponLeft.name.Replace("(Clone)", "");


                    weaponRightAndLeftPairs.Add(inventoryData.weaponPrefabs[x].name, new Weapon[] { weaponRight, weaponLeft });

                    weaponRight.transform.position = unEquipPointHandGun.position;
                    weaponRight.transform.rotation = unEquipPointHandGun.rotation;

                    weaponLeft.transform.position = unEquipPointHandGun.position;
                    weaponLeft.transform.rotation = unEquipPointHandGun.rotation;

                    AllWeaponList.Add(weaponLeft);
                    AllWeaponList.Add(weaponRight);

                }
                
            }
        }

        private void CheckCurrentWeaponToEquip()
        {
            for (int x = 0; x < inventoryData.availableWeaponNames.Count; x++)
            {
                if(inventoryData.currentWeaponName == inventoryData.availableWeaponNames[x])
                {
                    EquipWeapon();
                }
            }
        }

        private void EquipWeapon()
        {
            //Debug.Log("Equip");

            Weapon[] weaponPair = weaponRightAndLeftPairs[inventoryData.currentWeaponName];

            currentWeaponPairs = weaponRightAndLeftPairs[inventoryData.currentWeaponName];

            Weapon rightWeapon = weaponPair[0], leftWeapon = weaponPair[1];

            rightWeapon.transform.SetParent(rightHandBone);
            leftWeapon.transform.SetParent(leftHandBone);

            rightWeapon.Equip = true;
            leftWeapon.Equip = true;

            rightWeapon.transform.localPosition = rightWeapon.WeaponHandPositionSetting.rightWeaponEquipSetting.equipHandPosition;
            rightWeapon.transform.localEulerAngles = rightWeapon.WeaponHandPositionSetting.rightWeaponEquipSetting.equipHandEulerAngle;
            leftWeapon.transform.localPosition = leftWeapon.WeaponHandPositionSetting.leftWeaponEquipSetting.equipHandPosition;
            leftWeapon.transform.localEulerAngles = leftWeapon.WeaponHandPositionSetting.leftWeaponEquipSetting.equipHandEulerAngle;


            rightWeapon.WeaponRecoil.SetUpHandIkObj(aimPivotR, aimPivotR.GetChild(0));
            leftWeapon.WeaponRecoil.SetUpHandIkObj(aimPivotL, aimPivotL.GetChild(1));

            weaponPositionControl.UpdateLeftAndRightWeapon(leftWeapon, rightWeapon);
            weaponPositionControl.UpdateCurrentWeapon();


            int indexToDisable = shoulderSetting == ShoulderSetting.Left ? 0 : 1;

            weaponPair[indexToDisable].gameObject.SetActive(false);

            int indexToCurrentWeapon = shoulderSetting == ShoulderSetting.Left ? 1 : 0;
            CurrentWeapon = weaponPair[indexToCurrentWeapon];
        }

        private void UnEquipWeapon()
        {
            for(int x = 0; x < currentWeaponPairs.Length; x++)
            {
                currentWeaponPairs[x].transform.SetParent(unEquipPointHandGun);

                currentWeaponPairs[x].Equip = false;

                currentWeaponPairs[x].gameObject.SetActive(true);
    
                currentWeaponPairs[x].transform.position = unEquipPointHandGun.position;
                currentWeaponPairs[x].transform.rotation = unEquipPointHandGun.rotation;

            }
        }

        public void SwitchShoulder()
        {
            shoulderSetting = shoulderSetting == ShoulderSetting.Left ? ShoulderSetting.Right : ShoulderSetting.Left;
            Weapon[] weaponPair = weaponRightAndLeftPairs[inventoryData.currentWeaponName];

            int index = shoulderSetting == ShoulderSetting.Left ? 1 : 0;
            CurrentWeapon = weaponPair[index];
        }

    }

    [System.Serializable]
    public class WeaponInventoryData
    {
        public Weapon[] weaponPrefabs;
        public List<string> availableWeaponNames;

        public string currentWeaponName;
    }
}
