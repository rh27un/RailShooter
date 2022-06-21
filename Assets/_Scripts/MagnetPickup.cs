using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetPickup : MonoBehaviour
{
	protected Magnet magnet;
	[SerializeField]
	protected float time;
	private void Start()
	{
		magnet = GameObject.FindGameObjectWithTag("Player").GetComponent<Magnet>();
	}

	public void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			magnet.StartMagnet(time);
			Destroy(gameObject);
		}
	}
}
