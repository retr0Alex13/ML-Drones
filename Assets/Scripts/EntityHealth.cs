using System;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    public event Action OnDeath;

    [SerializeField] private float health = 100f;
    public float Health { get { return health; } private set { } }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            Die();
        }
    }

    private void Die()
    {
        OnDeath?.Invoke();
    }
}
