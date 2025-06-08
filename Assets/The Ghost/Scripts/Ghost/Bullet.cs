using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    TrailRenderer trail;
    void Start()
    {
        trail = GetComponent<TrailRenderer>();
        trail.time = 0.3f;  // Duration of trail
        trail.minVertexDistance = 0.1f;  // Smoothness
        trail.startWidth = 0.015f;
        trail.endWidth = 0.01f;

        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        trail.receiveShadows = false;
        trail.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        trail.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
        Destroy(gameObject, 3f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
