using System;
using UnityEngine;

public class ObjectGravity : MonoBehaviour, IGravity
{
    public Rigidbody ObjectRigidbody;
     
    public float GravityStrength = 35f;

    [field: SerializeField] public Vector3 Gravity { get; set; }
    [field: SerializeField] public Vector3 DesiredGravity { get; set; }
    [field: SerializeField] public float GravityScale { get; set; }
    [Range(.1f, 3f)]
    [field: SerializeField] public float Radius { get; set; }
    [field: SerializeField] public float InGravityRadiusAdjustment { get; set; }

    [field: SerializeField] public Collider[] GravityColliders { get; set; }
    [field: SerializeField] public LayerMask GravityColliderMask { get; set; }

    [field: SerializeField] public IGravityProvider CurrentGravityProvider { get; set; }



    private void Awake()
    {
        if(ObjectRigidbody == null)
        {
            if(!TryGetComponent<Rigidbody>(out ObjectRigidbody))
            {
                ObjectRigidbody = GetComponentInChildren<Rigidbody>();
            }
        }

        GravityColliders = new Collider[8];
    }
    private void FixedUpdate()
    {
        float scale;
        (DesiredGravity, scale) = this.PollDesiredGravity(ObjectRigidbody.position);

        Gravity = DesiredGravity * scale;

        ObjectRigidbody.AddForce(Gravity * GravityStrength, ForceMode.Acceleration);
    }
}
