using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class DroneAgent : Agent
{
    private DroneController droneController;
    private Rigidbody droneRigidBody;

    void Start()
    {
        droneController = GetComponent<DroneController>();
        droneRigidBody = GetComponent<Rigidbody>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(transform.rotation);
        sensor.AddObservation(droneRigidBody.velocity.magnitude);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
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
}
