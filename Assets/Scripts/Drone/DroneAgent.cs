using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class DroneAgent : Agent
{
    public event Action OnNewEpisode;

    [SerializeField]
    private GameObject explosionVFX;

    private bool randomizeDroneRotation;

    private BoxCollider spawnZoneCollider;

    private DroneController droneController;
    private Rigidbody droneRigidBody;

    private bool isTargetDetected;
    private Transform targetTransform;

    public override void Initialize()
    {
        droneController = GetComponent<DroneController>();
        droneRigidBody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        ResetDrone();
        OnNewEpisode?.Invoke();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (isTargetDetected)
        {
            Debug.Log("Enemy spotted!");
            sensor.AddObservation(targetTransform.localPosition);
        }
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.localRotation);
        sensor.AddObservation(droneRigidBody.velocity.magnitude);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        AddReward(-1f / MaxStep);

        float forwardAmount = 0f;
        float turnAmount = 0f;
        float altitudeAmount = 0f;

        switch (actions.DiscreteActions[0])
        {
            case 0:
                forwardAmount = 0f;
                break;
            case 1:
                forwardAmount = +1f;
                break;
            case 2:
                forwardAmount = -1f;
                break;
        }

        switch (actions.DiscreteActions[1])
        {
            case 0:
                turnAmount = 0f;
                break;
            case 1:
                turnAmount = -1f;
                break;
            case 2:
                turnAmount = +1f;
                break;
        }

        switch (actions.DiscreteActions[2])
        {
            case 0:
                altitudeAmount = 0f;
                break;
            case 1:
                altitudeAmount = +1f;
                break;
            case 2:
                altitudeAmount = -1f;
                break;
        }

        droneController.SetInputs(turnAmount, forwardAmount, altitudeAmount);
    }
    private void CheckRayPerception()
    {
        RayPerceptionSensorComponent3D[] m_rayPerceptionSensorComponent3Ds = GetComponents<RayPerceptionSensorComponent3D>();

        foreach (var sensor in m_rayPerceptionSensorComponent3Ds)
        {
            var rayOutputs = RayPerceptionSensor.Perceive(sensor.GetRayPerceptionInput(), false).RayOutputs;
            int lengthOfRayOutputs = rayOutputs.Length;


            // Alternating Ray Order: it gives an order of
            // (0, -delta, delta, -2delta, 2delta, ..., -ndelta, ndelta)
            // index 0 indicates the center of raycasts
            for (int i = 0; i < lengthOfRayOutputs; i++)
            {
                GameObject goHit = rayOutputs[i].HitGameObject;
                if (goHit != null)
                {
                    var rayDirection = rayOutputs[i].EndPositionWorld - rayOutputs[i].StartPositionWorld;
                    var scaledRayLength = rayDirection.magnitude;
                    float rayHitDistance = rayOutputs[i].HitFraction * scaledRayLength;

                    if (goHit.TryGetComponent(out Enemy enemy))
                    {
                        targetTransform = enemy.transform;
                        isTargetDetected = true;
                    }
                }
            }
        }
    }

    private void Update()
    {
        CheckRayPerception();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        int forwardAction = 0;
        if (Input.GetKey(KeyCode.W))
        {
            forwardAction = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            forwardAction = 2;
        }

        int turnAmount = 0;
        if (Input.GetKey(KeyCode.A))
        {
            turnAmount = 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            turnAmount = 2;
        }

        int altitudeAmount = 0;
        if (Input.GetKey(KeyCode.Space))
        {
            altitudeAmount = 1;
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            altitudeAmount = 2;
        }

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = forwardAction;
        discreteActions[1] = turnAmount;
        discreteActions[2] = altitudeAmount;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Enemy _))
        {
            OnGoalAchived();
        }
        else
        {
            Debug.Log("Crashed!");
            SetReward(-1f);
            EndEpisode();
        }
    }

    private void ResetDrone()
    {
        ResetDroneVelocity();
        SetRandomPosition();

        if (Academy.Instance.EnvironmentParameters.GetWithDefault("Lesson", 0) > 0)
        {
            int randomValue = UnityEngine.Random.Range(0, 2);

            if (randomValue > 0)
            {
                SetRandomRotation();
            }
            else
            {
                transform.localRotation = Quaternion.identity;
            }
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, -180f, 0f);
        }

        droneController.ResetSpeed();
        isTargetDetected = false;
    }

    private void ResetDroneVelocity()
    {
        droneRigidBody.velocity = new Vector3(0.1f,0f);
        droneRigidBody.angularVelocity = Vector3.zero;
    }

    public void SetSpawnZoneCollider(BoxCollider spawnZone)
    {
        spawnZoneCollider = spawnZone;
    }

    private void SetRandomPosition()
    {
        Bounds bounds = spawnZoneCollider.bounds;
        Vector3 randomPosition = new Vector3(
            UnityEngine.
            Random.Range(bounds.min.x, bounds.max.x),
            UnityEngine.
            Random.Range(bounds.min.y, bounds.max.y),
            UnityEngine.
            Random.Range(bounds.min.z, bounds.max.z)
        );
        transform.position = randomPosition;
        droneController.SetAltitude(randomPosition.y);
    }

    private void OnGoalAchived()
    {
        Debug.Log("Enemy hit");
        Instantiate(explosionVFX, transform.position, Quaternion.identity);
        SetReward(1f);
        EndEpisode();
    }
    private void SetRandomRotation()
    {
        float randomRotation = UnityEngine.Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0f, randomRotation, 0f);
    }
}
