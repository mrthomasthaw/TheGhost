
using UnityEngine;

namespace MrThaw
{
    /// <summary>
    /// Uses Humanoid IK system of unity to look at a position or to a transform
    /// </summary>
    public class AIStateSystemHumanLookIK : AIStateSystem
    {
        private Vector3 lookAtPosition;
        private float headIKTarget;
        private Transform lookAtTransform;
        private float headAim;
        private float currentReferenceAngle = 0;
        private Vector2 tAngles; // Horizontal & Vertical
        private Vector3 realLookPos;
        private Vector3 headPos;
        private Transform Transform;
        private Animator animator;
        public HumanLookIKProps cLookProps;
        private bool lookEnabled;

        private float horizontalLookAnglePlus;
        private float verticalLookAnglePlus;

        public bool headOnlyLook { get; private set; }

        public AIStateSystemHumanLookIK(HumanLookIKProps _lookIKProps, Animator _animator, Transform transform)
        {
            cLookProps = _lookIKProps;
            animator = _animator;
            this.Transform = transform;
        }

        public override void OnStart(AIBlackBoard blackBoard)
        {
        }

        public override void OnActionActivate(AIBlackBoard blackBoard)
        {
            AIBBDLookAt lookAtBBD = blackBoard.GetBBData<AIBBDLookAt>();
            SetLookAtToStartLook(lookAtBBD, blackBoard);
        }

        public void SetLookAtToStartLook(AIBBDLookAt lookAtBBD, AIBlackBoard blackboard)
        {
            if (lookAtBBD != null)
            {
                headIKTarget = 1;
                lookEnabled = true;
                headOnlyLook = lookAtBBD.HeadOnlyLook;

                if (lookAtBBD.GetType() == typeof(AIBBDLookAtPosition))
                {
                    var bbd = lookAtBBD as AIBBDLookAtPosition;
                    lookAtPosition = bbd.Position;
                }
                else if (lookAtBBD.GetType() == typeof(AIBBDLookAtTransform))
                {
                    var bbd = lookAtBBD as AIBBDLookAtTransform;
                    lookAtTransform = bbd.Transform;
                }

                blackboard.RemoveBBData(lookAtBBD);
            }
        }

        public void SetReferenceAngle(float _referenceAngle)
        {
            currentReferenceAngle = _referenceAngle;
        }

        public void SetAdditionalAngles(Vector2 additionalVertHorAngle)
        {
            verticalLookAnglePlus = additionalVertHorAngle.x;
            horizontalLookAnglePlus = additionalVertHorAngle.y;
        }

