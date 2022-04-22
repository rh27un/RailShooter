using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickup : MonoBehaviour
{
	public Gun gun;
	public int ammo;

	public void Start()
	{
		gun = Instantiate(gun);
		gun.name = gun.name.Replace("(Clone)", string.Empty);
		gun.storedAmmo = ammo;
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
