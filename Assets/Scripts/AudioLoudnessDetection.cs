using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLoudnessDetection : MonoBehaviour
{
    public int sampleWindow = 256; // Number of samples for amplitude
    public int spectrumSize = 4096; // Number of samples for frequency spectrum analysis
    public float minFrequency = 50f;
    public float maxFrequency = 400f; 
    private AudioClip microphoneClip; // Microphone input

    private AudioSource audioSource;
    private int sampleRate;
    void Start()
    {
        sampleRate = AudioSettings.outputSampleRate; // Dynamically get the sample rate
        Debug.Log("Microphone sample rate: " + sampleRate); // Log the sample rate
        MicrophoneToAudioClip();
    }

    public void MicrophoneToAudioClip()
    {
        // Get the first microphone in the device list (Main)
        string microphoneName = Microphone.devices[0];

        // Record microphone input (Name, Loop, Length in seconds, Frequency of mic)
         microphoneClip = Microphone.Start(microphoneName, true, 10, sampleRate);

        // Setup an AudioSource to play back the microphone input
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = microphoneClip;
        audioSource.loop = true;
        audioSource.mute = false; // Set this to false if you want to hear the audio, otherwise keep it true for silent
        audioSource.Play();
    }

    public float GetLoudnessFromMicrophone()
    {
        if (Microphone.IsRecording(Microphone.devices[0]))
        {
            return GetLoudnessFromAudioClip(Microphone.GetPosition(Microphone.devices[0]), microphoneClip);
        }
        else
        {
            Debug.LogWarning("Microphone is not recording.");
            return 0f;
        }
    }

    public float GetLoudnessFromAudioClip(int clipPosition, AudioClip clip)
    {
        // Find which part of the audio we want to use (Where there is sound)
        int startPosition = clipPosition - sampleWindow;
        // No sound is emitted
        if (startPosition < 0)
            return 0;

        // An array made to get intensity later
        float[] waveData = new float[sampleWindow];
        clip.GetData(waveData, startPosition);

        // Compute loudness
        float totalLoudness = 0;
        for (int i = 0; i < sampleWindow; i++)
        {
            totalLoudness += Mathf.Abs(waveData[i]); // Negative values are no sound, intensity in positive numbers
        }
        // Return the loudness over the given sample window
        return totalLoudness / sampleWindow;
    }

    // New method using Harmonic Product Spectrum (HPS) for fundamental frequency detection
    public float GetDominantFrequencyUsingHPS()
    {
        if (!audioSource.isPlaying || audioSource.clip == null)
        {
            Debug.LogWarning("AudioSource is not playing or has no clip.");
            return 0f;
        }

        // Create a float array to store spectrum data
        float[] spectrumData = new float[spectrumSize];
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

        // Harmonic Product Spectrum: Downsample the spectrum multiple times and multiply them together
        int harmonics = 3; // Number of harmonics to use (HPS order)
        float[] hps = new float[spectrumSize];
        for (int i = 0; i < spectrumSize; i++)
        {
            hps[i] = spectrumData[i]; // Start with the original spectrum
        }

        // For each harmonic (downsampled version), multiply with the original spectrum
        for (int harmonic = 2; harmonic <= harmonics; harmonic++)
        {
            for (int i = 0; i < spectrumSize / harmonic; i++)
            {
                hps[i] *= spectrumData[i * harmonic]; // Multiply with the downsampled spectrum
            }
        }

        // Find the index of the largest component in the HPS array
        float maxMagnitude = 0f;
        int maxIndex = 0;
        for (int i = 0; i < spectrumSize / harmonics; i++) // Use /harmonics to avoid high frequencies beyond the downsampled range
        {
            if (hps[i] > maxMagnitude)
            {
                maxMagnitude = hps[i];
                maxIndex = i;
            }
        }

        // Convert index to frequency in Hertz
        float frequency = maxIndex * (AudioSettings.outputSampleRate / 2f) / spectrumSize;

        // Return the detected dominant frequency
       
        return frequency;
    }
}
