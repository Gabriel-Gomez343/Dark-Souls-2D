using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactEffect : MonoBehaviour
{
    public GameObject impactEffectPrefab;
    public Vector3 impactEffectOffset; // Offset from the enemy's position
    public Vector3 impactEffectScale = Vector3.one; // Default scale factor

    // Function to be called by animation events
    public void PlayImpactEffect()
    {
        // Calculate the position to spawn the impact effect
        Vector3 impactPosition = transform.position + impactEffectOffset;

        // Instantiate the impact effect prefab at the desired position with the specified scale
        GameObject impactEffectInstance = Instantiate(impactEffectPrefab, impactPosition, Quaternion.identity);
        impactEffectInstance.transform.localScale = impactEffectScale;

        // Create a symmetrical instance by mirroring the impact effect along the X-axis
        GameObject symmetricalEffectInstance = Instantiate(impactEffectPrefab, impactPosition, Quaternion.identity);
        
        // Flip the sprite of the symmetrical instance horizontally
        FlipSprite(symmetricalEffectInstance);

        // Apply the same scale to the symmetrical instance
        symmetricalEffectInstance.transform.localScale = impactEffectScale;
    }

    // Function to flip the sprite of the symmetrical instance horizontally
    private void FlipSprite(GameObject obj)
    {
        // Get the SpriteRenderer component of the GameObject
        SpriteRenderer spriteRenderer = obj.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Flip the sprite horizontally
            spriteRenderer.flipX = true;
        }
        else
        {
            Debug.LogWarning("SpriteRenderer component not found on the GameObject.");
        }
    }
}
