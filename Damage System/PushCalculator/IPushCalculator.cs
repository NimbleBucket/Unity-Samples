using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IPushCalculator
{
    Transform Source { get; set; }
    Vector3 LocalProjectionPlane { get; set; }
    float AdditionalProjectionPlaneBoost { get; set; }
    float PushStrength { get; set; }

    Vector3 GetFromPosition(Vector3 toPushPosition);

    public Vector3 CalculatePush(Vector3 toPushPosition)
    { 
        var direction = (toPushPosition - GetFromPosition(toPushPosition)).normalized;
        var projectionPlane = Source.TransformDirection(LocalProjectionPlane);
        direction = Vector3.ProjectOnPlane(direction, projectionPlane).normalized;
        direction = (direction + (projectionPlane * AdditionalProjectionPlaneBoost)).normalized;
        return direction * PushStrength;
    }
}

