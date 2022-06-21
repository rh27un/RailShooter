using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour
{
	public Gun gun;
	public int ammo;
	protected Magnet magnet;
	protected Vector3 startPos;

	public void Start()
	{
		gun = Instantiate(gun);
		gun.name = gun.name.Replace("(Clone)", string.Empty);
		gun.storedAmmo = ammo;
		magnet = GameObject.FindGameObjectWithTag("Player").GetComponent<Magnet>();
	}

	public void Update()
	{
		if (magnet.magnet)
		{
			transform.position = Vector3.Lerp(transform.position, magnet.transform.position, 0.1f);
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			other.GetComponent<Shooting>().PickUpWeapon(gun);
			Destroy(gameObject);
		}
	}
}
