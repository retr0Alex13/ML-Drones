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
        List<Collider> colliders = new List<Collider>();

        Collider[] overlappedColliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider collider in overlappedColliders)
        {
            if (collider != null && collider.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.AddExplosionForce(1000, transform.position, explosionRadius);
                colliders.Add(collider);
            }
        }

        Instantiate(explosionFX, transform.position, Quaternion.identity);

        return colliders;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Obstacle"))
        {
            if (transform.parent.parent.TryGetComponent(out DroneAgent drone))
            {
                drone.OnObstacleHit();
            }
        }
    }
}