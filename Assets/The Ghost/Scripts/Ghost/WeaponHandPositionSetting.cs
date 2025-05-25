using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon Hand Position Setting", menuName ="ScriptableObjects/WeaponHandPositionSetting", order = 1)]
public class WeaponHandPositionSetting : ScriptableObject
{
    [System.Serializable]
    public class ShoulderSide
    {
        [Header("-Right Hand-")]
        public Vector3 rightHandIdlePos;
        public Vector3 rightHandIdleEulerAngle;
        public Vector3 rightHandAimPos;
        public Vector3 rightHandAimEulerAngle;

        [Header("-Left Hand-")]
        public Vector3 leftHandIdlePos;
        public Vector3 leftHandIdleEulerAngle;
        public Vector3 leftHandAimPos;
        public Vector3 leftHandAimEulerAngle;
    }

    [System.Serializable]
    public class WeaponEquipSetting
    {
        public Vector3 equipHandPosition;
        public Vector3 equipHandEulerAngle;
    }

    [SerializeField]
    public WeaponEquipSetting rightWeaponEquipSetting;

    [SerializeField]
    public WeaponEquipSetting leftWeaponEquipSetting;

    [SerializeField]
    public ShoulderSide rightShoulderSide;

    [SerializeField]
    public ShoulderSide leftShoulderSide;



}
