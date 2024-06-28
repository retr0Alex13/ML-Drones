using UnityEngine;

public class DroneController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float ascendSpeed = 5f;
    [SerializeField] private float descendSpeed = 3f;


    [Header("Rotation Settings")]
    [SerializeField] private float yawSpeed = 100f;
    [SerializeField] private float rotationSmoothness = 0.1f;

    [Space(10)]

    [Header("Speed Control")]
    [SerializeField] private float maxSpeed = 200f;
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float deceleration = 5f;

    [Space(10)]

    [Header("Drone Lean")]
    [SerializeField] private float leanIntensity = 20f;
    [SerializeField] private float pitchIntensity = 10f;
    [SerializeField] private float stabilizationSpeed = 5f;


    private Rigidbody rb;
    private float targetAltitude;
    private float currentSpeed;

    private float horizontalInput;
    private float verticalInput;
    private float altitudeInput;
    private float smoothedYawInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        targetAltitude = transform.position.y; // Initialize targetAltitude with the initial y-position
    }

    private void FixedUpdate()
    {
        HandleRotation();
        HandleMovement();
        Lean();
        AltitudeControl();
        HandleSpeed();
    }

    private void HandleMovement()
    {
        // Calculate the movement direction in world space
        Vector3 moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;
        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        // Apply movement force
        Vector3 movement = moveDirection * currentSpeed;
        rb.AddForce(movement, ForceMode.Acceleration);
    }

    private void HandleRotation()
    {
        // Smooth the yaw input
        smoothedYawInput = Mathf.Lerp(smoothedYawInput, horizontalInput, 1f / rotationSmoothness);

        // Apply yaw rotation
        float yawRotation = smoothedYawInput * yawSpeed * Time.fixedDeltaTime;
        transform.Rotate(Vector3.up * yawRotation, Space.World);
    }


    private void Lean()
    {
        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

        if (moveDirection.magnitude > 0)
        {
            Quaternion leanRotation = Quaternion.Euler(verticalInput * pitchIntensity, 0, -horizontalInput * leanIntensity);
            transform.localRotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * leanRotation, Time.deltaTime * 5f);
        }
        else
        {
            Quaternion levelRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            transform.localRotation = Quaternion.Slerp(transform.rotation, levelRotation, Time.deltaTime * stabilizationSpeed);
        }
    }

    private void AltitudeControl()
    {
        targetAltitude += altitudeInput * ascendSpeed * Time.deltaTime;
        targetAltitude = Mathf.Clamp(targetAltitude, transform.position.y, float.MaxValue); // Clamp targetAltitude with the initial y-position as the minimum

        float altitudeError = targetAltitude - transform.position.y;
        float verticalVelocity = altitudeError * ascendSpeed + Mathf.Clamp(rb.velocity.y, -descendSpeed, 0);
        rb.velocity = new Vector3(rb.velocity.x, verticalVelocity, rb.velocity.z);
    }

    private void HandleSpeed()
    {
        if (Mathf.Abs(horizontalInput) > 0 || Mathf.Abs(verticalInput) > 0)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime);
            ResetSpeed();
        }
    }

    public void ResetSpeed()
    {
        currentSpeed = 0f;
    }

    public void SetAltitude(float altitude)
    {
        targetAltitude = altitude;
    }

    public void SetInputs(float horizontal, float vertical, float attitude)
    {
        horizontalInput = horizontal;
        verticalInput = vertical;
        altitudeInput = attitude;
    }
}