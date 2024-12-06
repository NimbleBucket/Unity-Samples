using BeetleBucketBusiness;
using ECM2;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class CharacterGravity : MonoBehaviour, IGravity
{
    private Character character;
    [field: SerializeField] public Vector3 Gravity { get; set; } = Vector3.down;
    [field: SerializeField] public Vector3 DesiredGravity { get; set; }
    [field: SerializeField] public float GravityScale { get; set; }
    [field: SerializeField] public float Radius { get; set; }
    public Collider[] GravityColliders { get; set; } = new Collider[8];
    [field: SerializeField] public LayerMask GravityColliderMask { get; set; }
    [field: SerializeField] public float InGravityRadiusAdjustment { get; set; } = 1f;
    [field: SerializeField] public IGravityProvider CurrentGravityProvider { get; set; }



    private float interpScale;

    public bool UpdateGravityScale;

    public float GravityAdjustSharpness;

    private float lastGravityUpdateTime; 

    // Start is called before the first frame update
    void Start()
    {
        GravityColliders = new Collider[8];
        TryGetComponent<Character>(out character);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float scale;
        var previousGravityProvider = CurrentGravityProvider;
        (DesiredGravity, scale) = this.PollDesiredGravity(character.position + (character.GetUpVector() * character.height / 2));
        if (UpdateGravityScale)
            GravityScale = scale;

        if(CurrentGravityProvider != previousGravityProvider)
        {
            if (previousGravityProvider != null && previousGravityProvider.OverrideCamera != null)
                previousGravityProvider.OverrideCamera.Priority = 0;

            if (CurrentGravityProvider != null && CurrentGravityProvider.OverrideCamera != null)
                CurrentGravityProvider.OverrideCamera.Priority = 10;
        }
         
        var newGravityFlattenedVertically = Vector3.ProjectOnPlane(DesiredGravity, character.GetRightVector()).normalized;
        var desiredNewGravity = DesiredGravity;
        if (Vector3.Dot(DesiredGravity, Gravity) < .3f)
        {
            desiredNewGravity = Vector3.ProjectOnPlane(DesiredGravity, character.GetForwardVector()).normalized; ;
        }

        CalculateNewGravity(desiredNewGravity);
    }

    void Update()
    {
        CalculateNewGravity(DesiredGravity);
    }

    private void CalculateNewGravity(Vector3 desiredNewGravity)
    {
        var timeChange = Time.time - lastGravityUpdateTime;
        Gravity = Vector3.Slerp(Gravity, desiredNewGravity, 1 - Mathf.Exp(-GravityAdjustSharpness * timeChange)).normalized;
        lastGravityUpdateTime = Time.time;
    }
}
