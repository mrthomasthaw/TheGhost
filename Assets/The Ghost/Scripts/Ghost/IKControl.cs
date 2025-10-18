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

        [SerializeField]
        private bool ikActive = false; // check if ik is active
        private Transform currentAimPivotObj, aimPivotR, aimPivotL;
        public Transform RightHandObj { get; private set; } // target for right hand
        public Transform LeftHandObj { get; private set; } // target for left hand
        private Transform lookObj = null; // target for head
        private Transform cameraT;
        private Transform rShoulderBoneT, lShoulderBoneT;
        private Transform aimTargetT;
        private Vector3 directionToAimTarget;
        private Quaternion aimPivotLookRotation;
        private bool death = false;

        [SerializeField]
        private DeathEventSO deathEventSO;

        [SerializeField]
        private float aimDirectionOffset;

        [SerializeField]
        private float aimYOffset;

        [SerializeField]
        private LayerMask targetableLayer;

        private float rHandWeight, lHandWeight;

        [SerializeField] private float weight, bWeight, hWeight; 

        private Vector3 aimPivotTargetDirection;

        private Vector3 ikLookPosition;

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
            //UpdateAimDirection(currentAimPivotObj);
            HandleDirectionForAim();
            HandleAimPosition();
            HandleChestLookAtPosition();
            HandleAimPivotRotation(currentAimPivotObj);
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
                    //if (lookObj != null)
                    //{
                    //    animator.SetLookAtWeight(0.3f, 0.3f, 1f);
                    //    //animator.SetLookAtWeight(0.3f);
                    //    animator.SetLookAtPosition(lookObj.position);
                    //}

                    hWeight = Mathf.Lerp(hWeight, 0f, Time.deltaTime * 10f);
                    bWeight = Mathf.Lerp(bWeight, 0f, Time.deltaTime * 10f);
                    weight = Mathf.Lerp(weight, 0, Time.deltaTime * 10f);

                    animator.SetLookAtWeight(weight, bWeight, hWeight);
                    animator.SetLookAtPosition(ikLookPosition);


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
            aimPivot.rotation = aimPivotLookRotation;
        }

        void UpdateAimDirection(Transform aimPivot)
        {
            Ray ray;

            if (UseCameraDirForAim)
            {
                ray = new Ray(cameraT.position, cameraT.forward);

                directionToAimTarget = ray.GetPoint(30);

                Vector3 lookDir = directionToAimTarget - aimPivot.position;
                lookDir.Normalize();

                Debug.DrawRay(aimPivot.position, lookDir, Color.red);

                Quaternion lookRot = Quaternion.LookRotation(lookDir);
                
                aimPivotLookRotation = Quaternion.Slerp(aimPivot.rotation, lookRot, Time.deltaTime * 600);
                //Debug.DrawRay(ray.origin, aimDirection, Color.red);
            }
            else
            {
                if (aimTargetT != null) 
                {
                    Vector3 targetDir = (aimTargetT.position - aimPivot.position).normalized;

                    // Scale vertical offset with distance (or some curve)
                    float verticalOffset = Vector3.Distance(aimTargetT.position, aimPivot.position) * -0.15f;

                    directionToAimTarget = aimTargetT.position + targetDir * aimDirectionOffset;
                    directionToAimTarget.y += verticalOffset;

                    Vector3 lookDir = (directionToAimTarget - aimPivot.position).normalized;

                    

                    Debug.DrawRay(aimPivot.position, (directionToAimTarget - aimPivot.position).normalized * 5f, Color.red);


                    Quaternion lookRot = Quaternion.LookRotation(lookDir);


                    aimPivotLookRotation = Quaternion.Slerp(aimPivot.rotation, lookRot, Time.deltaTime * 600);
                }
                
            }

            Debug.Log(currentAimPivotObj.name);

        }

        void HandleAimPivotRotation(Transform aimPivot)
        {
            Vector3 LookDir = aimPivotTargetDirection - aimPivot.position;

            Quaternion lookRot = Quaternion.LookRotation(LookDir);
            aimPivotLookRotation = Quaternion.Slerp(aimPivot.rotation, lookRot, Time.deltaTime * 700f);
        }

        void HandleDirectionForAim()
        {
            if (UseCameraDirForAim)
            {
                directionToAimTarget = cameraT.forward;
            }
            else
            {
                if (aimTargetT == null)
                {
                    directionToAimTarget = currentAimPivotObj.forward;
                    return;
                }

                Vector3 direction = (aimTargetT.position - currentAimPivotObj.position).normalized;
                direction.y += -0.01f;
                directionToAimTarget = direction;
            }

            
            Debug.DrawRay(currentAimPivotObj.position, directionToAimTarget * 1f, Color.magenta);
        }

        void HandleAimPosition()
        {
            Transform sourceTransform = UseCameraDirForAim ? cameraT : currentAimPivotObj;

            Ray aimHelperRay = new Ray(sourceTransform.position, directionToAimTarget);
            aimPivotTargetDirection = aimHelperRay.GetPoint(60);

            RaycastHit hit;

            bool canFireWeapon = true;

            Debug.DrawRay(sourceTransform.position, directionToAimTarget * 100, Color.yellow);


            bool hitObject = Physics.Raycast(aimHelperRay, out hit, 100f, targetableLayer);
            if (hitObject)
            {
                // layer 12 = bodyParts
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("bodyParts") && hit.collider.GetComponentInParent<IKControl>() == this)
                {
                    return;
                }

                float dist = Vector3.Distance(sourceTransform.position, hit.point);

                if (dist > 1f)
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("bodyParts"))
                    {
                        aimPivotTargetDirection = hit.point;
                    }
                }
                else
                {
                    canFireWeapon = false;
                    aimPivotTargetDirection = aimHelperRay.GetPoint(10) + Vector3.up * (-8f);
                }

            }
        }

        void HandleChestLookAtPosition()
        {
            Transform sourceTransform = UseCameraDirForAim ? cameraT : currentAimPivotObj;

            Vector3 lookDirection = directionToAimTarget + sourceTransform.up * (-0.2f);
            Ray spineLookHelperRay = new Ray(sourceTransform.position, lookDirection);

            ikLookPosition = spineLookHelperRay.GetPoint(30);

            Debug.DrawRay(sourceTransform.position, lookDirection * 100, Color.red);
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

        public void SetAimTargetTransform(Transform aimTarget)
        {
            this.aimTargetT = aimTarget;
        }

        public void SetIkActive(bool isActive)
        {
            this.ikActive = isActive;
        }

        public void OnDeath(GameObject sender)
        {
            if (sender == this.gameObject)
                death = true;
        }

    }
}

