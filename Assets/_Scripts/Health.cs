using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Completely redo this to work with damage boxes
public class Health : MonoBehaviour
{
	public float maxHealth;
	protected float curHealth { get; set; }


	public float CurHealth
	{
		get { return curHealth; }
	}
	protected bool burning = false;
	protected float fireDamage;
	protected float firstBurn;
	protected float lastBurn;
	protected float burnTime;

	public string enemyType;

	void Start()
	{
		curHealth = maxHealth;
	}
	void Update()
	{
		if (burning)
		{
			if (Time.time - lastBurn >= 1f)
			{
				//Debug.Log("Burning!");
				Damage(fireDamage);
				lastBurn = Time.time;
				if (Time.time - firstBurn >= burnTime)
				{
					//Debug.Log("Not Burning!");
					burning = false;
				}
			}
		}
	}
	public void SetFire(float _fireDamage, float _burnTime)
	{
		if (!burning)
		{
			burning = true;
			fireDamage = _fireDamage;
			burnTime = _burnTime;
			firstBurn = Time.time;
			//Debug.Log("Burning!");
			Damage(fireDamage);
			lastBurn = Time.time;
		}
		else
		{
			firstBurn = Time.time;
		}
	}
	public virtual void Damage(float _damage)
	{
		// TODO: Particle effects, sound
		curHealth -= _damage;
		if (curHealth < 0f)
		{
			Die();
		}
	}
	public void Damage(float _damage, string hitFrom)
	{
		curHealth -= _damage;
		if(curHealth < 0f)
		{
			Die(hitFrom);
		}
	}
	public void Heal(float _health)
	{
		curHealth = Mathf.Clamp(curHealth + _health, 0f, maxHealth);
	}

	public virtual void Die()
	{
		// TODO: Instantiate ragdoll, play sound
		gameObject.GetComponent<Enemy>()?.DropWeapon();
		GameObject.FindGameObjectWithTag("GameController").GetComponent<Score>().ScoreType(ScoreType.Kill);
		GameObject.FindGameObjectWithTag("GameController").GetComponent<TrainController>().Die(enemyType);
		Destroy(gameObject);
	}

	public void Die(string hitFrom)
	{
		gameObject.GetComponent<Enemy>()?.DropWeapon();
		GameObject.FindGameObjectWithTag("GameController").GetComponent<Score>().ScoreType(hitFrom == "head" ? ScoreType.HeadshotKill : ScoreType.Kill);
		GameObject.FindGameObjectWithTag("GameController").GetComponent<TrainController>().Die(enemyType);
		Destroy(gameObject);
	}

}
