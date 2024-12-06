using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderGravity : MonoBehaviour, IGravityProvider
{
    public Vector3 axisVector;
    public Dictionary<Collider, Vector3> storedGravityNormal;
    public List<IGravity> currentGravityProviders;

    public float GravityScale = 1f;

    [field: SerializeField]
    public int Priority { get; set; }
    [field: SerializeField]
    public bool Invert { get; set; }
    [field: SerializeField] public CinemachineVirtualCamera OverrideCamera { get; set; }
    [field: SerializeField] public Collider[] VisualColliders { get; set; }

    public static Vector3 NearestPointOnLine(Vector3 linePnt, Vector3 lineDir, Vector3 pnt)
    {
        lineDir.Normalize();//this needs to be a unit vector
        var v = pnt - linePnt;
        var d = Vector3.Dot(v, lineDir);
        return linePnt + lineDir * d;
    } 

    public (Vector3, float) GetGravity(Vector3 atPosition)
    { 
        Vector3 nearestOnLine = NearestPointOnLine(transform.position, transform.rotation * axisVector, atPosition);
        return ((nearestOnLine - atPosition).normalized * (Invert ? -1 : 1), GravityScale); 
    }

    public bool CanSupplyGravity(Vector3 atPosition, float radius)
    {
        return true;
    }
}
