using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
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
        float randomX = UnityEngine.Random.Range(-1f, 64);
        transform.localPosition = new Vector3(randomX, 2.25f, 51f);
    }
}
