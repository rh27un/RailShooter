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
	public float stageClearBonus = 1000f;
	public float liveLeftBonus = 100f;

	protected float stageScore = 0;

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
	protected bool toggle;
	void Awake()
	{
		score = 0;
		stageScore = 0;
		hUDManager = GameObject.FindGameObjectWithTag("HUDManager").GetComponent<HUDManager>();
		hUDManager.SetScoreText(score);
	}

	// Update is called once per frame
	void Update()
	{
		if(Time.time > lastScore + (multiplierDecayDelay))
		{
			multiplier = Mathf.Max(1f, multiplier - (multiplierDecay * multiplier * Time.deltaTime));
			if (toggle)
			{
				hUDManager.SetMultiplierText(multiplier, Time.time - lastScore);
				if(multiplier == 1f)
					toggle = false;
			}
		}
	}
	public void ScoreInfo(HitInfo info)
	{
		lastScore = Time.time;
		string scoreText = string.Empty;
		float points = 50f; // base points
		scoreText += info.target + " Kill";
		switch (info.target)
		{
			case "Aenas Beamer":
				points += 100f;
				break;
			case "Aenas Commander":
				points += 90f;
				break;
			case "Aenas Railgunner":
				points += 80f;
				break;
			case "Aenas Rocketeer":
				points += 70f;
				break;
			case "Aenas":
				points += 60f;
				break;
			case "Bren Commander":
				points += 50f;
				break;
			case "Bren Grenadier":
				points += 40f;
				break;
			case "Bren Minigunner":
				points += 30f;
				break;
			case "Bren":
				points += 20f;
				break;
			case "Cokril Commander":
				points += 10f;
				break;
			default:
				break;
		}
		switch (info.targetHitbox)
		{
			case "head":
				if (info.source != "Melee" && !info.isExplosion)
				{
					scoreText += " - Headshot";
					points += 50f;
				}
				break;
			default:
				break;
		}
		switch (info.source)
		{
			case "Pistol":
				scoreText += " - Pistol Kill";
				points += 10f;
				break;
			default :
				break;
		}
		if (info.isExplosion)
		{
			scoreText += " - Explosion Kill";
			points += 20f;
		}
		if(info.distance > 50f)
		{
			scoreText += $" - Sniped: {info.distance:N2}m";
			points += Mathf.Floor(info.distance);
			if(Mathf.Floor(info.distance) == 69f)
			{
				scoreText += " - Nice!";
				points += 69f;
			}
		}
		float multipliedPoints = points * multiplier;
		newScores.Add($"{scoreText} - +{multipliedPoints:N2}");
		hUDManager.UpdateNewScoresText(newScores);
		StartCoroutine("RemoveNewScore");
		multiplier += multiplierIncrement;
		hUDManager.SetMultiplierText(multiplier, 0f);
		score += multipliedPoints;
		stageScore += multipliedPoints;
		hUDManager.SetScoreText(score);
		toggle = true;
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
		hUDManager.SetMultiplierText(multiplier, 0f);
		score += pointsToAdd;
		stageScore+= pointsToAdd;
		hUDManager.SetScoreText(score);
		toggle = true;
	}

	public void NewStage()
	{
		stageScore = 0;
	}

	public void ScorePoints(float points)
	{
		score += points;
		stageScore += points;
	}

	public void ScorePoints(float points, string message, float _multiplierIncrement)
	{
		lastScore = Time.time;
		var pointsToAdd = points * multiplier;
		newScores.Add(message + (points > 0f ? "+" + pointsToAdd.ToString() : string.Empty));
		hUDManager.UpdateNewScoresText(newScores);
		StartCoroutine("RemoveNewScore");
		multiplier += _multiplierIncrement;
		hUDManager.SetMultiplierText(multiplier, 0f);
		score += pointsToAdd;
		stageScore += pointsToAdd;
		hUDManager.SetScoreText(score);
		toggle = true;
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

	public float GetStageScore()
	{ 
		return stageScore;
	}

	public void Checkpoint(float _score, float _stageScore)
	{
		score = _score;
		stageScore = _stageScore;
		multiplier = 1f;
		newScores.Clear();
		hUDManager.SetScoreText(score);
		hUDManager.SetMultiplierText(multiplier, 0f);
		hUDManager.UpdateNewScoresText(newScores);
	}
}
