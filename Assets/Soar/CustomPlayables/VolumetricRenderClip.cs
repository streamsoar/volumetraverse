using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class VolumetricRenderClip : PlayableAsset
{

    [SerializeField]
    public VolumetricRenderBehavior attributes = new VolumetricRenderBehavior();

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<VolumetricRenderBehavior>.Create(graph, attributes);
        return playable;
    } 

}
