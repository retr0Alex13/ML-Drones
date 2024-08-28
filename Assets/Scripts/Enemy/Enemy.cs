using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private DroneAgent droneAgent;

    [SerializeField] private bool moveEnemy;
    [SerializeField] private bool randomizeSpeed;
    [SerializeField] private float speed = 1f;
    [SerializeField, Range(3, 5)] private float maxEnemySpeed = 4f;

    private BoxCollider spawnZoneCollider;

    private float randomMovingSpeed;
    private Vector3 targetPosition;

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
        if (moveEnemy)
        {
            MoveTowardsTarget();
        }
    }

    private void MoveTowardsTarget()
    {
        Vector3 currentLocalPosition = transform.localPosition;
        Vector3 targetLocalPosition = transform.parent.InverseTransformPoint(targetPosition);

        float step = (randomizeSpeed ? randomMovingSpeed : speed) * Time.deltaTime;
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
        if (Academy.Instance.EnvironmentParameters.GetWithDefault("Lesson", 0) > 1)
        {
            moveEnemy = true;
        }
        if (Academy.Instance.EnvironmentParameters.GetWithDefault("Lesson", 0) > 2)
        {
            randomizeSpeed = true;
        }
        randomMovingSpeed = Random.Range(speed, maxEnemySpeed);
    }

    public void SetSpawnZoneCollider(BoxCollider spawnZone)
    {
        spawnZoneCollider = spawnZone;
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
