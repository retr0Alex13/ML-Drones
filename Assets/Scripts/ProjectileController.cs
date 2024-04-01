using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private GameObject explosionFX;

    private void Update()
    {
        transform.position = transform.parent.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    public List<Collider> Explode()
    {
        // Create a list to store the colliders
        List<Collider> colliders = new List<Collider>();

        // Get all colliders within the explosion radius
        Collider[] overlappedColliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider collider in overlappedColliders)
        {
            if (collider != null && collider.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.AddExplosionForce(1000, transform.position, explosionRadius);
                colliders.Add(collider);
            }
        }

        // Instantiate the explosion effect
        Instantiate(explosionFX, transform.position, Quaternion.identity);

        // Return the colliders array
        return colliders;
    }
}