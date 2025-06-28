using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

public class HealthControl : MonoBehaviour
{
    private TargetableBodyPart[] targetableBodyParts;
    private List<Collider> bodyColliders = new List<Collider>();
    private List<Rigidbody> ragdollRigidBodies = new List<Rigidbody>();
    private List<CharacterJoint> joints = new List<CharacterJoint>();
    private Animator animator;

    [SerializeField]
    private DeathEventSO deathEventSO;

    [SerializeField]
    private int health;

    public bool IsDeath { get; private set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        targetableBodyParts = GetComponentsInChildren<TargetableBodyPart>();
        targetableBodyParts.ToList().ForEach(part => 
        {
            part.onHit += OnHit;

            Collider collider = part.GetComponent<Collider>();
            if (collider != null) 
            {
                collider.isTrigger = true;
                bodyColliders.Add(collider);
            }

            Rigidbody rb = part.GetComponent<Rigidbody>();
            if (rb != null) 
            { 
                rb.isKinematic = true;

                ragdollRigidBodies.Add(rb);
            }
        });

        //GetComponentsInChildren<CharacterJoint>().ToList().ForEach(joint =>
        //{
        //    if (joint != null)
        //    {
        //        SoftJointLimit swing = new SoftJointLimit { limit = -45f };
        //        joint.swing1Limit = swing;
        //        joint.swing2Limit = swing;
        //        joint.lowTwistLimit = new SoftJointLimit { limit = -20f };
        //        joint.highTwistLimit = new SoftJointLimit { limit = 20f };
        //    }

        //    joints.Add(joint);
        //});
        
    }

    void OnHit()
    {
        Debug.Log("Hit");
        if(health > 0)
        {
            health -= 10;
        }
        else
        {
            health = Mathf.Clamp(health, 0, 100);
            deathEventSO.RaiseEvent(this.gameObject);
            Death();
        }
    }

    void Death()
    {
        IsDeath = true;
        bodyColliders.ForEach(collider => 
        {
            collider.isTrigger = false;
        });

        ragdollRigidBodies.ForEach(r => 
        { 
            r.isKinematic = false;
        });

        animator.enabled = false;
    }
}
