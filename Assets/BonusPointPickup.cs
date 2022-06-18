using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusPointPickup : MonoBehaviour
{
	[SerializeField]
	protected int pointsToAdd;

	[SerializeField]
	protected float multiplierToAdd;

	[SerializeField]
	protected string message;
	public void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			GameObject.FindGameObjectWithTag("GameController").GetComponent<Score>().ScorePoints(pointsToAdd, message, multiplierToAdd);
			Destroy(gameObject);
		}
	}
}
