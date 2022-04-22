using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
	[SerializeField]
	protected TMP_Text healthText;
	[SerializeField]
	protected TMP_Text gunText;
	[SerializeField]
	protected TMP_Text ammoText;
	[SerializeField]
	protected TMP_Text livesText;
	[SerializeField]
	protected RawImage fadeImage;
	protected Color targetColor;
	[SerializeField]
	protected GameObject continueObject;
	[SerializeField]
	protected TMP_Text continueAmountText;
	[SerializeField]
	protected TMP_Text continueTimerText;
	[SerializeField]
	public TextMeshProUGUI scoreText;
	[SerializeField]
	public TextMeshProUGUI newScoreText;
	[SerializeField]
	public TextMeshProUGUI multiplierText;
	[SerializeField]
	protected float fadeTime;

	//public bool IsFirstToLoad { get; set; }

	private void Awake()
	{
		//SceneManager.sceneLoaded += OnSceneLoaded;
	}
	private void Update()
	{
		if(fadeImage.color != targetColor)
		{
			fadeImage.color = Color.Lerp(fadeImage.color, targetColor, fadeTime);
		}
	}

	public void SetColor(Color _targetColor)
	{
		targetColor = _targetColor;
	}
	public void SetColor(Color _currentColor, Color _targetColor)
	{
		fadeImage.color = _currentColor;
		targetColor = _targetColor;
	}

	//void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	//{
	//	if (scene.name == "Init" || !IsFirstToLoad)
	//		return;
	//	healthText = GameObject.Find("HealthValue").GetComponent<TMP_Text>();
	//	gunText = GameObject.Find("GunName").GetComponent<TMP_Text>();
	//	ammoText = GameObject.Find("AmmoValue").GetComponent<TMP_Text>();
	//	livesText = GameObject.Find("LivesValue").GetComponent<TMP_Text>();
	//	fadeImage = GameObject.Find("FadeImage").GetComponent<RawImage>();
	//	continueObject = GameObject.Find("ContinuePanel");
	//	continueAmountText = GameObject.Find("ContinueAmount").GetComponent<TMP_Text>();
	//	continueTimerText = GameObject.Find("ContinueTimer").GetComponent<TMP_Text>();
	//	continueObject.SetActive(false);
	//	targetColor = Color.clear;
	//	fadeImage.color = Color.black;
	//}

	private void OnDisable()
	{
		//SceneManager.sceneLoaded -= OnSceneLoaded;
	}
	public void SetHealthText(float value)
	{
		healthText.text = Mathf.Max(value, 0).ToString("N0");
	}

	public void SetGunText(string value)
	{
		gunText.text = value;
	}
	public void SetAmmoText(int ammo, int extra)
	{
		ammoText.text = $"{ammo.ToString()} / {extra.ToString()}";
	}

	public void SetLivesText(int lives)
	{
		livesText.text = Mathf.Max(lives, 0).ToString();
	}

	public void ShowContinueScreen()
	{
		continueObject.SetActive(true);
	}

	public void HideContinueScreen()
	{
		continueObject.SetActive(false);
	}

	public void SetContinueAmount(int amount)
	{
		continueAmountText.text = amount == -1 ? string.Empty : $"{amount} Credits";
	}

	public void SetContinueTimer(int time)
	{
		continueTimerText.text = time.ToString();
	}

	public void SetMultiplierText(float multiplier)
	{
		if (multiplier > 1f)
		{
			multiplierText.text = $"x{multiplier:N2}";
		} else
		{
			multiplierText.text = string.Empty;
		}
	}
	public void UpdateNewScoresText(List<string> newScores)
	{
		newScoreText.text = string.Empty;
		foreach (string score in newScores)
		{
			newScoreText.text = string.Concat($"{score}!\n", newScoreText.text);
		}
	}
	public void SetScoreText(float score)
	{
		scoreText.text = score.ToString("N0");
	}
}
