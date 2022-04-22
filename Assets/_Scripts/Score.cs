using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public enum ScoreType
{
	Other = 0,
	Kill = 1,
	HeadshotKill = 2
}
public class Score : MonoBehaviour
{
	protected float score = 0;
	protected float multiplier = 1;
	protected float lastScore;
	protected List<string> newScores = new List<string>();

	[SerializeField]
	protected float multiplierDecay;
	[SerializeField]
	protected float multiplierDecayDelay;
	[SerializeField]
	protected float removeScoreTimer;
	[SerializeField]
	protected float multiplierIncrement;

	[SerializeField]
	protected List<float> scoringSystem = new List<float>();

	protected HUDManager hUDManager;
	void Awake()
	{
		score = 0;
		hUDManager = GameObject.FindGameObjectWithTag("HUDManager").GetComponent<HUDManager>();
		hUDManager.SetScoreText(score);
	}

	// Update is called once per frame
	void Update()
	{
		if(Time.time > lastScore + multiplierDecayDelay)
		{
			multiplier = Mathf.Max(1f, multiplier - (multiplierDecay * Time.deltaTime));
			hUDManager.SetMultiplierText(multiplier);
		}
	}

	public void ScoreType(ScoreType type)
	{
		lastScore = Time.time;
		var points = scoringSystem[(int)type];
		var pointsToAdd = points * multiplier;
		newScores.Add((type == global::ScoreType.HeadshotKill ? "Headshot! " : string.Empty) + "+" + pointsToAdd.ToString());
		hUDManager.UpdateNewScoresText(newScores);
		StartCoroutine("RemoveNewScore");
		multiplier += multiplierIncrement;
		hUDManager.SetMultiplierText(multiplier);
		score += pointsToAdd;
		hUDManager.SetScoreText(score);
	}
	public void ScorePoints(float points)
	{

	}
	IEnumerator RemoveNewScore()
	{
		yield return new WaitForSeconds(removeScoreTimer);
		newScores.RemoveAt(0);
		hUDManager.UpdateNewScoresText(newScores);
	}

	public float GetScore()
	{
		return score;
	}

	public void Checkpoint(float _score)
	{
		score = _score;
		multiplier = 1f;
		newScores.Clear();
		hUDManager.SetScoreText(score);
		hUDManager.SetMultiplierText(multiplier);
		hUDManager.UpdateNewScoresText(newScores);
	}
}
