using MotionverseSDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Motionverse
{
    public class PlayerManager : MonoBehaviour
    {
        public Player player;
        public InputField inputField;
        public Button btnSend;

        // Start is called before the first frame update
        void Start()
        {
            btnSend.onClick.AddListener(() =>
            {
                DriveTask driveTask = new DriveTask();
                driveTask.player = player;
                driveTask.text = inputField.text;
                NLPDrive.GetDrive(driveTask);
            });
        }

    }
}
