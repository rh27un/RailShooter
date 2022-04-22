using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "stageData", menuName = "Stages/Stage", order = 1)]
public class Stage : ScriptableObject
{
	public AudioClip music;
	public bool isBoss;
	public List<AnimationCurve> spawnCurves = new List<AnimationCurve>();
	public Dictionary<string, AnimationCurve> test = new Dictionary<string, AnimationCurve>();
	public float spawnScale;
}
