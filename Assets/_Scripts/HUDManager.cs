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

	[SerializeField]
	protected GameObject bossPanel;
	[SerializeField]
	protected TextMeshProUGUI bossText;
	[SerializeField]
	protected Image bossBar;

	[SerializeField]
	protected GameObject reloadBG;
	[SerializeField]
	protected Image reloadBar;
	protected float reloadStart;
	protected float reloadEnd;
	protected bool isReloading;

	[SerializeField]
	protected GameObject chargeBG;
	[SerializeField]
	protected Image chargeBG2;
	[SerializeField]
	protected Image chargeBar;
	protected float chargeStart;
	protected float chargeEnd;
	protected bool isCharging;

	[SerializeField]
	protected float inaccuracyMod;
	[SerializeField]
	protected RectTransform upCross;
	[SerializeField]
	protected RectTransform downCross;
	[SerializeField]
	protected RectTransform leftCross;
	[SerializeField]
	protected RectTransform rightCross;

	[SerializeField]
	protected TMP_Text stageScoreText;
	[SerializeField]
	protected TMP_Text stageBonusText;
	[SerializeField]
	protected TMP_Text livesLeftText;
	[SerializeField]
	protected TMP_Text livesBonusText;
	[SerializeField]
	protected TMP_Text totalScoreText;
	[SerializeField]
	protected GameObject header;
	[SerializeField]
	protected GameObject stageScoreLabel;
	[SerializeField]
	protected GameObject stageBonusLabel;
	[SerializeField]
	protected GameObject livesLeftLabel;
	[SerializeField]
	protected GameObject totalScoreLabel;
	[SerializeField]
	protected GameObject continueButton;

	protected RectTransform multiplierTransform;

	protected bool animatingMultiplier;
	protected Vector2 multiplierStartPos;
	protected Vector2 multiplierEndPos;
	protected Quaternion multiplierStartRot;
	protected Quaternion multiplierEndRot;
	protected Color multiplierStartColor;
	protected Color multiplierEndColor;
	protected float multiplierTimeScale = 1f;
	protected float mutliplierAnimStart;
	protected float tension;

	protected bool stageEnded;

	[SerializeField]
	protected Score scoreKeeper;
	protected float stageScore;
	protected float totalScore;
	protected float livesLeft;
	//public bool IsFirstToLoad { get; set; }

	public TMP_Text stageName;
	public TMP_Text stageDescription;
	public Color textTargetColor;
	public Color textStartColor;
	public float textColorNow;
	private void Awake()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
		multiplierTransform = multiplierText.GetComponent<RectTransform>();
		multiplierStartPos = multiplierTransform.anchoredPosition;
		textTargetColor = Color.white;
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		int stageIndex = scene.buildIndex;
		if (stageIndex < 3)
			return;
		textStartColor = new Color(1f, 1f, 1f, 0f);
		textTargetColor = Color.white;
		textColorNow = 0f;
		stageName.text = $"Stage {stageIndex - 2} - {scene.name}";
		switch(stageIndex)
		{
			case 3:
				stageDescription.text = "Get on the train!";
				break;
			default:
				stageDescription.text = string.Empty;
				break;
		}

	}
	private void Update()
	{
		if(fadeImage.color != targetColor)
		{
			fadeImage.color = Color.Lerp(fadeImage.color, targetColor, fadeTime);
		}

		if(stageName.color != textTargetColor)
		{
			textColorNow += Time.deltaTime;
			Color color = Color.Lerp(textStartColor, textTargetColor, textColorNow);
			stageName.color = color;
			stageDescription.color = color;
		} else if(stageName.color == Color.white)
		{
			StartCoroutine("TextFadeOut");
		}

		if (isReloading)
		{
			float t = (Time.time - reloadStart) / (reloadEnd - reloadStart);
			if (t <= 1)
			{
				reloadBar.fillAmount = t;
			} else
			{
				isReloading = false;
				reloadBG.SetActive(false);
			}
		}
		if (isCharging)
		{
			float t = (Time.time - chargeStart) / (chargeEnd - chargeStart);
			if (t <= 1)
			{
				chargeBar.fillAmount = t;
			}
			else
			{
				isCharging = false;
				chargeBG.SetActive(false);
			}
		}
		if (animatingMultiplier)
		{
			float t = (Time.time - mutliplierAnimStart) / (multiplierTimeScale);
			if(t <= 1)
			{
				multiplierTransform.anchoredPosition = Vector2.Lerp(multiplierStartPos, multiplierEndPos, t);
				multiplierTransform.rotation = Quaternion.Lerp(multiplierStartRot, multiplierEndRot, t);
				multiplierText.color = Color.Lerp(multiplierStartColor, multiplierEndColor, t);
			} else
			{
				animatingMultiplier = false;
				multiplierTransform.anchoredPosition = multiplierStartPos;
				multiplierTransform.rotation = multiplierStartRot;
				multiplierText.color = multiplierStartColor;
				multiplierText.text = string.Empty;
			}
		} else
		{
			multiplierTransform.anchoredPosition = multiplierStartPos + (Random.insideUnitCircle * tension * 2f);
			tension += Time.deltaTime;
		}
	}
	
	public IEnumerator TextFadeOut()
	{
		yield return new WaitForSeconds(2f);
		textStartColor = Color.white;
		textTargetColor = new Color(1f, 1f, 1f, 0f);
		textColorNow = 0f;
	}

	protected void CancelMultiplierAnimation()
	{
		if (animatingMultiplier)
		{
			multiplierTransform.anchoredPosition = multiplierStartPos;
			multiplierTransform.rotation = multiplierStartRot;
			multiplierText.color = multiplierStartColor;
			animatingMultiplier = false;
		}
	}

	protected void AnimateMultiplier()
	{
		if (animatingMultiplier)
		{
			multiplierTransform.anchoredPosition = multiplierStartPos;
			multiplierTransform.rotation = multiplierStartRot;
			multiplierText.color = multiplierStartColor;
		}
		multiplierStartPos = multiplierTransform.anchoredPosition;
		multiplierStartRot = multiplierTransform.rotation;
		multiplierEndRot = Quaternion.Euler(multiplierStartRot.eulerAngles + new Vector3(0f, 0f, Random.Range(-20f, 20f)));
		multiplierEndPos = multiplierStartPos + new Vector2(Random.Range(-10f, 10f), Random.Range(-100f, -70f));
		multiplierStartColor = multiplierText.color;
		multiplierEndColor = new Color(1f, 1f, 1f, 0f);
		mutliplierAnimStart = Time.time;
		animatingMultiplier = true;
	}

	public void StartEndScreen()
	{
		StartCoroutine("AnimateEndScreen");
	}

	protected IEnumerator AnimateEndScreen()
	{
		header.SetActive(true);
		SetColor(Color.black);
		stageEnded = true;
		yield return new WaitForSecondsRealtime(1f);
		Time.timeScale = 0f;
		yield return new WaitForSecondsRealtime(0.5f);
		stageScoreLabel.SetActive(true);
		stageScoreText.gameObject.SetActive(true);
		stageScoreText.text = "00000000000";
		yield return new WaitForSecondsRealtime(0.5f);
		totalScoreLabel.SetActive(true);
		totalScoreText.gameObject.SetActive(true);
		totalScoreText.text = "00000000000";
		yield return new WaitForSecondsRealtime(0.5f);
		//
		stageBonusLabel.SetActive(true);
		stageBonusText.gameObject.SetActive(true);
		yield return new WaitForSecondsRealtime(0.5f);
		livesLeftLabel.SetActive(true);
		livesLeftText.gameObject.SetActive(true);
		yield return new WaitForSecondsRealtime(0.5f);

		//var tenth = scoreKeeper.GetStageScore() / 27;
		var twenth = scoreKeeper.GetScore() / 27;
		for(int i = 1; i <= 27; i++)
		{
			stageScoreText.text = Mathf.Min((twenth * i), scoreKeeper.GetStageScore()).ToString("00000000000");
			totalScoreText.text = (twenth * i).ToString("00000000000");
			yield return new WaitForSecondsRealtime(0.05f);
		}
		yield return new WaitForSecondsRealtime(0.5f);
		scoreKeeper.ScorePoints(scoreKeeper.stageClearBonus); 
		stageBonusText.text = scoreKeeper.stageClearBonus.ToString("0");
		stageScoreText.text = scoreKeeper.GetStageScore().ToString("00000000000");
		totalScoreText.text = scoreKeeper.GetScore().ToString("00000000000");
		yield return new WaitForSecondsRealtime(0.5f);
		float lifeBonus = 0f;
		while(livesLeft > 0)
		{
			lifeBonus += scoreKeeper.liveLeftBonus;
			livesLeft--;
			livesLeftText.text = livesLeft.ToString();
			livesBonusText.gameObject.SetActive(true);
			livesBonusText.text = $"+ {lifeBonus}";
			scoreKeeper.ScorePoints(scoreKeeper.liveLeftBonus);
			stageScoreText.text = scoreKeeper.GetStageScore().ToString("00000000000");
			totalScoreText.text = scoreKeeper.GetScore().ToString("00000000000");
			yield return new WaitForSecondsRealtime(0.5f);
		}
		yield return new WaitForSecondsRealtime(0.5f);
		continueButton.SetActive(true);
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

	public void EndEndScreen()
	{
		stageEnded = false;
		header.SetActive(false);
		stageScoreLabel.SetActive(false);
		stageScoreText.gameObject.SetActive(false);
		totalScoreLabel.SetActive(false);
		totalScoreText.gameObject.SetActive(false);
		stageBonusLabel.SetActive(false);
		stageBonusText.gameObject.SetActive(false);
		livesLeftLabel.SetActive(false);
		livesLeftText.gameObject.SetActive(false);
		livesBonusText.gameObject.SetActive(false);
		continueButton.SetActive(false);
		SetColor(Color.clear);
		scoreKeeper.NewStage();
	}

	public void StartReload(float _reloadTime)
	{
		reloadBG.SetActive(true);
		reloadStart = Time.time;
		reloadEnd = Time.time + _reloadTime;
		isReloading = true;
	}
	public void EndReload()
	{
		reloadBG.SetActive(false);
		isReloading = false;
	}

	public void StartCharge(float _chargeMin, float _chargeMax)
	{
		chargeBG.SetActive(true);
		chargeStart = Time.time;
		chargeEnd = Time.time + _chargeMax;
		chargeBG2.fillAmount = _chargeMin / _chargeMax;
		isCharging = true;
	}
	public void EndCharge()
	{
		chargeBG.SetActive(false);
		isCharging = false;
	}
	public void SetColor(Color _targetColor)
	{
		if(stageEnded) return;
		targetColor = _targetColor;
	}
	public void SetColor(Color _currentColor, Color _targetColor)
	{
		fadeImage.color = _currentColor;
		targetColor = _targetColor;
	}

	public void SetInaccuracy(float _inaccuracy)
	{
		float modI = _inaccuracy * inaccuracyMod;
		upCross.anchoredPosition = new Vector2(0f, modI);
		downCross.anchoredPosition = new Vector2(0f, -modI);
		leftCross.anchoredPosition = new Vector2(-modI, 0f);
		rightCross.anchoredPosition = new Vector2(modI, 0f);
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
		healthText.text = Mathf.Max(value, 1).ToString("N0");
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
		livesLeftText.text = Mathf.Max(lives, 0).ToString();
		livesLeft = lives;
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

	public void SetMultiplierText(float multiplier, float _tension)
	{
		tension = _tension;
		if (multiplier > 1f)
		{
			CancelMultiplierAnimation();
			multiplierText.text = $"x{multiplier:N2}";
		} else
		{
			AnimateMultiplier();
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
		totalScore = score;
		stageScore = score;
	}

	public void StartBossFight(string bossName)
	{
		bossPanel.SetActive(true);
		bossText.text = bossName;
		bossBar.fillAmount = 1f;
	}

	public void SetBossHealth(float health)
	{
		bossBar.fillAmount = health;
	}

	public void EndBossFight()
	{
		bossPanel.SetActive(false);
	}
}
