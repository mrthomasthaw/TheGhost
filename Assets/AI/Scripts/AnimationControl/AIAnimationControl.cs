using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIAnimationControl : MonoBehaviour
{
    private Animator animator;

    private List<CustomStateMachineBehaviour> stateMachineBehaviours = new List<CustomStateMachineBehaviour>();

    private AnimationStateInfo animationStateInfo;

    private MrThaw.IKControl ikControl;

    [SerializeField]private AnimationKey currentAnimationKey;

    [SerializeField]private float layerWeight;

    private void Start()
    {
        animator = GetComponent<Animator>();
        ikControl = GetComponent<MrThaw.IKControl>();
        animationStateInfo = new AnimationStateInfo();

        stateMachineBehaviours = animator.GetBehaviours<CustomStateMachineBehaviour>().ToList();
        stateMachineBehaviours.ForEach(s => s.SetUp(animationStateInfo));
    }

    private void Update()
    {
        HandleLayerWeight();
    }

    void HandleLayerWeight()
    {
        switch (currentAnimationKey)
        {
            case AnimationKey.None:
                break;
            case AnimationKey.AimWeapon:
                layerWeight = Mathf.Lerp(layerWeight, 1f, Time.deltaTime * 30f);
                animator.SetLayerWeight(1, layerWeight);
                break;
            case AnimationKey.PutDownWeapon:
                layerWeight = Mathf.Lerp(layerWeight, 0f, Time.deltaTime * 30f);
                animator.SetLayerWeight(1, layerWeight);
                break;
            case AnimationKey.Crouch:
                break;
        }
    }

   

    public void AnimateSingleAction(AnimationKey key)
    {
        switch (key)
        {
            case AnimationKey.None:
                break;
            case AnimationKey.AimWeapon:
                animator.SetBool("Aim", true);
                break;
            case AnimationKey.PutDownWeapon:
                animator.SetBool("Aim", false);
                break;
            case AnimationKey.Crouch:
                break;
        }

        currentAnimationKey = key;
    }

    public bool IsAnimationFinished(AnimationKey key) 
    {
        switch(key)
        {
            case AnimationKey.None:
                return true;
            case AnimationKey.AimWeapon:
                return animationStateInfo.HaveAimingFinished;
            case AnimationKey.PutDownWeapon:
                return animationStateInfo.HavePuttingDownWeaponFinished;
            default:
                return true;
        }
    }
}

public enum AnimationKey
{
    None,
    AimWeapon,
    PutDownWeapon,
    Crouch
}
