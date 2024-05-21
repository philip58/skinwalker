using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.Video;

public class VidPlayer : MonoBehaviour
{
    [SerializeField] string videoFileName;
    private VideoPlayer videoPlayer;
    
    public bool playScare = false;
    // Start is called before the first frame update
    void Start()
    {
        PlayVideo();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PlayVideo(){
        videoPlayer = GetComponent<VideoPlayer>();
    }

    public void Scare(){
        if(videoPlayer && playScare)
        {
            videoPlayer.frame = 0;
            string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
            Debug.Log(videoPath);
            videoPlayer.url = videoPath;
            videoPlayer.frame = 0;
            videoPlayer.Play();
        }
    }

}
