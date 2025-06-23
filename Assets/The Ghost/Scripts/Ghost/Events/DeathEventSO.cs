using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScriptableObjects/DeathEventSO", menuName = "Events/Death Event")]
public class DeathEventSO : ScriptableObject
{
    public event Action<GameObject> OnEventRaised;

    public void RaiseEvent(GameObject go)
    {
        OnEventRaised?.Invoke(go);
    }
}
