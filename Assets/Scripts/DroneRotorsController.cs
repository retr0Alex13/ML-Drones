using UnityEngine;

public class DroneRotorsController : MonoBehaviour
{
    [Header("Rotor Settings")]
    public GameObject[] rotors;
    public float maxRotorSpeed = 500f; // Maximum rotor speed when the drone is at maximum speed
    public float minRotorSpeed = 100f; // Minimum rotor speed when the drone is stationary
    public float rotorSpeedMultiplier = 1f; // Multiplier to control the overall rotor speed

    private Rigidbody droneRigidbody;

    void Start()
    {
        droneRigidbody = GetComponent<Rigidbody>();

        // Ensure the number of rotors matches the number of rotor speeds
        if (rotors.Length == 0)
        {
            Debug.LogError("No rotors assigned!");
        }
    }

    void Update()
    {
        RotateRotors();
    }

    void RotateRotors()
    {
        // Calculate rotor speed based on drone velocity
        float normalizedSpeed = Mathf.Clamp01(droneRigidbody.velocity.magnitude / droneRigidbody.velocity.normalized.magnitude);
        float rotorSpeed = Mathf.Lerp(minRotorSpeed, maxRotorSpeed, normalizedSpeed) * rotorSpeedMultiplier;

        for (int i = 0; i < rotors.Length; i++)
        {
            if (rotors[i] != null)
            {
                // Rotate the rotor based on the calculated rotor speed
                rotors[i].transform.Rotate(Vector3.forward, rotorSpeed * Time.deltaTime);
            }
        }
    }
}
