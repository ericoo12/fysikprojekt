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
    public float threshold = 0.0001f; // Threshold to detect actual speech
    public float frequencyAnalysisInterval = 0.2f; // Analyze frequency every 0.2 seconds
    private float nextAnalysisTime = 0f;

    // Movement sensitivity based on frequency
    public float maxJumpStrength = 15f;  // Maximum jump for lowest frequencies
    public float minJumpStrength = 5f;   // Minimum jump for highest frequencies
    public Animator animator;

    private bool fly = false;
    private float jumpCooldown = 1f; // Cooldown between jumps
    private float lastJumpTime = 0f;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Wait for the next analysis time before analyzing sound
        if (Time.time >= nextAnalysisTime)
        {
            AnalyzeAudio();
            nextAnalysisTime = Time.time + frequencyAnalysisInterval; // Set the next analysis time
        }

        animator.SetFloat("Jump", body.velocity.y);
        animator.SetFloat("Fall", body.velocity.y);
        if (isGrounded() && fly)
        {
            animator.SetBool("grounded", true);
            fly = false;
        }
    }

    private void AnalyzeAudio()
    {
        // Get loudness and dominant frequency from the microphone
        float rawLoudness = detector.GetLoudnessFromMicrophone();
        float loudness = rawLoudness * loudnessSensibility;

        // Only proceed if the loudness is above the threshold (someone is speaking)
        if (loudness > threshold && isGrounded() && Time.time >= lastJumpTime + jumpCooldown)
        {
            float frequency = detector.GetDominantFrequencyUsingHPS();

            // If frequency is 0, skip applying the jump
            if (frequency > 0)
            {

                // Calculate the jump strength based on the frequency
                float jumpStrength = CalculateJumpStrength(frequency);

                // Apply jump force based on the smoothed frequency
                body.velocity = new Vector2(jumpStrength / 2, jumpStrength);
                fly = true;
                lastJumpTime = Time.time; // Update last jump time to enforce cooldown

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
        float jumpStrength = 0;
        if (frequency > 100)
        {
            jumpStrength = 5f;
        }
        else if (frequency > 150)
        {
            jumpStrength = 7f;
        }
        else if (frequency> 300)
        {
            jumpStrength = 10f;
        }
        else if (frequency >= 300)
        {
            jumpStrength = 15f;
        }
        return jumpStrength;
    }
}
