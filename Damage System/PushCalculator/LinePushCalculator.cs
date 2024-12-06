using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeetleBucketBusiness;


public class LinePushCalculator : IPushCalculator
{
    public Transform Source { get; set; }
    [field: SerializeField]
    public Vector3 LocalProjectionPlane { get; set; } = Vector3.up;
    [field: SerializeField]
    public float AdditionalProjectionPlaneBoost { get; set; } = 1f;
    [field: SerializeField]
    public float PushStrength { get; set; } = 1f;

    public Vector3 LineStartLocal;
    public Vector3 LineEndLocal;

    public Vector3 LineStart => Source.TransformPoint(LineStartLocal);
    public Vector3 LineEnd => Source.TransformPoint(LineEndLocal); 

    public Vector3 GetFromPosition(Vector3 toPushPosition)
    { 

        return toPushPosition.ClosestPointOnLine(LineStart, LineEnd);

    }
}
