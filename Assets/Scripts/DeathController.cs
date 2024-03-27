using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathController : MonoBehaviour
{
    //[SerializeField] private GameObject deathVFX;
    // Add audio on death
    private EntityHealth entityHealth;

    private void Awake()
    {
        entityHealth = GetComponent<EntityHealth>();
    }

    private void OnEnable()
    {
        entityHealth.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        entityHealth.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        // Handle death here
        Debug.Log("Entity died");
        Destroy(gameObject);
    }
}
