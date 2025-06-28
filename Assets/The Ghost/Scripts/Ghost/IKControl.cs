using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MrThaw
{
    public class IKControl : MonoBehaviour
    {
        protected Animator animator; // The animator attached to the player

        public bool ikActive = false; // check if ik is active
        private Transform currentAimPivotObj, aimPivotR, aimPivotL;
        public Transform RightHandObj { get; private set; } // target for right hand
        public Transform LeftHandObj { get; private set; } // target for left hand
        private Transform lookObj = null; // target for head
        private Transform cameraT;
        private Transform rShoulderBoneT, lShoulderBoneT;
        private Transform aimTarget;
        private Vector3 aimDirection;
        private Quaternion aimPivotLookRotation;
        private bool death = false;

        [SerializeField]
        private DeathEventSO deathEventSO;

        [SerializeField]
        private float aimDirectionOffset;

        [SerializeField]
        private float aimYOffset;

        private float rHandWeight, lHandWeight;



        public bool UseCameraDirForAim;
        public float RHandWeight { get => rHandWeight; set => rHandWeight = value; }
        public float LHandWeight { get => lHandWeight; set => lHandWeight = value; }

        private void OnEnable()
        {
            deathEventSO.OnEventRaised += OnDeath;
        }

        private void OnDisable()
        {
            deathEventSO.OnEventRaised -= OnDeath;
        }

        void Awake()
        {
            animator = GetComponent<Animator>(); 

            aimPivotR = CommonUtil.FindDeepestChildByName(transform, "AimPivotR");
            aimPivotL = CommonUtil.FindDeepestChildByName(transform, "AimPivotL");

            
            currentAimPivotObj = aimPivotR;
            RightHandObj = currentAimPivotObj.GetChild(0);
            LeftHandObj = currentAimPivotObj.GetChild(1);

            rShoulderBoneT = animator.GetBoneTransform(HumanBodyBones.RightShoulder).transform;
            lShoulderBoneT = animator.GetBoneTransform(HumanBodyBones.LeftShoulder).transform;

            cameraT = Camera.main.transform;

        }

        private void Update()
        {
            if (death) return;
            UpdateAimDirection(currentAimPivotObj);
        }

        private void OnAnimatorMove()
        {
            if (death) return;
            HandleAimPivotObjRotationAndPosition(aimPivotR, rShoulderBoneT);
            HandleAimPivotObjRotationAndPosition(aimPivotL, lShoulderBoneT);
        }

        

        //a callback for calculating IK
        void OnAnimatorIK()
        {
            if (!death && animator)
            {

                //if the IK is active, set the position and rotation directly to the goal. 
                if (ikActive)
                {

                    // Set the look target position, if one has been assigned
                    if (lookObj != null)
                    {
                        animator.SetLookAtWeight(0.3f, 0.3f, 1f);
                        //animator.SetLookAtWeight(0.3f);
                        animator.SetLookAtPosition(lookObj.position);
                    }


                    // Set the right hand target position and rotation, if one has been assigned
                    if (RightHandObj != null)
                    {
                        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rHandWeight);
                        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rHandWeight);
                        animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandObj.position);
                        animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandObj.rotation);
                    }
                    if (LeftHandObj != null)
                    {
                        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, lHandWeight);
                        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, lHandWeight);
                        animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandObj.position);
                        animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandObj.rotation);
                    }

                }

                //if the IK is not active, set the position and rotation of the hand and head back to the original position
                else
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                    animator.SetLookAtWeight(0);
                }
            }
        }

      

        void HandleAimPivotObjRotationAndPosition(Transform aimPivot, Transform shoulderTransform)
        {
            aimPivot.position = shoulderTransform.position;

            //Vector3 lookDir = aimDirection - aimPivot.position;
            //lookDir.Normalize();

            //Debug.DrawRay(aimPivot.position, lookDir, Color.red);

            //Quaternion lookRot = Quaternion.LookRotation(lookDir);
            //aimPivot.rotation = Quaternion.Slerp(aimPivot.rotation, lookRot, Time.deltaTime * 600);
            aimPivot.rotation = aimPivotLookRotation;
        }

        void UpdateAimDirection(Transform aimPivot)
        {
            Ray ray;

            if (UseCameraDirForAim)
            {
                ray = new Ray(cameraT.position, cameraT.forward);

                aimDirection = ray.GetPoint(30);

                Vector3 lookDir = aimDirection - aimPivot.position;
                lookDir.Normalize();

                Debug.DrawRay(aimPivot.position, lookDir, Color.red);

                Quaternion lookRot = Quaternion.LookRotation(lookDir);
                
                aimPivotLookRotation = Quaternion.Slerp(aimPivot.rotation, lookRot, Time.deltaTime * 600);
                //Debug.DrawRay(ray.origin, aimDirection, Color.red);
            }
            else
            {
                if (aimTarget != null) 
                {
                    Vector3 targetDir = (aimTarget.position - aimPivot.position).normalized;

                    // Scale vertical offset with distance (or some curve)
                    float verticalOffset = Vector3.Distance(aimTarget.position, aimPivot.position) * -0.15f;

                    aimDirection = aimTarget.position + targetDir * aimDirectionOffset;
                    aimDirection.y += verticalOffset;

                    Vector3 lookDir = (aimDirection - aimPivot.position).normalized;

                    

                    Debug.DrawRay(aimPivot.position, (aimDirection - aimPivot.position).normalized * 5f, Color.red);


                    Quaternion lookRot = Quaternion.LookRotation(lookDir);


                    aimPivotLookRotation = Quaternion.Slerp(aimPivot.rotation, lookRot, Time.deltaTime * 600);
                }
                
            }

            Debug.Log(currentAimPivotObj.name);

        }
        

        public void SwitchAimPivot(ShoulderSetting shoulder)
        {
            if (shoulder == ShoulderSetting.Right)
            {
                currentAimPivotObj = aimPivotR;

            }
            else
            {
                currentAimPivotObj = aimPivotL;
            }

            RightHandObj = currentAimPivotObj.GetChild(0);
            LeftHandObj = currentAimPivotObj.GetChild(1);
        }

        public void SetLookObj(Transform t)
        {
            lookObj = t;
        }

        public void SetAimTarget(Transform aimTarget)
        {
            this.aimTarget = aimTarget;
        }

        public void OnDeath(GameObject sender)
        {
            if (sender == this.gameObject)
                death = true;
        }

    }
}

