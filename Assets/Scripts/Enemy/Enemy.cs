using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public MoveBetweenWaypoints MoveBetweenWaypoints;
    [SerializeField] private bool moveEnemy;

    [field: SerializeField] public DroneAgent DroneAgent { get; set; }

    private BoxCollider spawnZoneCollider;

    private Vector3 targetPosition;

    private void Awake()
    {
        MoveBetweenWaypoints = GetComponent<MoveBetweenWaypoints>();
    }

    private void Start()
    {
        DroneAgent.OnNewEpisode += ResetEnemy;

        SetMovementState();
    }

    private void OnDestroy()
    {
        DroneAgent.OnNewEpisode -= ResetEnemy;
    }

    private void ResetEnemy()
    {
        SetRandomPosition();

        SetRandomRotation();

        SetMovementState();
    }

    private void SetMovementState()
    {
        if (Academy.Instance.EnvironmentParameters.GetWithDefault("Lesson", 0) <= 0)
        {
            return;
        }
        else
        {
            int randomNum = Random.Range(0, 2);
            moveEnemy = randomNum > 0 ? true : false;

            if (moveEnemy)
            {
                MoveBetweenWaypoints.enabled = true;
            }
        }
    }

    public void SetSpawnZoneCollider(BoxCollider spawnZone)
    {
        spawnZoneCollider = spawnZone;
        SetRandomPosition();
    }

    private void SetRandomPosition()
    {
        CreateRandomPoint();
        transform.position = targetPosition;
    }

    private void SetRandomRotation()
    {
        float randomRotation = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0f, randomRotation, 0f);
    }

    private void CreateRandomPoint()
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
