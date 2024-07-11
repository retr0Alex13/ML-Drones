using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private DroneAgent droneAgent;

    [SerializeField] private float speed = 1f;
    [SerializeField, Range(1, 5)] private float maxEnemySpeed = 1f;
    [SerializeField] private bool randomizeSpeed;
    [SerializeField] private BoxCollider[] spawnZoneColliders;
    [SerializeField] private List<Transform> waypoints;

    private Vector3 targetPosition;
    private BoxCollider currentSpawnZone;

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
        //MoveTowardsTarget();
    }

    private void MoveTowardsTarget()
    {
        Vector3 currentLocalPosition = transform.localPosition;
        Vector3 targetLocalPosition = transform.parent.InverseTransformPoint(targetPosition);

        float step = (randomizeSpeed ? maxEnemySpeed : speed) * Time.deltaTime;
        transform.localPosition = Vector3.MoveTowards(currentLocalPosition, targetLocalPosition, step);

        if (Vector3.Distance(currentLocalPosition, targetLocalPosition) < 0.001f)
        {
            SetNewRandomTarget();
        }
    }

    private void ResetEnemy()
    {
        SetRandomPosition();
        SetRandomRotation();
        maxEnemySpeed = Random.Range(1f, 5f);
    }

    private void SetRandomPosition()
    {
         currentSpawnZone = 
            spawnZoneColliders[Random.Range(0, spawnZoneColliders.Length)];

        SetNewRandomTarget();
        transform.position = targetPosition;
    }

    private void SetRandomRotation()
    {
        float randomRotation = UnityEngine.Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0f, randomRotation, 0f);
    }

    private void SetNewRandomTarget()
    {
        Bounds localBounds = TransformBoundsToLocal(currentSpawnZone.bounds);
        Vector3 randomLocalPosition = new Vector3(
            Random.Range(localBounds.min.x, localBounds.max.x),
            localBounds.min.y,
            Random.Range(localBounds.min.z, localBounds.max.z)
        );

        targetPosition = transform.parent.TransformPoint(randomLocalPosition);
    }

    private Bounds TransformBoundsToLocal(Bounds worldBounds)
    {
        Vector3 center = transform.parent.InverseTransformPoint(worldBounds.center);
        Vector3 extents = worldBounds.extents;
        return new Bounds(center, extents * 2);
    }
}
