using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
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
        //body.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, body.velocity.y);

        // float loudness = detector.GetLoudnessFromMicrophone() * loudnessSensibility;

        float rawLoudness = detector.GetLoudnessFromMicrophone();
        float loudness = rawLoudness * loudnessSensibility;

        // Log the raw and adjusted loudness values

        //Debug.Log("Raw Loudness: " + rawLoudness);
        // Debug.Log("Loudness Sensibility: " + loudnessSensibility);

        //Debug.Log("Loudness: " + loudness);
        if (loudness > threshold && isGrounded() && loudness < 20)
        {
            // Trigger jump based on sound loudness
            body.velocity = new Vector2(body.velocity.x, loudness / 2);
            body.velocity = new Vector2(body.velocity.y, loudness);
            Debug.Log("Adjusted Loudness: " + loudness);
        }
        else if (loudness > threshold && isGrounded() && loudness > 20)
        {
            body.velocity = new Vector2(body.velocity.x, 15 / 2);
            body.velocity = new Vector2(body.velocity.y, 15);
        }
    }


    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }
}
