using System;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class DroneAgent : Agent
{
    public event Action OnNewEpisode;

    [SerializeField]
    private List<GameObject> obstacles;

    [SerializeField]
    private GameObject explosionVFX;

    private DroneController droneController;
    private Rigidbody droneRigidBody;

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
    private void CheckRayCast()
    {
        RayPerceptionSensorComponent3D m_rayPerceptionSensorComponent3D = GetComponent<RayPerceptionSensorComponent3D>();

        var rayOutputs = RayPerceptionSensor.Perceive(m_rayPerceptionSensorComponent3D.GetRayPerceptionInput(), false).RayOutputs;
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

                // Print info:
                string dispStr = "";
                dispStr = dispStr + "__RayPerceptionSensor - HitInfo__:\r\n";
                dispStr = dispStr + "GameObject name: " + goHit.name + "\r\n";
                dispStr = dispStr + "Hit distance of Ray: " + rayHitDistance + "\r\n";
                dispStr = dispStr + "GameObject tag: " + goHit.tag + "\r\n";
                Debug.Log(dispStr);
            }
        }
    }

    private void Update()
    {
        CheckRayCast();
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
            Debug.Log("No hit on enemy");
            SetReward(-1f);
            EndEpisode();
        }
    }

    private void ResetDrone()
    {
        ResetDroneVelocity();
        SetDroneRandomPosition();
        transform.localRotation = Quaternion.identity;
        droneController.ResetSpeed();
    }

    private void ResetDroneVelocity()
    {
        droneRigidBody.velocity = new Vector3(0.1f,0f);
        droneRigidBody.angularVelocity = Vector3.zero;
    }

    private void SetDroneRandomPosition()
    {
        float xPos = UnityEngine.Random.Range(-6.25f, 11.25f);
        float zPos = UnityEngine.Random.Range(-4.2f, -1.4f);
        float altitude = UnityEngine.Random.Range(0.3f, 2.65f);
        transform.localPosition = new Vector3(xPos, altitude, zPos);
        droneController.SetAltitude(altitude);
    }

    private void OnGoalAchived()
    {
        Debug.Log("Enemy hit");
        Instantiate(explosionVFX, transform.position, Quaternion.identity);
        SetReward(1f);
        //HandleRandomObstacleSpawn();
        EndEpisode();
    }
}
