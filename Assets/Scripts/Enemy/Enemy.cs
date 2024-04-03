using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Collider spawnBoundsCollider;
    private Rigidbody rigidBody;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    public void ResetEnemy()
    {
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = Vector3.zero;

        // Randomize enemy position
        float randomX = Random.Range(spawnBoundsCollider.bounds.min.x, spawnBoundsCollider.bounds.max.x);
        transform.localPosition = new Vector3(randomX, 2.25f, 51f);
    }
}
