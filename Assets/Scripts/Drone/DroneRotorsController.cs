using UnityEngine;

public class DroneRotorsController : MonoBehaviour
{
    [Header("Rotor Settings")]
    public GameObject[] rotors;
    public float maxRotorSpeed = 500f;
    public float minRotorSpeed = 100f;
    public float rotorSpeedMultiplier = 1f;

    private Rigidbody droneRigidbody;

    void Start()
    {
        droneRigidbody = GetComponent<Rigidbody>();

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
        float normalizedSpeed = Mathf.Clamp01(droneRigidbody.velocity.magnitude / droneRigidbody.velocity.normalized.magnitude);
        float rotorSpeed = Mathf.Lerp(minRotorSpeed, maxRotorSpeed, normalizedSpeed) * rotorSpeedMultiplier;

        for (int i = 0; i < rotors.Length; i++)
        {
            if (rotors[i] != null)
            {
                rotors[i].transform.Rotate(Vector3.forward, rotorSpeed * Time.deltaTime, Space.Self);
            }
        }
    }
}
