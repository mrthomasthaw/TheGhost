using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw
{
    public class AimInputHelper
    {
        private float hipFireTimer;

        private CharacterAimState aimState;

        public delegate void OnAim();
        public OnAim onAim;

        public delegate void OnPutDownWeapon();
        public OnPutDownWeapon onPutDownWeapon;

        //public void HandleHipFireAndAimInputs(ref bool aim)
        //{
        //    if (Input.GetButton("Shoot") && !Input.GetButton("PreciseAim")) // Hip Fire 
        //    {
        //        hipFireTimer = 2f;
        //    }

        //    if (hipFireTimer > 0)
        //    {
        //        hipFireTimer -= Time.deltaTime;
        //        aim = true;
        //        onAim?.Invoke();
        //    }

        //    if (Input.GetButton("PreciseAim"))
        //    {
        //        hipFireTimer = 0f;
        //        aim = true;
        //        onAim?.Invoke();
        //    }
        //    else if (hipFireTimer <= 0)
        //    {
        //        aim = false;
        //        onPutDownWeapon?.Invoke();
        //    }
        //}

        public void HandleHipFireAndAimInputs(ref bool aim, float deltaTime)
        {
            switch (aimState)
            {
                case CharacterAimState.Idle:
                    HandleIdleState(ref aim, ref this.aimState);
                    break;
                case CharacterAimState.HipFire:
                    HandleHipFireState(ref aim, ref this.aimState, deltaTime);
                    break;
                case CharacterAimState.PreciseAim:
                    HandlePreciseAimState(ref aim, ref this.aimState);
                    break;
            }
        }


        public void HandleHipFireAndAimInputs(ref bool aim, ref CharacterAimState characterAimState, float deltaTime)
        {
            switch (characterAimState)
            {
                case CharacterAimState.Idle:
                    HandleIdleState(ref aim, ref characterAimState);
                    break;
                case CharacterAimState.HipFire:
                    HandleHipFireState(ref aim, ref characterAimState, deltaTime);
                    break;
                case CharacterAimState.PreciseAim:
                    HandlePreciseAimState(ref aim, ref characterAimState);
                    break;
            }
        }




        private void HandleIdleState(ref bool aim, ref CharacterAimState _aimState)
        {
            if (Input.GetButton("Shoot") && !Input.GetButton("PreciseAim")) // Hip Fire 
            {
                hipFireTimer = 2f;
                aim = true;
                onAim?.Invoke();
                _aimState = CharacterAimState.HipFire;
            }
            else if (Input.GetButton("PreciseAim"))
            {
                hipFireTimer = 0f;
                aim = true;
                onAim?.Invoke();
                _aimState = CharacterAimState.PreciseAim;
            }
        }

        private void HandleHipFireState(ref bool aim, ref CharacterAimState _aimState, float deltaTime)
        {
            if (hipFireTimer > 0)
            {
                hipFireTimer -= deltaTime;
                aim = true;
            }
            else
            {
                aim = false;
                onPutDownWeapon?.Invoke();
                _aimState = CharacterAimState.Idle;
            }

            if (Input.GetButton("Shoot") && !Input.GetButton("PreciseAim")) // HipFire
            {
                hipFireTimer = 2f;
            }
            else if(Input.GetButton("PreciseAim"))
            {
                hipFireTimer = 0f;
                _aimState = CharacterAimState.PreciseAim;
            }
        }

        private void HandlePreciseAimState(ref bool aim, ref CharacterAimState _aimState)
        {
            if (! Input.GetButton("PreciseAim"))
            {
                aim = false;
                onPutDownWeapon?.Invoke();
                _aimState = CharacterAimState.Idle;
            }
        }
    }

}
