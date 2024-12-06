using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGravity
{
    Vector3 Gravity { get; set; }
    Vector3 DesiredGravity { get; set; }

    float GravityScale { get; set; }
    GameObject gameObject { get; }

    float Radius { get; set; }
    float InGravityRadiusAdjustment { get; set; }

    IGravityProvider CurrentGravityProvider { get; set; }
    Collider[] GravityColliders { get; set; } 
    LayerMask GravityColliderMask { get; set; }  

}

public static class GravityExtensions
{
    public static (Vector3 gravityNormal, float gravityScale) PollDesiredGravity(this IGravity gravity, Vector3 position)
    {
        return gravity.PollDesiredGravity(position, out _, out _); 
    }


    public static (Vector3 gravityNormal, float gravityScale) PollDesiredGravity(this IGravity gravity, Vector3 position, out int numColliders, out IGravityProvider[] gravityProviders)
    {
        gravityProviders = null;
        var radiusAdjustment = gravity.CurrentGravityProvider == null ? 0 : gravity.InGravityRadiusAdjustment;
        numColliders = Physics.OverlapSphereNonAlloc(position, gravity.Radius + radiusAdjustment, gravity.GravityColliders, gravity.GravityColliderMask); 
        if (numColliders <= 0)
        {
            gravity.CurrentGravityProvider = null;
            return (Vector3.down, 1f);
        }

        IGravityProvider currentGravityProvider = null;
        gravityProviders = new IGravityProvider[8];
        for (int i = 0; i < numColliders; i++) 
        {
            var collider = gravity.GravityColliders[i];
            if (collider != null)
            {
                collider.TryGetComponent<IGravityProvider>(out IGravityProvider gravityProvider);
                if (gravityProvider != null && gravityProvider.CanSupplyGravity(position, gravity.Radius))
                {
                    gravityProviders[i] = gravityProvider;

                    if (currentGravityProvider == null || currentGravityProvider.Priority < gravityProvider.Priority)
                        currentGravityProvider = gravityProvider;
                }
            }
        }
         

        gravity.CurrentGravityProvider = currentGravityProvider;
        if (currentGravityProvider == null)
        {
            gravityProviders = null;
            return (Vector3.down, 1f);
        }

        return currentGravityProvider.GetGravity(position);
    }
}
