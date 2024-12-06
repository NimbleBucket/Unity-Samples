using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public interface IGravityProvider
{
    int Priority { get; set; }
    bool Invert { get; set; }
    (Vector3, float) GetGravity(Vector3 atPosition);
    bool CanSupplyGravity(Vector3 atPosition, float radius);

    CinemachineVirtualCamera OverrideCamera { get; set; }

    Collider[] VisualColliders { get; set; }

}
