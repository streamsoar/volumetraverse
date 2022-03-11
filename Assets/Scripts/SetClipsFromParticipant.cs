using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SoarSDK;
using UnityEngine.Timeline;
using UnityEngine.Playables;

[ExecuteInEditMode]
public class SetClipsFromParticipant : MonoBehaviour
{

    public string participantName;
    public bool setClips;

    [HideInInspector] public List<string> clipNames;
    [HideInInspector] public string[] clipNamesArray;

    [HideInInspector] public int index = 0;
    [HideInInspector] public string participantPath;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (setClips)
        {
            PlayableDirector director = gameObject.GetComponent<PlayableDirector>();
            TimelineAsset timelineAsset = director.playableAsset as TimelineAsset;
            participantPath = Application.dataPath + "/Participants" + "/" + participantName;
            if (System.IO.Directory.Exists(participantPath) && participantName.Length > 0)
            {
                DirectoryInfo di = new DirectoryInfo(participantPath);
                FileInfo[] files = di.GetFiles("*_master.m3u8");
                foreach (FileInfo file in files)
                {
                    clipNames.Add(file.Name);
                }

                clipNamesArray = clipNames.ToArray();
                var outputTracks = timelineAsset.GetOutputTracks();

                foreach (var outputTrack in outputTracks)
                {
                    if (outputTrack is VolumetricRenderTrack)
                    {
                        var c = outputTrack.GetClips();
                        VolumetricRender volRender = director.GetGenericBinding(outputTrack) as VolumetricRender;
                        volRender.participantFilePath = participantPath + "/" + clipNamesArray[index];
                        foreach (var clip in c)
                        {
                            VolumetricRenderClip clip1 = clip.asset as VolumetricRenderClip;
                            VolumetricRenderBehavior behavior = clip1.attributes;
                            clipNamesArray[index] = clipNamesArray[index].Replace(".m3u8", "");
                            behavior.fileName = clipNamesArray[index];
                            index++;
                        }
                    }

                }


            }
            else
            {
                Debug.Log("Participant not found, please check spelling");
            }
            clipNames.Clear();
            Array.Clear(clipNamesArray, 0, clipNamesArray.Length);
            Array.Resize(ref clipNamesArray, 0);
            index = 0;
            setClips = false;
        }
        
    }
}
