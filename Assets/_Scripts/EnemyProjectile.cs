﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
	public float damage;

	private void OnTriggerEnter(Collider other)
	{
		if (other.isTrigger)
			return;
		var health = other.gameObject.GetComponent<PlayerHealth>();

		if(health != null)
		{
			health.Damage(damage);
		}

		Destroy(gameObject);
	}
}
