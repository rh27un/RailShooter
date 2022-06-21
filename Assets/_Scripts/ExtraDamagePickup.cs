using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraDamagePickup : MonoBehaviour
{
	[SerializeField]
	protected float damageMod;
	[SerializeField]
	protected float time;
	[SerializeField]
	protected string message;

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			GameObject.FindGameObjectWithTag("GameController").GetComponent<Score>().ScorePoints(0f, message, 0f);
			other.GetComponent<Shooting>().ExtraDamage(damageMod, time);
			Destroy(gameObject);
		}
	}
}
