using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomlessClipPickup : MonoBehaviour
{
	[SerializeField]
	protected float time;
	[SerializeField]
	protected string message;

	public void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			GameObject.FindGameObjectWithTag("GameController").GetComponent<Score>().ScorePoints(0f, message, 0f);
			other.GetComponent<Shooting>().BottomlessClip(time);
			Destroy(gameObject);
		}
	}
}
