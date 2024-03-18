using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private float explotionRadius = 5f;
    // Add particle effect to the projectile

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    private void Explode()
    {
        int maxColliders = 20;
        Collider[] colliders = new Collider[maxColliders];
        int colliderNumbers = Physics.OverlapSphereNonAlloc(transform.position, explotionRadius, colliders);
        for (int i = 0; i < colliderNumbers; i++)
        {
            if (colliders[i] == null)
            {
                continue;
            }
            if (colliders[i].TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.AddExplosionForce(1000, transform.position, explotionRadius);
            }
            // Damage damagable objects
        }
        // Enable particle effect
    }
}
