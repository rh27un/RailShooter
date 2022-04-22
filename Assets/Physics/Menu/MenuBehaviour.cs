using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class MenuBehaviour : PlayableBehaviour
{
    public Transform mainTrans;
    public Transform playTrans;
    public Transform optionTrans;

    public override void OnPlayableCreate (Playable playable)
    {
        
    }
}
