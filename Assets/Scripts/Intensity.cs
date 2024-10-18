using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intensity : MonoBehaviour
{
    float intensity;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        intensity = MicrophoneRecorder.intensity;
        if (intensity > -1000 && intensity < 1000)
            transform.position = new Vector3(intensity / 1.0f, 0, 0);
    }

}