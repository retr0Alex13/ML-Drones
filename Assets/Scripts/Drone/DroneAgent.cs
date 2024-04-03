using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class DroneAgent : Agent
{
    [SerializeField] private ProjectileController projectile;
    [SerializeField] private Enemy enemy;
    [SerializeField] private Collider spawnBoundsCollider;

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
        enemy.ResetEnemy();
        // Randomize surroundings
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(transform.rotation);
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

    private void ResetDrone()
    {
        transform.localRotation = Quaternion.identity;
        ResetDroneVelocity();
        SetDroneRandomPosition();
    }

    private void ResetDroneVelocity()
    {
        droneRigidBody.velocity = new Vector3(0.01f, 0f);
        droneRigidBody.angularVelocity = Vector3.zero;
    }

    private void SetDroneRandomPosition()
    {
        transform.localPosition = new Vector3(
            Random.Range(spawnBoundsCollider.bounds.min.x, spawnBoundsCollider.bounds.max.x),
            Random.Range(spawnBoundsCollider.bounds.min.y, spawnBoundsCollider.bounds.max.y),
            Random.Range(spawnBoundsCollider.bounds.min.z, spawnBoundsCollider.bounds.max.z));
        transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Projectile"))
        {
            return;
        }
        if (droneRigidBody.velocity.magnitude > 10f)
        {
            List<Collider> objects = projectile.Explode();
            Debug.Log(objects);
            if(objects == null)
            {
                return;
            }
            else
            {
                foreach (Collider obj in objects)
                {
                    Debug.Log(obj.gameObject.name);
                    if (obj.transform.TryGetComponent(out Enemy enemy))
                    {
                        AddReward(5f);
                        EndEpisode();
                        return;
                    }
                }
            }
            Debug.Log("No Enemies");
            AddReward(-1f);
            EndEpisode();
        }
        else if (collision.transform.CompareTag("Obstacle"))
        {
            Debug.Log("Hit Obstacle");
            OnObstacleHit();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.transform.CompareTag("Obstacle"))
        {
            OnObstacleHit();
        }
    }

    public void OnObstacleHit()
    {
        Debug.Log("Hit Obstacle");
        AddReward(-1f);
        //EndEpisode();
    }
}
