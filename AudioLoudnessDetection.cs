using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLoudnessDetection : MonoBehaviour
{
    public int sampleWindow = 64;
    private AudioClip microphoneClip; // Microphone input
    void Start()
    {
        //Access microphone at start.
        MicrophoneToAudioClip();
    }

    void Update()
    {

    }

    public void MicrophoneToAudioClip()
    {
        //Get the first microphone in device list (Main)
        string microphoneName = Microphone.devices[0];
        //(Name, On, Value high enough (still loops), Frequency of mic)
        microphoneClip = Microphone.Start(microphoneName, true, 20, AudioSettings.outputSampleRate);
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
        //Find which part of the audio we want to use (Where there is sound)
        int startPosition = clipPosition - sampleWindow;
        //No sound is emitted
        if (startPosition < 0)
            return 0;
        //An array made to get intensity later
        float[] waveData = new float[sampleWindow];
        clip.GetData(waveData, startPosition);

        //compute loudness
        float totalLoudness = 0;
        

        for (int i = 0; i < sampleWindow; i++)
        {
            totalLoudness += Mathf.Abs(waveData[i]); // negative values is no sound, intensity in positive numbers.
            
        }
        //Return the loudness over the given sample window which impacts the character in the y-axis later.
        return totalLoudness / sampleWindow;
    }
}