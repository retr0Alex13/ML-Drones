using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private DroneAgent droneAgent;

    [SerializeField] private float speed = 1f;
    [SerializeField] private bool randomizeSpeed;
    [SerializeField] private BoxCollider spawnZoneCollider;
    [SerializeField] private List<Transform> waypoints;

    private float randomSpeed = 1f;
    private int index = 0;

    private void Awake()
    {
        droneAgent.OnNewEpisode += ResetEnemy;
    }

    private void OnDestroy()
    {
        droneAgent.OnNewEpisode -= ResetEnemy;
    }

    private void Update()
    {
        //MoveToWaypoints();
    }

    private void MoveToWaypoints()
    {
        Vector3 targetPosition = waypoints[index].position;
        Vector3 newPosition;
        if (randomizeSpeed)
        {
            newPosition = Vector3.MoveTowards(transform.position, targetPosition, randomSpeed * Time.deltaTime);
        }
        else
        {
            newPosition = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
        transform.localPosition = newPosition;

        float distance = Vector3.Distance(transform.position, targetPosition);
        if (distance < 0.1f)
        {
            index++;
            if (index >= waypoints.Count)
            {
                index = 0;
            }
        }
    }

    private void ResetEnemy()
    {
        RandomizePosition();
        randomSpeed = Random.Range(1f, 5f);
    }

    private void RandomizePosition()
    {
        Bounds bounds = spawnZoneCollider.bounds;
        Vector3 randomPosition = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x), 1f,
            Random.Range(bounds.min.z, bounds.max.z)
        );

        transform.position = randomPosition;
    }
}
