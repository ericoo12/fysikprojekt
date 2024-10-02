using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calibrate : MonoBehaviour
{
    public AudioLoudnessDetection detector; // Reference to the AudioLoudnessDetection component
    public KeyCode detectHighFreqKey = KeyCode.H; // Key to start detecting high frequencies
    public KeyCode detectLowFreqKey = KeyCode.L; // Key to start detecting low frequencies
    private bool detectingHighFreq = false; // Track if high frequency detection is active
    private bool detectingLowFreq = false; // Track if low frequency detection is active

    void Update()
    {
        // Check if the key for detecting high frequencies is held down
        if (Input.GetKey(detectHighFreqKey))
        {
            detectingHighFreq = true;
            detectingLowFreq = false; // Stop detecting low frequencies
            float highFrequency = detector.GetDominantFrequencyUsingHPS();
            if (highFrequency > 0)
            {
                Debug.Log("Detected High Frequency: " + highFrequency);
            }
        }
        else
        {
            detectingHighFreq = false; // Reset detection when the key is released
        }

        // Check if the key for detecting low frequencies is held down
        if (Input.GetKey(detectLowFreqKey))
        {
            detectingLowFreq = true;
            detectingHighFreq = false; // Stop detecting high frequencies
            float lowFrequency = detector.GetDominantFrequencyUsingHPS();
            if (lowFrequency > 0)
            {
                Debug.Log("Detected Low Frequency: " + lowFrequency);
            }
        }
        else
        {
            detectingLowFreq = false; // Reset detection when the key is released
        }
    }
}