using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float explotionRadius = 5f;
    [SerializeField] private GameObject explosionFX;

    private void OnEnable()
    {
        DroneController.OnDroneCollision += Explode;
    }

    private void OnDisable()
    {
        DroneController.OnDroneCollision -= Explode;
    }

    private void Update()
    {
        transform.position = transform.parent.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explotionRadius);
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
            if (colliders[i].TryGetComponent(out IDamageable damageable))
            {
                damageable.Damage(damageAmount);
            }
            if (colliders[i].TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.AddExplosionForce(1000, transform.position, explotionRadius);
            }
        }
        Instantiate(explosionFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
