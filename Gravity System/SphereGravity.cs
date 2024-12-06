using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereGravity : MonoBehaviour, IGravityProvider
{
    [field: SerializeField]
    public int Priority { get; set; }
    [field:SerializeField] public bool Invert { get; set; }
    public float GravityScale = 1f;

    [field: SerializeField] public CinemachineVirtualCamera OverrideCamera { get; set; }
    [field: SerializeField] public Collider[] VisualColliders { get; set; }

    public (Vector3, float) GetGravity(Vector3 atPosition)
    {  
        return ((transform.position - atPosition).normalized * (Invert ? -1 : 1), GravityScale); 
    }

    public bool CanSupplyGravity(Vector3 atPosition, float radius)
    {
        return true;
    }
}