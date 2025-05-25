using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MrThaw
{
    public class CrouchControl : MonoBehaviour
    {
        Animator animator;
        AnimateMovement animateMovement;
        AimInputHelper aimInputHelper;

        float v, h;
        float inputMagnitude;
        bool crouch;
        bool aim;

        [SerializeField] private CrouchEventSO crouchEvent;
        [SerializeField] private ShoulderSetting shoulderSetting;

        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
            animateMovement = GetComponent<AnimateMovement>();
            aimInputHelper = new AimInputHelper();
        }

        // Update is called once per frame
        void Update()
        {
            aimInputHelper.HandleHipFireAndAimInputs(ref aim, Time.deltaTime);

            if (Input.GetButtonDown("ShoulderChange"))
                HandleShoulderChange();

            if (Input.GetButtonDown("Crouch"))
            {
                crouch = !crouch;
                crouchEvent.RaiseEvent(this.gameObject, crouch);


                if (crouch)
                {
                    //DisableScripts();

                    if (shoulderSetting == ShoulderSetting.Left)
                    {
                        animator.CrossFadeInFixedTime("Rifle_CrouchLoop L", 0.4f);
                    }
                    else
                    {
                        animator.CrossFadeInFixedTime("Rifle_CrouchLoop R", 0.4f);
                    }
                }
                else
                {
                    //EnableScripts();

                    if (aim && shoulderSetting == ShoulderSetting.Left)
                    {
                        animator.CrossFadeInFixedTime("Weapon Locomotion Left Shoulder", 0.4f);
                    }
                    else if (aim && shoulderSetting == ShoulderSetting.Right)
                    {
                        animator.CrossFadeInFixedTime("Weapon Locomotion Right Shoulder", 0.4f);
                    }
                    else if (shoulderSetting == ShoulderSetting.Right)
                    {
                        animator.CrossFadeInFixedTime("Default Locomotion Right Shoulder", 0.4f);
                    }
                    else if (shoulderSetting == ShoulderSetting.Left)
                    {
                        animator.CrossFadeInFixedTime("Default Locomotion Left Shoulder", 0.4f);
                    }
                }

            }

            HandleCrouchState();

        }

        private void HandleCrouchState()
        {
            if (crouch)
            {
                h = Input.GetAxis("Horizontal");
                v = Input.GetAxis("Vertical");

                inputMagnitude = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));
                animator.SetFloat("Input Magnitude", inputMagnitude, 0.1f, Time.deltaTime);
                animator.SetFloat("Vertical", v, 0.08f, Time.deltaTime);
                animator.SetFloat("Horizontal", h, 0.08f, Time.deltaTime);
            }
        }

        public void HandleShoulderChange()
        {
            shoulderSetting = shoulderSetting == ShoulderSetting.Right ? ShoulderSetting.Left : ShoulderSetting.Right;

            if (crouch)
            {

                if (shoulderSetting == ShoulderSetting.Left)
                {
                    animator.CrossFadeInFixedTime("Rifle_CrouchLoop L", 0.4f);
                }
                else
                {
                    animator.CrossFadeInFixedTime("Rifle_CrouchLoop R", 0.4f);
                }
            }
        }

        void EnableScripts()
        {
            if (animateMovement)
                animateMovement.enabled = true;
        }

        void DisableScripts()
        {
            if(animateMovement)
                animateMovement.enabled = false;
        }
    }
}