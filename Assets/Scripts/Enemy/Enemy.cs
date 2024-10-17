using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [field: SerializeField] public DroneAgent DroneAgent { get; set; }

    [SerializeField] private bool moveEnemy;
    [SerializeField] private bool randomizeSpeed;
    [SerializeField] private float defaultSpeed = 1f;
    [SerializeField] private float maxSpeed = 5f;

    private BoxCollider spawnZoneCollider;

    private Vector3 targetPosition;

    private void Start()
    {
        DroneAgent.OnNewEpisode += ResetEnemy;
    }

    private void OnDestroy()
    {
        DroneAgent.OnNewEpisode -= ResetEnemy;
    }

    private void Update()
    {
        if (moveEnemy)
        {
            MoveTowardsTarget();
        }
    }

    private void MoveTowardsTarget()
    {
        Vector3 currentLocalPosition = transform.localPosition;
        Vector3 targetLocalPosition = transform.parent.InverseTransformPoint(targetPosition);
        float moveSpeed = Random.Range(defaultSpeed, maxSpeed);

        float step = (randomizeSpeed ? moveSpeed : defaultSpeed) * Time.deltaTime;
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
    }

    public void SetSpawnZoneCollider(BoxCollider spawnZone)
    {
        spawnZoneCollider = spawnZone;
        SetRandomPosition();
    }

    private void SetRandomPosition()
    {
        SetNewRandomTarget();
        transform.position = targetPosition;
    }

    private void SetRandomRotation()
    {
        float randomRotation = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0f, randomRotation, 0f);
    }

    private void SetNewRandomTarget()
    {
        Bounds localBounds = TransformBoundsToLocal(spawnZoneCollider.bounds);
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
