using UnityEngine;
using HuggingFace.API;
using System;

public class SpeechRecognizer : MonoBehaviour
{
    private AudioClip recordedClip;
    private bool isRecording = false;

    GameObject startButton; 
    GameObject stopButton;

    void Start()
    {
        startButton = GameObject.Find("StartBtn");
        stopButton = GameObject.Find("StopBtn");

        // Add listeners to the buttons
        if (startButton != null)
            startButton.GetComponent<Button>().onClick.AddListener(StartRecording);
        if (stopButton != null)
            stopButton.GetComponent<Button>().onClick.AddListener(StopRecording);
    }
    public void StartRecording()
    {
        if (!isRecording)
        {
            recordedClip = Microphone.Start(null, false, 10, 16000);
            isRecording = true;
        }
    }

    public void StopRecording()
    {
        if (isRecording)
        {
            Microphone.End(null);
            isRecording = false;
            ProcessAudio();
        }
    }

    private void ProcessAudio()
    {
        float[] samples = new float[recordedClip.samples];
        recordedClip.GetData(samples, 0);

        byte[] audioData = new byte[samples.Length * 2];
        for (int i = 0; i < samples.Length; i++)
        {
            short value = (short)(samples[i] * short.MaxValue);
            BitConverter.GetBytes(value).CopyTo(audioData, i * 2);
        }

        SendAudioToHuggingFace(audioData);
    }

    private void SendAudioToHuggingFace(byte[] audioData)
    {
        HuggingFaceAPI.AutomaticSpeechRecognition(audioData, OnTranscriptionSuccess, OnTranscriptionError);
    }

    private void OnTranscriptionSuccess(string transcription)
    {
        Debug.Log("Transcription: " + transcription);
        // Here you can implement your command parsing logic
    }

    private void OnTranscriptionError(string error)
    {
        Debug.LogError("Transcription Error: " + error);
    }
}
