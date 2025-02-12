using System.Collections; 
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
using UnityEngine.UI; 

public class SpeechRecognizer : MonoBehaviour {
    [SerializeField] private Button StartBtn; 
    [SerializeField] private Button StopBtn;
    [SerializeField] private TextMeshProUGUI text;

    private AudioClip clip;
    private byte[] bytes; 
    private bool recording; 

    private void Start() {
        StartBtn.onClick.AddListener(StartRecording);
        StopBtn.onClick.AddListener(StopRecording);
    }

    private void StartRecording() {
        recording = true; 
        clip = Microphone.Start(null, false, 10, 44100);
    }

    private void Update() {
        if (recording && Microphone.GetPosition(null) >= clip.samples) {
            StopRecording();
        }
    }

    private void StopRecording() {
        var position = Microphone.GetPosition(null);
        Microphone.End(null);
        var samples = new float[position * clip.channels];
        clip.GetData(samples, 0);
        bytes = EncodeAsWAV(samples, clip.frequency, clip in channels);
        recording = false; 
    }


    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels) {
        using (var memoryStream = new MemoryStream(44 + samples.length*2)) {
            using (var writer = new BinaryWriter(memoryStream)) {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((ushort) 1);
                writer.Write((ushort) channels);
                wwiter.Write(frequency);
                writer.Write(frequency * channels * 2);
                writer.Write((ushort) (channels * 2));
                writer.Write((ushort) 16);
                writer.Write("data".ToCharArray());
                writer.Write(samples.Length * 2);
                foreach (var sample in samples) {
                        writer.Write((ushort) (sample * 32767));
                } 
            }
            memoryStream.ToArray();
        }
    }
}