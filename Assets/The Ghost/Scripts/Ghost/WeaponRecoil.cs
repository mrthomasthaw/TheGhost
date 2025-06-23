using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [SerializeField] AnimationCurve animationCurve;
    [SerializeField] Transform currentAimPivot, handIkObj;

    [SerializeField]
    private DeathEventSO deathEventSO;


    public Vector3 baseHandPos;
    public bool isRecoilStart;
    public float recoilTimer;
    public float recoilDuration;

    private bool death = false;

    private void OnEnable()
    {
        deathEventSO.OnEventRaised += OnDeath;
    }

    private void OnDisable()
    {
        deathEventSO.OnEventRaised += OnDeath;
    }


    // Start is called before the first frame update
    void Start()
    {
        if(handIkObj != null)
            baseHandPos = handIkObj.localPosition;
    }


    private void LateUpdate()
    {
        if (death) return;

        if (recoilTimer >= 2f)
        {
            isRecoilStart = false;
            recoilTimer = 0;
        }

        if (isRecoilStart)
        {
            HandleWeaponRecoil();
        }

    }

    public void StartWeaponRecoil()
    {
        isRecoilStart = true;
    }

    public void HandleWeaponRecoil()
    {
        recoilTimer += Time.deltaTime * 12f;
        recoilDuration = animationCurve.Evaluate(recoilTimer);

        handIkObj.localPosition = baseHandPos + (Vector3.forward * -0.6f * recoilDuration);
    }

    public void SetUpHandIkObj(Transform currentAimPivot, Transform handIkObj)
    {
        this.currentAimPivot = currentAimPivot;
        this.handIkObj = handIkObj;
        baseHandPos = handIkObj.localPosition;
    }

    public void OnDeath(GameObject sender)
    {
        if (sender == this.gameObject)
            death = true;
    }
}
