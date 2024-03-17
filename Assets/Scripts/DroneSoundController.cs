using System.Collections;
using UnityEngine;

public class DroneSoundController : MonoBehaviour
{
    [SerializeField] private float changePitchAmount = 1.5f;
    [SerializeField] private float speedToChangePitch = 5f;
    [SerializeField] private Rigidbody droneRigidBody;

    private float defaultPitch;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        defaultPitch = audioSource.pitch;
    }

    private void Update()
    {
        // Calculate the velocity magnitude of the rigidbody
        float velocityMagnitude = droneRigidBody.velocity.magnitude;

        // Calculate the target pitch based on the velocity magnitude
        float targetPitch = velocityMagnitude > speedToChangePitch ? defaultPitch * changePitchAmount : defaultPitch;

        // Smoothly change the pitch of the audio source
        float currentPitch = audioSource.pitch;
        float pitchChangeSpeed = 1f; // Adjust this value to control the speed of pitch change
        
        // Smoothly change the pitch of the audio source
        audioSource.pitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * pitchChangeSpeed);
    }
}
