using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    private float speed = 10f;  // Speed for horizontal movement
    private float jumpForce = 10f;  // Force for jumping
    [SerializeField] private LayerMask groundLayer;
    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    
    //sound
    public AudioSource source;
    public AudioLoudnessDetection detector;
    //Helping adjust the sensitivity of the microphone
    public float loudnessSensibility = 1000f;
    public float threshold = 0.000001f;


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }
   

    // Update is called once per frame
    void Update()
    {
        body.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, body.velocity.y);

      // float loudness = detector.GetLoudnessFromMicrophone() * loudnessSensibility;

        float rawLoudness = detector.GetLoudnessFromMicrophone();
        float loudness = rawLoudness * loudnessSensibility;

        // Log the raw and adjusted loudness values
        Debug.Log("Raw Loudness: " + rawLoudness);
        Debug.Log("Loudness Sensibility: " + loudnessSensibility);
        Debug.Log("Adjusted Loudness: " + loudness);
        //Debug.Log("Loudness: " + loudness);
        if (loudness > threshold && isGrounded())
        {
            // Trigger jump based on sound loudness
            body.velocity = new Vector2(body.velocity.x, jumpForce);
        }
    }

    
    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0 , Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }
}
