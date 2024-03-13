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
        RotateLeftRight();
        HandleSpeed();
    }

    private void Update()
    {
        Debug.Log(rb.velocity.magnitude);
    }

    private void BasicMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;
        Vector3 velocity = moveDirection * currentSpeed;
        rb.AddRelativeForce(velocity);
    }

    private void Rotate()
    {
        float horizontal = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        float vertical = Input.GetAxis("Vertical") * rotationSpeed * Time.deltaTime;

        Vector3 rotation = new Vector3(-vertical, horizontal, 0f);
        transform.Rotate(rotation);
    }


    private void Lean()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        if (moveDirection.magnitude > 0)
        {
            Quaternion leanRotation = Quaternion.Euler(vertical * pitchIntensity, 0, -horizontal * leanIntensity);
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
        float altitudeInput = Input.GetAxis("Jump") - Input.GetAxis("Fire1");
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
    float horizontalInput = Input.GetAxis("Horizontal");
    float verticalInput = Input.GetAxis("Vertical");

    // Accelerate or decelerate based on input
    if (Mathf.Abs(horizontalInput) > 0 || Mathf.Abs(verticalInput) > 0)
    {
        // Accelerate towards the maximum speed
        currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
    }
    else
    {
        // Decelerate to zero when no input is given
        currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime);
    }
}

}
