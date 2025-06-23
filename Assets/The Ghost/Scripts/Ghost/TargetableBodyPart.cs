using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetableBodyPart : MonoBehaviour
{
    private LayerMask hitLayer;
    public delegate void OnHit();
    public OnHit onHit;

    private void Awake()
    {
        hitLayer = LayerMask.GetMask("Bullet");
        this.gameObject.layer = LayerMask.NameToLayer("BodyPart");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & hitLayer) != 0)
        {
            onHit?.Invoke();
        }
    }


}
