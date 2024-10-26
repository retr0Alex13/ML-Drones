using UnityEngine;

public class Enemy : MonoBehaviour
{
    public MoveBetweenWaypoints MoveBetweenWaypoints;

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
    }

    private void OnDestroy()
    {
        DroneAgent.OnNewEpisode -= ResetEnemy;
    }

    private void ResetEnemy()
    {
        SetRandomPosition();
    }
    private void SetRandomPosition()
    {
        CreateRandomPoint();
        if (MoveBetweenWaypoints.enabled)
        {
            return;
        }
        transform.position = targetPosition;
    }

    public void SetSpawnZoneCollider(BoxCollider spawnZone)
    {
        spawnZoneCollider = spawnZone;
        SetRandomPosition();
    }

    public void SetRandomRotation()
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
