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
    public Animator animator;

    private bool fly = false;
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
            float frequency = detector.GetDominantFrequencyUsingHPS();

            // If frequency is 0, we skip applying the jump (in case there's no significant sound)
            if (frequency > 0)
            {
                // Calculate the jump strength based on frequency (low frequency = large jump, high frequency = small jump)
                float jumpStrength = CalculateJumpStrength(frequency);

                // Apply jump force based on the frequency
                body.velocity = new Vector2(jumpStrength / 2, jumpStrength);
                fly = true;
                // body.velocity = new Vector2(body.velocity.y, jumpStrength);
                   
               Debug.Log("Loudness: " + loudness + " | Frequency: " + frequency + " | Jump Strength: " + jumpStrength);
            }
        }
        animator.SetFloat("Jump", body.velocity.y);
        animator.SetFloat("Fall", body.velocity.y);
        if(isGrounded() && fly != false){
            animator.SetBool("grounded", true);
            fly = false;
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
       if(frequency <50){
        jumpStrength = 5f;
       }
       else if(frequency <100){
         jumpStrength = 7f;
       }
       else if(frequency <200){
         jumpStrength = 10f;
       }
       else if(frequency <300 || frequency > 300){
         jumpStrength = 15f;
       }
        return jumpStrength;
    }
}