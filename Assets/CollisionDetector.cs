using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
	public TrainController train;
	public FPSCharacter character;
	public PlayerHealth health;
	private void Start()
	{
		train = GameObject.FindGameObjectWithTag("GameController").GetComponent<TrainController>();
		character = transform.parent.GetComponent<FPSCharacter>();
		health = transform.parent.GetComponent<PlayerHealth>();
	}
	private void OnCollisionEnter(Collision collision)
	{
		Debug.Log(collision.GetContact(0).normal.x);
		if (collision.collider.tag != "Train" && collision.collider.tag != "TrainFlat" && collision.collider.tag != "Enemy")
		{
			if (collision.GetContact(0).normal.x > 0.1f && train.GetCurSpeed() - character.GetFriction().x > 0.1f)
			{
				health.Damage((train.GetCurSpeed() - character.GetFriction().x) * 2.5f);
			}
		}
	}
}
