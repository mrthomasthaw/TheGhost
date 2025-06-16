using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HealthControl : MonoBehaviour
{
    private TargetableBodyPart[] targetableBodyParts;
    private List<Collider> bodyColliders = new List<Collider>();
    private List<Rigidbody> ragdollRigidBodies = new List<Rigidbody>();
    public int health;

    private void Awake()
    {
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
    }

    void OnHit()
    {
        Debug.Log("Hit");
        health -= 10;
    }
}
