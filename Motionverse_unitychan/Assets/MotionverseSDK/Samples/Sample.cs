using MotionverseSDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sample : MonoBehaviour
{
    public Player Player;
    public void OnTextDrive(string text)
    {
        DriveTask driveTask = new DriveTask() { player = Player ,text = text};
        TextDrive.GetDrive(driveTask);
    }

    public void OnAudioUrlDrive(string url)
    {
        DriveTask driveTask = new DriveTask() { player = Player, text = url };
        AudioUrlDrive.GetDrive(driveTask);
    }


    public void OnNLPDrive(string text)
    {
        DriveTask driveTask = new DriveTask() { player = Player, text = text };
        NLPDrive.GetDrive(driveTask);
    }

}
