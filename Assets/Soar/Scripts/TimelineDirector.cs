using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using SoarSDK;
using UnityEngine.Video;
using System.IO;

public class TimelineDirector : MonoBehaviour
{
    public TimelineAsset timelineAsset;
    public PlayableDirector director;

    [HideInInspector] public bool ready;
    [HideInInspector] public bool setTimes;
    [HideInInspector] public int count = 0;
    [HideInInspector] public List<string> fileNames;
    [HideInInspector] public string[] fileNamesArray;
    [HideInInspector] public List<float> timeStamps;
    [HideInInspector] public float[] timeStampsArray;
    [HideInInspector] public bool getNames;
    [HideInInspector] public bool getTimes;
    [HideInInspector] public int secondCount;
    [HideInInspector] public bool videoPlayerReady;
    [HideInInspector] public VideoPlayer videoPlayer;

    [HideInInspector] public List<PlaybackInstance> instances;
    [HideInInspector] public List<VolumetricRender> volumetricRenders;

    [HideInInspector] public List<VolumetricRenderTrack> volRenderTracks;
    [HideInInspector] public VolumetricRenderTrack[] volRenderTracksArray;

    [HideInInspector] public bool playing;
    [HideInInspector] public bool setTracks;

    [HideInInspector] public string newFilePath;

    [HideInInspector] public SetClipsFromParticipant setClipsFromParticipant;

    [HideInInspector] public bool setClipFromParticipant;

    private void Awake()
    {
        director.stopped += Director_stopped;
        director.paused += Director_paused;

        var outputTracks = timelineAsset.GetOutputTracks();

        getNames = true;
    }

    private void Director_paused(PlayableDirector obj)
    {
        var outputTracks = timelineAsset.GetOutputTracks();

        foreach (var outputTrack in outputTracks)
        {
            if (outputTrack is VolumetricRenderTrack)
            {
                VolumetricRender volRender = director.GetGenericBinding(outputTrack) as VolumetricRender;
                var index = volRender.instanceRef.IndexOf(volRender.GetComponent<PlaybackInstance>());
                volRender.PauseModel(index);
            }

        }
    }

    private void Director_stopped(PlayableDirector obj)
    {
        var outputTracks = timelineAsset.GetOutputTracks();
        foreach (var outputTrack in outputTracks)
        {
            if (outputTrack is VolumetricRenderTrack)
            {
                VolumetricRender volRender = director.GetGenericBinding(outputTrack) as VolumetricRender;
                var index = volRender.instanceRef.IndexOf(volRender.GetComponent<PlaybackInstance>());
                volRender.StopModel(index);
                VolumetricTimelineGlobals.instanceList.Clear();
            }
        }
    }

    public void GetClipNames()
    {
        var outputTracks = timelineAsset.GetOutputTracks();

        foreach (var outputTrack in outputTracks)
        {
            if (outputTrack is VolumetricRenderTrack)
            {
                var c = outputTrack.GetClips();
                foreach (var clip in c)
                {
                    VolumetricRenderClip clip1 = clip.asset as VolumetricRenderClip;
                    VolumetricRenderBehavior behavior = clip1.attributes;
                    string videoFileName = behavior.fileName.Replace("_master", "");
                    fileNames.Add(videoFileName);
                }
                fileNamesArray = fileNames.ToArray();
            }

        }
    }

    public void GetClipTimes()
    {
       
        if (!videoPlayerReady)
        {
            videoPlayer = gameObject.AddComponent<VideoPlayer>();
            videoPlayer.hideFlags = HideFlags.HideInInspector;
            videoPlayer.playOnAwake = false;
            videoPlayer.waitForFirstFrame = false;
            videoPlayer.renderMode = VideoRenderMode.CameraFarPlane;
            videoPlayer.targetCamera = Camera.main;
            videoPlayer.source = VideoSource.Url;
            if (setClipFromParticipant)
            {
                newFilePath = Path.Combine(setClipsFromParticipant.participantPath + "/" + fileNamesArray[count] + ".mp4");
            }
            else
            {
                newFilePath = Path.Combine(Application.streamingAssetsPath, "VOD/" + fileNamesArray[count] + ".mp4");
            }
            videoPlayer.url = newFilePath;
            videoPlayer.Prepare();
            videoPlayerReady = true;
        }

        if (videoPlayer.isPrepared)
        {
            videoPlayerReady = false;
            timeStamps.Add((float)videoPlayer.length);
            Destroy(videoPlayer);
            count++;
            if(count == fileNamesArray.Length)
            {
                timeStampsArray = timeStamps.ToArray();
                secondCount = 0;
                getTimes = false;
                setTracks = true;
            }
        }
    }

    private void Update()
    {

        if (getNames)
        {
            GetClipNames();
            getNames = false;
            getTimes = true;
        }

        if (getTimes)
        {
            GetClipTimes();
        }

        if (setTracks)
        {
            var outputTracks = timelineAsset.GetOutputTracks();
            var tracksArray = outputTracks.ToArray<TrackAsset>();



            for (int i = 0; i < tracksArray.Length; i++)
            {
                if (tracksArray[i] is VolumetricRenderTrack)
                {
                    volRenderTracks.Add(tracksArray[i] as VolumetricRenderTrack);
                }
            }
            volRenderTracksArray = volRenderTracks.ToArray();
            setTracks = false;
            setTimes = true;


        }

        if (setTimes)
        {
            var outputTracks = timelineAsset.GetOutputTracks();

            for(int i = 0; i < volRenderTracksArray.Length; i++)
            {
                if(volRenderTracksArray[i] is VolumetricRenderTrack)
                {

                    var c = volRenderTracksArray[i].GetClips();
                    foreach(var clip in c)
                    {
                        clip.duration = (double)timeStampsArray[secondCount];
                        secondCount++;

                        if (secondCount == volRenderTracksArray.Length)
                        {
                            setTimes = false;
                            playing = true;
                        }
                    }
                }
            }
        }

        if (playing)
        {
            instances = VolumetricTimelineGlobals.instanceList;
            volumetricRenders = VolumetricTimelineGlobals.volRenderList;
            director.Play();
            playing = false;
            //director.Evaluate();
        }
    }

    public void PlayTimeline()
    {
        director.Play();
    }

    public void StopTimeline()
    {
        director.Stop();
    }

    public void PauseTimeline()
    {
        director.Pause();
    }
}
