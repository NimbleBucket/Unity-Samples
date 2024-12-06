using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using Unity.Cinemachine;

public class SplineGravity : MonoBehaviour, IGravityProvider
{
    public SplineComputer GravitySplineComputer;
    public Vector2 LeftRightExtents = new Vector2(-2, 2);
    public Vector2 DownUpExtents = new Vector2(0, 3);
    public float ForwardExtents = .1f;

    public float GravityScale = 1f;

    [field: SerializeField]
    public int Priority { get; set; }

    [field: SerializeField]
    public bool Invert { get; set; }

    private Transform temp;

    [field: SerializeField] public CinemachineVirtualCamera OverrideCamera { get; set; }

    [field: SerializeField] public Collider[] VisualColliders { get; set; }


    private void Start()
    {
        temp = (new GameObject()).transform;
    }

    public (Vector3, float) GetGravity(Vector3 atPosition)
    {
        var splineSample = GravitySplineComputer.Project(atPosition);
        return (splineSample.rotation * (Invert? Vector3.up : Vector3.down) , GravityScale);
    }

    public bool CanSupplyGravity(Vector3 atPosition, float radius)
    {
        var splineSample = GravitySplineComputer.Project(atPosition);
        var splinePosition = splineSample.position;




        var directionToSpline = (atPosition - splinePosition).normalized;
        var positionWithRadius = atPosition + (directionToSpline * radius);
        Debug.DrawLine(splinePosition, positionWithRadius);

        //early return if we're farther than any of the extents would allow
        if(Vector3.Distance(positionWithRadius, splinePosition) > Mathf.Max(Mathf.Abs(LeftRightExtents.x), LeftRightExtents.y, Mathf.Abs(DownUpExtents.x), DownUpExtents.y))
           return false;


        temp.SetPositionAndRotation(splinePosition, Quaternion.LookRotation(splineSample.forward, splineSample.up));
       
        var localizedPosition = temp.InverseTransformPoint(positionWithRadius); 
        return localizedPosition.x < LeftRightExtents.y &&
                localizedPosition.x > LeftRightExtents.x &&
                localizedPosition.y < DownUpExtents.y &&
                localizedPosition.y > DownUpExtents.x;



        var topRightPosition = new Vector3(LeftRightExtents.y, DownUpExtents.y, ForwardExtents/2);
        var bottomLeftPosition = new Vector3(LeftRightExtents.x, DownUpExtents.x, -ForwardExtents / 2);
        Bounds bounds = new Bounds();
        var checkPosition = Quaternion.Inverse(splineSample.rotation) * (atPosition - splinePosition);
       // Debug.DrawLine(atPosition, checkPosition + position, Color.green);
        bounds.SetMinMax(bottomLeftPosition + splinePosition, topRightPosition + splinePosition);
        //DrawBounds(bounds);
        return bounds.Contains(checkPosition + splinePosition); 
    }
    public void DrawBounds(Bounds b, float delay = 0)
    {
        // bottom
        var p1 = new Vector3(b.min.x, b.min.y, b.min.z);
        var p2 = new Vector3(b.max.x, b.min.y, b.min.z);
        var p3 = new Vector3(b.max.x, b.min.y, b.max.z);
        var p4 = new Vector3(b.min.x, b.min.y, b.max.z);

        Debug.DrawLine(p1, p2, Color.blue, delay);
        Debug.DrawLine(p2, p3, Color.red, delay);
        Debug.DrawLine(p3, p4, Color.yellow, delay);
        Debug.DrawLine(p4, p1, Color.magenta, delay);

        // top
        var p5 = new Vector3(b.min.x, b.max.y, b.min.z);
        var p6 = new Vector3(b.max.x, b.max.y, b.min.z);
        var p7 = new Vector3(b.max.x, b.max.y, b.max.z);
        var p8 = new Vector3(b.min.x, b.max.y, b.max.z);

        Debug.DrawLine(p5, p6, Color.blue, delay);
        Debug.DrawLine(p6, p7, Color.red, delay);
        Debug.DrawLine(p7, p8, Color.yellow, delay);
        Debug.DrawLine(p8, p5, Color.magenta, delay);

        // sides
        Debug.DrawLine(p1, p5, Color.white, delay);
        Debug.DrawLine(p2, p6, Color.gray, delay);
        Debug.DrawLine(p3, p7, Color.green, delay);
        Debug.DrawLine(p4, p8, Color.cyan, delay);
    }
}
