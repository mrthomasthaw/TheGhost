using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPositionAdjustment : MonoBehaviour
{
    [System.Serializable]
    public class PositionSettings
    {
        [Header("-Rifle Default-")]
        public Vector3 rifleRHSDefaultPos;
        public Vector3 rifleRHSDefaultEulerAngle;
        public Vector3 rifleLHSDefaultPos;
        public Vector3 rifleLHSDefaultEulerAngle;

        [Header("-Rifle Crouch-")]
        public Vector3 rifleRHSCrouchPos;
        public Vector3 rifleRHSCrouchEulerAngle;
        public Vector3 rifleLHSCrouchPos;
        public Vector3 rifleLHSCrouchEulerAngle;


        [Header("-Pistol Default-")]
        public Vector3 pistolRHSDefaultPos;
        public Vector3 pistolRHSDefaultEulerAngle;
        public Vector3 pistolLHSDefaultPos;
        public Vector3 pistolLHSDefaultEulerAngle;

        [Header("-Pistol Crouch-")]
        public Vector3 pistolRHSCrouchPos;
        public Vector3 pistolRHSCrouchEulerAngle;
        public Vector3 pistolLHSCrouchPos;
        public Vector3 pistolLHSCrouchEulerAngle;
    }

    [SerializeField]
    public PositionSettings positionSettings;

    public Transform rifleRHS, rifleLHS;
    public Transform pistolRHS, pistolLHS;

    [SerializeField] bool crouch;

    // Start is called before the first frame update
    void Start()
    {
        rifleRHS = CommonUtil.FindDeepestChildByName(this.transform, "RifleRHS");
        rifleLHS = CommonUtil.FindDeepestChildByName(this.transform, "RifleLHS");
        pistolRHS = CommonUtil.FindDeepestChildByName(this.transform, "PistolRHS");
        pistolLHS = CommonUtil.FindDeepestChildByName(this.transform, "PistolLHS");
    }

    // Update is called once per frame
    void Update()
    {    
        if(Input.GetButtonDown("Crouch"))
        {
            crouch = !crouch;
        }

        Vector3 newRifleRHSPos;
        Vector3 newRifleRHSEulerAngle;

        Vector3 newRifleLHSPos;
        Vector3 newRifleLHSEulerAngle;

        if (crouch)
        {
            newRifleRHSPos = positionSettings.rifleRHSCrouchPos;
            newRifleRHSEulerAngle = positionSettings.rifleRHSCrouchEulerAngle;

            newRifleLHSPos = positionSettings.rifleLHSCrouchPos;
            newRifleLHSEulerAngle = positionSettings.rifleLHSCrouchEulerAngle;
        }
        else
        {
            newRifleRHSPos = positionSettings.rifleRHSDefaultPos;
            newRifleRHSEulerAngle = positionSettings.rifleRHSDefaultEulerAngle;

            newRifleLHSPos = positionSettings.rifleLHSDefaultPos;
            newRifleLHSEulerAngle = positionSettings.rifleLHSDefaultEulerAngle;
        }

        rifleRHS.localPosition = Vector3.Lerp(rifleRHS.localPosition, newRifleRHSPos, Time.deltaTime * 3f);
        rifleRHS.localEulerAngles = newRifleRHSEulerAngle;

        rifleLHS.localPosition = Vector3.Lerp(rifleLHS.localPosition, newRifleLHSPos, Time.deltaTime * 3f);
        rifleLHS.localEulerAngles = newRifleLHSEulerAngle;

    }
}
