using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLoudnessDetection : MonoBehaviour
{
    public int sampleWindow = 256; // Number of samples for amplitude
    public int spectrumSize = 4096; // Number of samples for frequency spectrum analysis
    public float minFrequency = 50f; // Minimum frequency for higher jump
    public float maxFrequency = 1000f; // Maximum frequency for smaller jump
    private AudioClip microphoneClip; // Microphone input

    private AudioSource audioSource;

    void Start()
    {
        // Access microphone at start.
        MicrophoneToAudioClip();
    }

    public void MicrophoneToAudioClip()
    {
        // Get the first microphone in the device list (Main)
        string microphoneName = Microphone.devices[0];

        // Record microphone input (Name, Loop, Length in seconds, Frequency of mic)
        microphoneClip = Microphone.Start(microphoneName, true, 20, AudioSettings.outputSampleRate);

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

    public float GetDominantFrequency()
    {
        if (!audioSource.isPlaying || audioSource.clip == null)
        {
            Debug.LogWarning("AudioSource is not playing or has no clip.");
            return 0f;
        }

        // Create a float array to store spectrum data
        float[] spectrumData = new float[spectrumSize];

        // Get frequency spectrum data from the audio source
        audioSource.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

        // Find the index of the largest frequency component
        float maxMagnitude = 0f;
        int maxIndex = 0;
        for (int i = 0; i < spectrumSize; i++)
        {
            if (spectrumData[i] > maxMagnitude)
            {
                maxMagnitude = spectrumData[i];
                maxIndex = i;
            }
        }

        // If the maxMagnitude is too low, it could be background noise or silence
        if (maxMagnitude < 0.0001f)
        {
            return 0f; // Consider it no significant sound detected
        }

        // Convert index to frequency in Hertz
        float frequency = maxIndex * (AudioSettings.outputSampleRate / 2f) / spectrumSize;

        return frequency;
    }
}