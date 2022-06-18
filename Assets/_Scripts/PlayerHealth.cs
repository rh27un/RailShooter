using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : Health
{
	[SerializeField]
	protected HUDManager hUDManager;
	[SerializeField]
	protected Color hurtColor;
	[SerializeField]
	protected Color invincibleColor;
	[SerializeField]
	protected int lives;
	public int maxLives;
	[SerializeField]
	public int continues;
	[SerializeField]
	protected bool isDying;
	protected bool isInvincible;
	protected bool canContinue;
	protected float continueTime;
	protected int continueTimer;
	public bool infiniteContinues;

	protected Transform spawnPoint;

	protected Serializer serializer;
	private void Awake()
	{
		hUDManager = GameObject.FindGameObjectWithTag("HUDManager").GetComponent<HUDManager>();
		serializer = GameObject.FindGameObjectWithTag("GameController").GetComponent<Serializer>();
		spawnPoint = GameObject.FindGameObjectWithTag("Respawn").transform;
		var difficulty = GameObject.Find("Difficulty").GetComponent<Difficulty>();

		maxLives = difficulty.lives;
		lives = difficulty.lives;
		continues = difficulty.infiniteContinues ? -1 : difficulty.continues;
		infiniteContinues = difficulty.infiniteContinues;
		hUDManager.SetLivesText(lives);
	}
	private void Update()
	{
		if (canContinue)
		{
			continueTimer = Mathf.FloorToInt(11 - (Time.time - continueTime));
			hUDManager.SetContinueTimer(continueTimer);
			if(continueTimer <= 0)
			{
				GoToLeaderBoard();
			}
			if (Input.GetKeyDown(KeyCode.Space))
			{
				hUDManager.HideContinueScreen();
				if(continues > 0 || infiniteContinues)
				{
					lives = maxLives;
					hUDManager.SetLivesText(lives);
					hUDManager.SetContinueAmount(continues);
					curHealth = maxHealth;
					hUDManager.SetHealthText(curHealth);
					canContinue = false;
					isDying = false;
					StartCoroutine("BeInvincible");
					serializer.LoadCheckpoint();
					GetComponent<FPSCharacter>().enabled = true;
				} 
				else
				{
					GoToLeaderBoard();
				}
			}
		}
	}

	protected void GoToLeaderBoard()
	{
		serializer.DeleteCheckpoint();
		SceneManager.LoadScene("Leaderboard");
		hUDManager.HideContinueScreen();
		Destroy(gameObject);

	}

	public override void Damage(float _damage)
	{
		// TODO: Particle effects, sound
		if (isDying || isInvincible)
			return;
		curHealth -= _damage;
		hUDManager.SetHealthText(curHealth);
		hUDManager.SetColor(hurtColor, Color.clear);
		if (curHealth < 0f)
		{
			Die();
		}
	}
	public void Heal(float _health, bool _cheat = false)
	{
		if (!_cheat)
		{
			curHealth = Mathf.Min(curHealth + _health, maxHealth);
			hUDManager.SetHealthText(curHealth);
		} else
		{
			curHealth += _health;
			hUDManager.SetHealthText(curHealth);
		}
		hUDManager.SetColor(Color.green, Color.clear);
	}

	public override void Die()
	{
		if (isDying)
			return;
		hUDManager.SetColor(Color.black);
		StartCoroutine("DieSlowly");
		StopCoroutine("BeInvincible");
	}

	public void LoseBoss()
	{
		if (isDying)
			return;
		hUDManager.SetColor(Color.black);
		StartCoroutine("LoseSlowly");
		StopCoroutine("BeInvincible");
	}
	protected IEnumerator LoseSlowly()
	{
		isDying = true;
		GetComponent<FPSCharacter>().enabled = false;
		yield return new WaitForSeconds(1f);
		continueTime = Time.time;
		canContinue = true;
		hUDManager.ShowContinueScreen();
		hUDManager.SetContinueAmount(continues);
	}
	protected IEnumerator DieSlowly()
	{
		lives--;
		hUDManager.SetLivesText(lives);
		isDying = true;
		GetComponent<FPSCharacter>().enabled = false;
		if (lives >= 0)
		{
			yield return new WaitForSeconds(1f);
			curHealth = maxHealth;
			hUDManager.SetHealthText(curHealth);
			transform.position = spawnPoint.position;
			transform.rotation = spawnPoint.rotation;
			isDying = false;
			GetComponent<FPSCharacter>().enabled = true;
			StartCoroutine("BeInvincible");
		} 
		else
		{
			yield return new WaitForSeconds(1f);
			continueTime = Time.time;
			canContinue = true;
			hUDManager.ShowContinueScreen();
			hUDManager.SetContinueAmount(continues);
		}
	}

	protected IEnumerator BeInvincible()
	{
		isInvincible = true;
		hUDManager.SetColor(invincibleColor);
		yield return new WaitForSeconds(5f);
		hUDManager.SetColor(Color.clear);
		isInvincible = false;
	}

	public void Respawn(int _maxLives, int _continues)
	{
		maxLives = _maxLives;
		lives = maxLives;
		hUDManager.SetLivesText(lives);
		continues = _continues;
		if (continues == -1)
			infiniteContinues = true;
	}
}
