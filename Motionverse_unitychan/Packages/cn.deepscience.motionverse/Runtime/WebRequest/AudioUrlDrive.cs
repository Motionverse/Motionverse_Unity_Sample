using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


namespace MotionverseSDK
{
    public class AudioUrlDrive
    {
        private const string TAG = nameof(AudioUrlDrive);

        private static Dictionary<string, Drive> m_cacheDrive = new Dictionary<string, Drive>();//缓存
        private static List<DriveTask> m_waitTask = new List<DriveTask>();//等待列表
        private static List<DriveTask> m_curTask = new List<DriveTask>();//当前正在加载列表


        public static void GetDrive(DriveTask driveTask)
        {
            CastTask(driveTask);
        }
        private static async void CastTask(DriveTask driveTask)
        {
            if (driveTask == null)
            {
                if (m_waitTask.Count == 0)
                {
                    return;//没有等待下载的任务
                }
                driveTask = m_waitTask[0];
                m_waitTask.RemoveAt(0);
            }
            if (m_curTask.Count > 0)
            {
                m_waitTask.Add(driveTask);
            }
            else
            {
                m_curTask.Add(driveTask);
                WebRequestDispatcher webRequestDispatcher = new WebRequestDispatcher();
                AudioUrlMotionParams postData = new AudioUrlMotionParams()
                {
                    audio_url = driveTask.text,
                };
                postData.body_config.body_motion = driveTask.player.bodyMotion;
                postData.body_config.body_head_x_rot = driveTask.player.bodyHeadXRot;
                postData.body_config.body_compress = true;
   

                Debug.Log(JsonUtility.ToJson(postData));
                WebResponse response = await webRequestDispatcher.Post(TokenClient.Host + Config.AudioMotionUrl, JsonUtility.ToJson(postData), new CancellationTokenSource().Token);
                Debug.Log(TokenClient.Host + Config.AudioMotionUrl);

                if (response.code == 200)
                {
                    Debug.Log(response.data);
                    JObject obj = JObject.Parse(response.data);
                    if (int.Parse(obj["code"].ToString()) == 0)
                    {
                        string audioUrl = driveTask.text;
                        string faceUrl = obj["data"]["allfaceData"]["oss_url"].ToString();
                        string motionUrl = obj["data"]["motionFusionedData"]["oss_url"].ToString();
                      
                        if (!m_cacheDrive.TryGetValue(audioUrl,out Drive drive)){
                            m_cacheDrive.Add(audioUrl, new Drive());
                        }

                        AudioDownLoad(audioUrl, driveTask.player);
                        FaceDownLoad(faceUrl, audioUrl, driveTask.player);
                        MotionDownLoad(motionUrl, audioUrl, driveTask.player);
                        m_curTask.Clear();
                        CastTask(null);
                    }
                }
                else
                {
                    Debug.Log(response.msg);
                }
            }

        }

        private static async void AudioDownLoad(string audioUrl, Player player)
        {
            AudioDownloader audioDownloader = new AudioDownloader();
            CancellationToken token = new CancellationToken();
            AudioType audioType = AudioType.WAV;
            if (audioUrl.Contains("mp3") || audioUrl.Contains("MP3"))
            {
                audioType = AudioType.MPEG;
            }
            AudioClip clip = await audioDownloader.Execute(audioUrl, audioType, token);
            if (m_cacheDrive.TryGetValue(audioUrl, out Drive drive))
            {
                drive.clip= clip;
                drive.step++;
                //SDKLogger.Log(TAG, "音频下载成功");

                if(drive.step == 3)
                {
                    player.StartDrive(drive);
                }
            }

           

        }

        private static async void FaceDownLoad(string faceUrl,string key, Player player)
        {
            JsonDownloader jsonDownloader = new JsonDownloader();
            CancellationToken token = new CancellationToken();
            string data = await jsonDownloader.Execute(faceUrl, token);
            if (m_cacheDrive.TryGetValue(key, out Drive drive))
            {
                drive.bsData= data;
                drive.step++;
                //SDKLogger.Log(TAG, "动作数据下载成功");
                if (drive.step == 3)
                {
                    player.StartDrive(drive);
                }
            }
        }

        private static async void MotionDownLoad(string motionUrl, string key, Player player)
        {
            JsonDownloader jsonDownloader = new JsonDownloader();
            CancellationToken token = new CancellationToken();
            byte[] data = await jsonDownloader.ExecuteBuffer(motionUrl, token);
            if (m_cacheDrive.TryGetValue(key, out Drive drive))
            {
                drive.motionByte = data;
                drive.step++;
                //SDKLogger.Log(TAG, "动作数据下载成功");
                if(drive.step == 3)
                {
                    player.StartDrive(drive);
                }
            }
        }





    }
}
