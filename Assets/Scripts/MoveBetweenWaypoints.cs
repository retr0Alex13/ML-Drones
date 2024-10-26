using UnityEngine;

public class MoveBetweenWaypoints : MonoBehaviour
{
    public Transform[] waypoints;
    [SerializeField] private float defaultSpeed = 2f;
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float reachThreshold = 0.1f;
    [SerializeField] private float rotationSpeed = 5f;

    private int currentWaypointIndex = 0;
    private bool isReversing;
    private bool randomizeSpeed;

    void OnEnable()
    {
        if (waypoints.Length == 0)
        {
            Debug.LogError("No waypoints set for MoveBetweenWaypoints script.");
            return;
        }
        int randomWaypointIndex = Random.Range(0, waypoints.Length);
        currentWaypointIndex = randomWaypointIndex;
        transform.position = waypoints[randomWaypointIndex].position;
    }

    void Update()
    {
        if (waypoints.Length == 0)
        {
            return;
        }
        MoveToNextWaypoint();
    }

    private void MoveToNextWaypoint()
    {
        Vector3 targetPosition = waypoints[currentWaypointIndex].position;

        RotateTowards(targetPosition);
        float speed;

        if (randomizeSpeed)
        {
            speed = Random.Range(defaultSpeed, maxSpeed);
        }
        else
        {
            speed = defaultSpeed;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < reachThreshold)
        {
            if (isReversing)
            {
                currentWaypointIndex--;

                if (currentWaypointIndex <= 0)
                {
                    currentWaypointIndex = 0;
                    isReversing = false;
                }
            }
            else
            {
                currentWaypointIndex++;

                if (currentWaypointIndex >= waypoints.Length - 1)
                {
                    currentWaypointIndex = waypoints.Length - 1;
                    isReversing = true;
                }
            }
        }
    }

    private void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;

        if (direction == Vector3.zero) return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }

    public void RandomizeSpeed()
    {
        int value = Random.Range(0, 2);
        randomizeSpeed = value > 0 ? true : false;
    }
}
