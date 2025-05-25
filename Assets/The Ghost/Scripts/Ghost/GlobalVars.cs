using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw
{
    public class GlobalVars : MonoBehaviour
    {
        AnimateMovement AnimateMovement { get; set; }

        WeaponPositionControl WPControl { get; set; }

        public float CharacterHorizontalSpeed { get; set; }
        public float CharacterVerticalSpeed { get; set; }

        public ShoulderSetting ShoulderSetting { get; set; }

        private void Start()
        {
            AnimateMovement = GetComponent<AnimateMovement>();
            WPControl = GetComponent<WeaponPositionControl>();
        }

        private void Update()
        {
            ReadCharacterMoveSpeed();
            ReadCharacterShoulderSide();
        }
        

        void ReadCharacterMoveSpeed()
        {
            CharacterHorizontalSpeed = AnimateMovement.H;
            CharacterVerticalSpeed = AnimateMovement.V;
        }

        void ReadCharacterShoulderSide()
        {
            ShoulderSetting = WPControl.Shoulder;
        }
    }
}
