using UnityEngine;

public class DroneController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float ascendSpeed = 5f;
    [SerializeField] private float descendSpeed = 3f;

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

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        BasicMovement();
        Rotate();
        Lean();
        AltitudeControl();
        HandleSpeed();
        Debug.Log(rb.velocity.magnitude);
    }

    private void BasicMovement()
    {
        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;
        Vector3 velocity = moveDirection * currentSpeed;
        rb.AddRelativeForce(velocity);
    }

    private void Rotate()
    {
        Vector3 rotation = new Vector3(-verticalInput, horizontalInput, 0f);
        transform.Rotate(rotation);
    }


    private void Lean()
    {
        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

        if (moveDirection.magnitude > 0)
        {
            Quaternion leanRotation = Quaternion.Euler(verticalInput * pitchIntensity, 0, -horizontalInput * leanIntensity);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * leanRotation, Time.deltaTime * 5f);
        }
        else
        {
            Quaternion levelRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, levelRotation, Time.deltaTime * stabilizationSpeed);
        }
    }

    private void AltitudeControl()
    {
        targetAltitude += altitudeInput * ascendSpeed * Time.deltaTime;
        targetAltitude = Mathf.Clamp(targetAltitude, 0f, float.MaxValue);

        float altitudeError = targetAltitude - transform.position.y;
        float verticalVelocity = altitudeError * ascendSpeed + Mathf.Clamp(rb.velocity.y, -descendSpeed, 0);
        rb.velocity = new Vector3(rb.velocity.x, verticalVelocity, rb.velocity.z);
    }

    private void RotateLeftRight()
    {
        if (Input.GetKey(KeyCode.K))
        {
            transform.Rotate(Vector3.up * -rotationSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.L))
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }
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
        }
    }

    public void SetInputs(float horizontal, float vertical, float attitude)
    {
        horizontalInput = horizontal;
        verticalInput = vertical;
        altitudeInput = attitude;
    }
}