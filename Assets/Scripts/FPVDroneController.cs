using UnityEngine;

public class FPVDroneController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float rotationSpeed = 100f;
    public float leanIntensity = 20f;
    public float pitchIntensity = 10f;
    public float ascendSpeed = 5f;
    public float descendSpeed = 3f;
    public float drag = 5f; // Adjust linear drag
    public float angularDrag = 5f; // Adjust angular drag
    public float stabilizationSpeed = 5f; // Adjust the speed of horizontal stabilization

    private Rigidbody rb;
    private float targetAltitude = 0f; // Target altitude for hovering
    public float defaultHoverHeight = 5f; // Adjust the desired hover height

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevent rigidbody from rotating due to physics forces
        rb.drag = drag;
        rb.angularDrag = angularDrag;
    }

    void FixedUpdate()
    {
        // Basic Movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;
        Vector3 velocity = moveDirection * moveSpeed;
        rb.AddRelativeForce(velocity);

        // Rotation
        float yaw = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, yaw, 0);

        // Lean in the direction of movement
        if (moveDirection.magnitude > 0)
        {
            Quaternion leanRotation = Quaternion.Euler(vertical * pitchIntensity, 0, -horizontal * leanIntensity);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * leanRotation, Time.deltaTime * 5f);
        }
        else
        {
            // Horizontal Stabilization
            Quaternion levelRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, levelRotation, Time.deltaTime * stabilizationSpeed);
        }

        // Altitude Control
        float altitudeInput = Input.GetAxis("Jump") - Input.GetAxis("Fire1");
        targetAltitude += altitudeInput * ascendSpeed * Time.deltaTime;
        targetAltitude = Mathf.Clamp(targetAltitude, 0f, float.MaxValue); // Ensure target altitude is not negative

        float altitudeError = targetAltitude - transform.position.y;
        float verticalVelocity = altitudeError * ascendSpeed + Mathf.Clamp(rb.velocity.y, -descendSpeed, 0);
        rb.velocity = new Vector3(rb.velocity.x, verticalVelocity, rb.velocity.z);

        // Rotate Left and Right
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
