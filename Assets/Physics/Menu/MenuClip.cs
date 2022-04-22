using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class MenuClip : PlayableAsset, ITimelineClipAsset
{
    public MenuBehaviour template = new MenuBehaviour ();
    public ExposedReference<Transform> mainTrans;
    public ExposedReference<Transform> playTrans;
    public ExposedReference<Transform> optionTrans;

    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<MenuBehaviour>.Create (graph, template);
        MenuBehaviour clone = playable.GetBehaviour ();
        clone.mainTrans = mainTrans.Resolve (graph.GetResolver ());
        clone.playTrans = playTrans.Resolve (graph.GetResolver ());
        clone.optionTrans = optionTrans.Resolve (graph.GetResolver ());
        return playable;
    }
}