        public override void OnUpdate(AIBlackBoard blackBoard)
        {
            if (lookEnabled || (!lookEnabled && !AtMinEpsilon(headAim)))
            {
                if (lookAtTransform)
                    lookAtPosition = lookAtTransform.position;

                headAim = Mathf.Lerp(CheckEpsilon(headAim, headIKTarget), headIKTarget,
                    (headIKTarget == 1 ? cLookProps.headIKSmooth : cLookProps.headIKBackSmooth) * Time.deltaTime);
                headPos = animator.GetBoneTransform(HumanBodyBones.Head).position;

                Vector3 fromHeadToLookAtNoXRot = (-headPos + new Vector3(lookAtPosition.x, headPos.y, lookAtPosition.z)).normalized;
                Vector3 refDir = Quaternion.Euler(0, currentReferenceAngle, 0) * Vector3.forward;
                Vector3 mPosToCDir = headPos.y * (Quaternion.AngleAxis(tAngles.x, Vector3.up) * Transform.TransformDirection(refDir));
                Vector3 mPosToZeroDir = headPos.y * (Quaternion.AngleAxis(0, Vector3.up) * Transform.TransformDirection(refDir));
                float targetAngleHorizontal = Vector3.Angle(fromHeadToLookAtNoXRot, mPosToZeroDir);
                targetAngleHorizontal = targetAngleHorizontal * Mathf.Sign(Vector3.Dot(Quaternion.AngleAxis(90, Vector3.up) * mPosToZeroDir, fromHeadToLookAtNoXRot));

                float bottomEdgeSize = Vector3.Distance(headPos, new Vector3(lookAtPosition.x, headPos.y, lookAtPosition.z));
                Vector3 lookPosWOY = headPos + mPosToCDir.normalized * bottomEdgeSize;

                Vector3 fromHeadToLookAt = (lookAtPosition - headPos).normalized;

                float targetAngleVertical = Vector3.Angle(fromHeadToLookAt, fromHeadToLookAtNoXRot);

                float dot = Mathf.Sign(Vector3.Dot(fromHeadToLookAt, -Vector3.up));

                float plusYHeadPos = -bottomEdgeSize * Mathf.Sin(tAngles.y * Mathf.Deg2Rad) / Mathf.Sin((90 - tAngles.y) * Mathf.Deg2Rad);
                targetAngleVertical *= dot;

                if (headIKTarget == 0)
                {
                    tAngles = Vector2.Lerp(tAngles, Vector2.zero, Time.deltaTime * cLookProps.angleLerpBackSpeed);
                }
                else
                {
                    tAngles = Vector2.Lerp(tAngles,
                        new Vector2(targetAngleHorizontal + horizontalLookAnglePlus,
                        targetAngleVertical + verticalLookAnglePlus),
                        Time.deltaTime * cLookProps.angleLerpSpeed);
                }

                tAngles.x = Mathf.Clamp(tAngles.x, -cLookProps.maxLookTAngleHorizontal, cLookProps.maxLookTAngleHorizontal);
                tAngles.y = Mathf.Clamp(tAngles.y, -cLookProps.maxLookTAngleVertical, cLookProps.maxLookTAngleVertical);

                realLookPos = lookPosWOY + Vector3.up * plusYHeadPos;
            }
            else if (AtMinEpsilon(headAim) && !lookEnabled)
            {
                headAim = 0;
            }
        }

        public override void OnAnimatorIK(AIBlackBoard blackBoard, int layerIndex)
        {
            animator.SetLookAtPosition(realLookPos);
            if (headOnlyLook)
                animator.SetLookAtWeight(headAim);
            else
                animator.SetLookAtWeight(headAim, cLookProps.slawBodyWeight, cLookProps.slawHeadWeight, 0, cLookProps.slawClamp);
        }

        public override void OnActionExit(AIBlackBoard blackBoard)
        {
            blackBoard.RemoveFirstBBData<AIBBDLookAt>();
            // Reset
            lookAtTransform = null;
            //currentReferenceAngle = 0;
            headIKTarget = 0;
            verticalLookAnglePlus = 0;
            horizontalLookAnglePlus = 0;
            lookEnabled = false;
        }

        public override bool HasStateFinished(AIBlackBoard blackBoard)
        {
            return true;
        }

        public static float CheckEpsilon(float xFloat, float target)
        {
            if (target > .5f) // target = 1
                xFloat = (target - xFloat < .01f) ? 1 : xFloat;
            else
                xFloat = (xFloat < .01f) ? 0 : xFloat;
            return xFloat;
        }

        public static bool AtMinEpsilon(float t)
        {
            if (t < .02f)
                return true;
            return false;
        }
    }

    [System.Serializable]
    public class HumanLookIKProps
    {
        public float headIKSmooth = 3f;
        public float headIKBackSmooth = 3f;

        [Space]
        [Range(0, 180)]
        public float maxLookTAngleHorizontal = 75;

        [Range(0, 90)]
        public float maxLookTAngleVertical = 90;

        [Space]
        public float angleLerpSpeed = 4f;

        public float angleLerpBackSpeed = 4.5f;

        [Space]
        public float slawBodyWeight = 1;

        public float slawHeadWeight = 1;
        public float slawClamp = .5f;
    }
}