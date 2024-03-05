using UnityEngine;

public class DroneController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float ascendSpeed = 5f;
    [SerializeField] private float descendSpeed = 3f;

    [Header("Control Intensity")]
    [SerializeField] private float leanIntensity = 20f;
    [SerializeField] private float pitchIntensity = 10f;
    [SerializeField] private float stabilizationSpeed = 5f;

    [Header("Altitude Settings")]
    [SerializeField] private float hoverHeight = 5f;

    private Rigidbody rb;
    private float targetAltitude = 0f;

    private void Start()
    {
        InitializeRigidbody();
    }

    private void FixedUpdate()
    {
        BasicMovement();
        Rotate();
        Lean();
        AltitudeControl();
        RotateLeftRight();
    }

    private void InitializeRigidbody()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void BasicMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;
        Vector3 velocity = moveDirection * moveSpeed;
        rb.AddRelativeForce(velocity);
    }

    private void Rotate()
    {
        float yaw = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, yaw, 0);
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
}
