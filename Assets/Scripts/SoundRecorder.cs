using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class SoundRecorder : MonoBehaviour
{

    AudioSource audioSource;

    //Microphone input
    public AudioClip audioClip;
    public bool useMicrophone = false;
    public string selectedDevice;

    public float[] samples = new float[512];
    public float[] samplesDB = new float[128];
    public static float intensity = 0;

    // Use this for initialization
    void Start()
    {
        audioSource = GetComponent<AudioSource>();


        // Prints all microphones found in the device.
        for (int i = 0; i < Microphone.devices.Length; i++)
        {
            Debug.Log(Microphone.devices[i].ToString());
        }

        if (useMicrophone)
        {
            if (Microphone.devices.Length > 0)
            {
                selectedDevice = Microphone.devices[0].ToString();
                audioSource.clip = Microphone.Start(selectedDevice, true, 10, AudioSettings.outputSampleRate);
            }
            else
            {
                useMicrophone = false;
            }
        }
        if (!useMicrophone)
        {
            audioSource.clip = audioClip;
        }
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource();
    }

    void GetSpectrumAudioSource()
    {
        audioSource.GetSpectrumData(samples, 0, FFTWindow.Hamming);

        audioSource.GetOutputData(samplesDB, 0);

        float sum = 0;
        for (int i = 0; i < samplesDB.Length; i++)
        {
            sum += samplesDB[i] * samplesDB[i];
        }
        float RMS = sum / samplesDB.Length; //Root-Mean-Squared.
        float RefValue = samplesDB.Length / (float)AudioSettings.outputSampleRate;
        intensity = 20 * Mathf.Log10(RMS / RefValue);
    }
}