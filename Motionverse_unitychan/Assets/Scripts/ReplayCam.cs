
using UnityEngine;
using UnityEngine.UI;
using NatSuite.Recorders;
using NatSuite.Recorders.Inputs;
using NatSuite.Recorders.Clocks;

public class ReplayCam : MonoBehaviour
{

    [Header(@"Recording")]
    public int videoWidth = 1280;
    public int videoHeight = 720;
    //public bool recordMicrophone;
    public Camera mainCamera;
    private IMediaRecorder recorder;
    private CameraInput cameraInput;
    private AudioInput audioInput;
    //private AudioSource microphoneSource;
    public AudioListener audioListener;
    public AudioSource audioSource;
    private bool isRecording = false;

    private void Awake()
    {
        Screen.SetResolution(videoWidth, videoHeight, false);
    }

    /// <summary>
    /// 设置屏幕分辨率
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void SetResolutions(int width, int height)
    {
        Screen.SetResolution(videoWidth, videoHeight, false);
        videoHeight = height;
        videoWidth = width;
    }

    private void OnDestroy()
    {
        // Stop microphone
        //microphoneSource.Stop();
        Microphone.End(null);
    }

    /// <summary>
    /// 开始录制
    /// </summary>
    public void StartRecording()
    {
        // Start recording
        var frameRate = 30;
        var sampleRate = AudioSettings.outputSampleRate;
        var channelCount = (int)AudioSettings.speakerMode;
        var clock = new RealtimeClock();
        recorder = new MP4Recorder(videoWidth, videoHeight, frameRate, sampleRate, channelCount);
        // Create recording inputs
        cameraInput = new CameraInput(recorder, clock, mainCamera);
        audioInput = new AudioInput(recorder, clock, audioListener);
        // Unmute microphone
        //microphoneSource.mute = true;
    }

    /// <summary>
    /// 结束录制
    /// </summary>
    public async void StopRecording()
    {
        // Mute microphone
        //microphoneSource.mute = true;
        // Stop recording
        audioInput?.Dispose();
        cameraInput.Dispose();
        var path = await recorder.FinishWriting();
        // Playback recording
        Debug.Log($"Saved recording to: {path}");
    }

    private void Update()
    {
        if (audioSource.clip != null)
        {
            if (!isRecording)
            {
                isRecording = true;
                StartRecording();
            }
        }
        else
        {
            if (isRecording)
            {
                isRecording = false;
                StopRecording();
            }
        }
    }

}