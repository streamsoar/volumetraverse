using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using SoarSDK;
using UnityEngine.Timeline;

public class VolumetricTimelineGlobals : MonoBehaviour
{
    public static List<VolumetricRender> volRenderList = new List<VolumetricRender>();
    public static List<PlaybackInstance> instanceList = new List<PlaybackInstance>();
}

public class VolumetricRenderTrackMixer : PlayableBehaviour
{

    public PlaybackState state;
    public PlaybackInstancePlayState instanceState;

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        VolumetricRender volRender = playerData as VolumetricRender;
        if (!volRender.GetComponent<PlaybackInstance>()) { return; }
        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            if (inputWeight > 0f)
            {
                ScriptPlayable<VolumetricRenderBehavior> inputPlayable = (ScriptPlayable<VolumetricRenderBehavior>)playable.GetInput(i);
                VolumetricRenderBehavior input = inputPlayable.GetBehaviour();
                PlayableGraph playableGraph = playable.GetGraph();
                PlayableDirector director = playable.GetGraph().GetResolver() as PlayableDirector;

                for (int j = 0; j < VolumetricTimelineGlobals.instanceList.Count; j++)
                {
                    state = volRender.GetInstanceState(j);
                    Texture colorTexture = volRender.GetComponent<MeshRenderer>().material.GetTexture("_CameraRGB");
                    PlaybackInstancePlayState instanceState = volRender.GetComponent<PlaybackInstance>()._playState;
                    bool textureNull = Object.ReferenceEquals(colorTexture, null);

                    switch (textureNull)
                    {
                        case true:
                            break;
                        default:
                            if(instanceState == PlaybackInstancePlayState.Playing)
                            {
                                volRender.GetComponent<MeshRenderer>().enabled = true;
                            }
                            //director.time += 0;
                            break;
                    }

                }

                if (director.state == PlayState.Paused)
                {

                    if (input.fileName.Length > 0)
                    {
                        if (input.clipLoaded)
                        {
                            if (input.clipPlaying)
                            {
                                var index = volRender.instanceRef.IndexOf(volRender.GetComponent<PlaybackInstance>());
                                volRender.SeekToCursor(index, (int)(inputPlayable.GetTime() * 1000000.0f));
                                input.seeking = true;
                            }

                            if (!input.clipPlaying)
                            {
                                var index = volRender.instanceRef.IndexOf(volRender.GetComponent<PlaybackInstance>());
                                PlaybackInstance instance = volRender.GetComponent<PlaybackInstance>();
                                volRender.GetComponent<MeshRenderer>().GetPropertyBlock(instance.props);
                                instance.props.Clear();
                                volRender.GetComponent<MeshRenderer>().SetPropertyBlock(instance.props);
                                volRender.LoadNewClip(input.fileName, index);
                                var newIndex = volRender.instanceRef.IndexOf(instance);
                                volRender.SeekToCursor(newIndex, (int)(inputPlayable.GetTime() * 1000000.0f));
                                PlaybackInstance newInstance = volRender.GetComponent<PlaybackInstance>();
                                VolumetricTimelineGlobals.instanceList.Add(newInstance);
                                input.seeking = true;
                            }
                        }

                        if (!input.clipLoaded)
                        {
                            var index = volRender.instanceRef.IndexOf(volRender.GetComponent<PlaybackInstance>());
                            PlaybackInstance instance = volRender.GetComponent<PlaybackInstance>();
                            volRender.GetComponent<MeshRenderer>().GetPropertyBlock(instance.props);
                            instance.props.Clear();
                            volRender.GetComponent<MeshRenderer>().SetPropertyBlock(instance.props);
                            volRender.LoadNewClip(input.fileName, index);
                            var newIndex = volRender.instanceRef.IndexOf(instance);
                            volRender.SeekToCursor(newIndex, (int)(inputPlayable.GetTime() * 1000000.0f));
                            PlaybackInstance newInstance = volRender.GetComponent<PlaybackInstance>();
                            VolumetricTimelineGlobals.instanceList.Add(newInstance);
                            input.seeking = true;
                        }
                    }


                }

                if (director.state == PlayState.Playing)
                {
                    //var index = volRender.instanceRef.IndexOf(volRender.GetComponent<PlaybackInstance>());

                    if (input.fileName.Length > 0)
                    {
                        if (input.clipLoaded)
                        {
                            if (input.clipPlaying)
                            {
                                if (input.seeking)
                                {
                                    var index = volRender.instanceRef.IndexOf(volRender.GetComponent<PlaybackInstance>());
                                    volRender.SeekToCursor(index, (int)(inputPlayable.GetTime() * 1000000.0f));
                                    volRender.StartPlayback(index);
                                    input.seeking = false;
                                }

                                if (!input.seeking)
                                {
                                    if (!input.clipStarted)
                                    {
                                        var index = volRender.instanceRef.IndexOf(volRender.GetComponent<PlaybackInstance>());
                                        volRender.StartPlayback(index);
                                        input.clipStarted = true;
                                    }
                                }
                            }

                            if (!input.clipPlaying)
                            {

                                if (!input.seeking)
                                {
                                    var index = volRender.instanceRef.IndexOf(volRender.GetComponent<PlaybackInstance>());
                                    PlaybackInstance instance = volRender.GetComponent<PlaybackInstance>();
                                    volRender.GetComponent<MeshRenderer>().GetPropertyBlock(instance.props);
                                    instance.props.Clear();
                                    volRender.GetComponent<MeshRenderer>().SetPropertyBlock(instance.props);
                                    volRender.GetComponent<MeshRenderer>().material.SetTexture("_CameraRGB", null);
                                    volRender.LoadNewClip(input.fileName, index);
                                    var newIndex = volRender.instanceRef.IndexOf(instance);
                                    PlaybackInstance newInstance = volRender.GetComponent<PlaybackInstance>();
                                    VolumetricTimelineGlobals.instanceList.Add(newInstance);
                                    input.clipPlaying = true;
                                }

                                if (input.seeking)
                                {
                                    VolumetricTimelineGlobals.instanceList.Clear();
                                    var index = volRender.instanceRef.IndexOf(volRender.GetComponent<PlaybackInstance>());
                                    PlaybackInstance instance = volRender.GetComponent<PlaybackInstance>();
                                    volRender.GetComponent<MeshRenderer>().GetPropertyBlock(instance.props);
                                    instance.props.Clear();
                                    volRender.GetComponent<MeshRenderer>().SetPropertyBlock(instance.props);
                                    volRender.LoadNewClip(input.fileName, index);
                                    var newIndex = volRender.instanceRef.IndexOf(instance);
                                    volRender.SeekToCursor(newIndex, (int)(inputPlayable.GetTime() * 1000000.0f));
                                    PlaybackInstance newInstance = volRender.GetComponent<PlaybackInstance>();
                                    Debug.Log(newInstance._playState);
                                    VolumetricTimelineGlobals.instanceList.Add(newInstance);
                                    input.clipPlaying = true;
                                }
                            }
                        }
                    }
                }
            }

            if (inputWeight == 0f)
            {
                ScriptPlayable<VolumetricRenderBehavior> inputPlayable = (ScriptPlayable<VolumetricRenderBehavior>)playable.GetInput(i);
                VolumetricRenderBehavior input = inputPlayable.GetBehaviour();
                PlayableDirector director = playable.GetGraph().GetResolver() as PlayableDirector;

                if (director.state == PlayState.Paused)
                {
                    if (input.clipPlaying)
                    {
                        PlaybackInstance instance = volRender.GetComponent<PlaybackInstance>();
                        VolumetricTimelineGlobals.instanceList.Remove(instance);
                    }
                    input.clipPlaying = false;
                    input.clipStarted = false;
                }

                if (director.state == PlayState.Playing)
                {
                    if (input.clipPlaying)
                    {
                        PlaybackInstance instance = volRender.GetComponent<PlaybackInstance>();
                        VolumetricTimelineGlobals.instanceList.Remove(instance);
                        instance.Close();
                    }
                    input.clipPlaying = false;
                    input.clipStarted = false;
                }

            }
        }
    }
}
