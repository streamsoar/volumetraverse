using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using SoarSDK;
using UnityEngine.Video;
using System.IO;

[Serializable]
public class VolumetricRenderBehavior : PlayableBehaviour
{
    [SerializeField]
    public string fileName;

    internal bool clipPlaying;
    internal bool clipLoaded;
    internal bool clipStarted;
    internal bool seeking;

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        clipLoaded = true;
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        clipLoaded = false;
    }
}
