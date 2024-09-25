using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player Movement Script using Audio Input for Jumping
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    private Rigidbody2D body;
    private BoxCollider2D boxCollider;

    // Sound-related components
    public AudioSource source;
    public AudioLoudnessDetection detector;
    public float loudnessSensibility = 1000f;
    public float threshold = 0.0001f; // Threshold to detect actual speech, ignore too quiet sounds

    // Movement sensitivity based on frequency
    public float maxJumpStrength = 15f;  // Maximum jump for lowest frequencies
    public float minJumpStrength = 5f;   // Minimum jump for highest frequencies

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get loudness and dominant frequency from the microphone
        float rawLoudness = detector.GetLoudnessFromMicrophone();
        float loudness = rawLoudness * loudnessSensibility;

        // Only proceed if the loudness is above the threshold (someone is speaking)
        if (loudness > threshold && isGrounded())
        {
            float frequency = detector.GetDominantFrequency();

            // If frequency is 0, we skip applying the jump (in case there's no significant sound)
            if (frequency > 0)
            {
                // Calculate the jump strength based on frequency (low frequency = large jump, high frequency = small jump)
                float jumpStrength = CalculateJumpStrength(frequency);

                // Apply jump force based on the frequency
                body.velocity = new Vector2(body.velocity.y / 2, jumpStrength);
                // body.velocity = new Vector2(body.velocity.y, jumpStrength);
                Debug.Log("Loudness: " + loudness + " | Frequency: " + frequency + " | Jump Strength: " + jumpStrength);
            }
        }
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    public float CalculateJumpStrength(float frequency)
    {
        // Debug log the frequency to ensure it's varying
        Debug.Log("Frequency: " + frequency);

        // Clamp the frequency within the range of minFrequency and maxFrequency
        float clampedFrequency = Mathf.Clamp(frequency, detector.minFrequency, detector.maxFrequency);

        // Debug log the clamped frequency
        Debug.Log("Clamped Frequency: " + clampedFrequency);

        // Reverse the mapping: high frequency = big jump, low frequency = small jump
        float normalizedFrequency = Mathf.InverseLerp(detector.minFrequency, detector.maxFrequency, clampedFrequency);

        // Debug log the normalized frequency (should be between 0 and 1)
        Debug.Log("Normalized Frequency: " + normalizedFrequency);

        // Calculate the jump strength based on the reverse frequency range
        float jumpStrength = Mathf.Lerp(minJumpStrength, maxJumpStrength, normalizedFrequency);

        // Debug log the final jump strength
        Debug.Log("Calculated Jump Strength: " + jumpStrength);

        return jumpStrength;
    }
}