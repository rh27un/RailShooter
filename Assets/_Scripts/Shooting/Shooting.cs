﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Shooting : MonoBehaviour
{
	[SerializeField]
	protected HUDManager hUDManager;
	public Vector3 hiddenGunStockpile;
	protected FPSCharacter characterScript;
	public List<Gun> guns;
	protected List<GameObject> gunObjects;
	protected Gun gun;
	protected int gunIndex;
	public GameObject projectilePrefab;
	public GameObject hitmarkerPrefab;
	public GameObject missmarkerPrefab;
	public Transform gunLoc;
	protected bool reloading = false;
	protected float lastFire;
	protected bool isCharger;
	protected bool charging = false;
	protected float chargeStart;
	[SerializeField]
	protected float spread;
	[SerializeField]
	protected float spreadDecay = 0.1f;
	[SerializeField]
	protected float spreadMod;
	protected int layermask = 1 << 10 | 1 << 11;
	public Gun melee;  
	public string gunsToLoad;
	protected Pause pause;
	void Start()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
		hUDManager = GameObject.FindGameObjectWithTag("HUDManager").GetComponent<HUDManager>();
		pause = GameObject.FindGameObjectWithTag("GameController").GetComponent<Pause>();
		gunIndex = 0;
		gunObjects = new List<GameObject>();
		for (int i = 0; i < guns.Count; i++)
		{
			gunObjects.Add(Instantiate(guns[i].gunPrefab, hiddenGunStockpile, Quaternion.identity));
			guns[i].curAmmo = guns[i].maxAmmo;
			guns[i].storedAmmo = 999;
		}
		gun = guns[gunIndex];


		layermask = ~layermask;
		characterScript = GetComponent<FPSCharacter>();
		characterScript.SwitchWeapons(gunObjects[gunIndex], hiddenGunStockpile, gun.aimedFOV);

		gunLoc = gunObjects[gunIndex].transform;
		gun.curAmmo = gun.maxAmmo;
		isCharger = (gun.damageCharge || gun.fireDamageCharge || gun.inaccuracyCharge || gun.multiShotCharge || gun.recoilCharge || gun.splashRangeCharge);
		spread = gun.inaccuracy;
		hUDManager.SetGunText(gun.name);
		hUDManager.SetAmmoText(gun.curAmmo, gun.storedAmmo);
		projectilePrefab = gun.projectilePrefab;
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		Serializer serializer = GameObject.FindGameObjectWithTag("GameController").GetComponent<Serializer>();

		if (mode == LoadSceneMode.Single && serializer.guns != string.Empty)
		{
			Clear();

			foreach (string gun in serializer.guns.Split(','))
			{
				string[] data = gun.Split(':');
				string gunName = data[0];
				var gunData = serializer.loadableGuns.SingleOrDefault(g => g.name == gunName);
				if (gunData != null)
				{
					string curAmmoString = data[1].Split('/')[0];
					string storedAmmoString = data[1].Split('/')[1];

					int curAmmo;
					int storedAmmo;

					if (int.TryParse(curAmmoString, out curAmmo) && int.TryParse(storedAmmoString, out storedAmmo))
					{
						var newGun = Instantiate(gunData);
						newGun.name = newGun.name.Replace("(Clone)", string.Empty);
						newGun.curAmmo = curAmmo;
						newGun.storedAmmo = storedAmmo;

						LoadWeapon(newGun);
					}
					else
					{
						Debug.LogError("Error parsing string: " + gun);
					}
				}
				else
				{
					Debug.LogError("No gun called " + gunName + " found!");
				}

			}
			SwitchWeapon(0);
		}
		hUDManager.SetGunText(gun?.name ?? string.Empty);
		hUDManager.SetAmmoText(gun?.curAmmo ?? 0, gun?.storedAmmo ?? 0);
	}
	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	void Update()
	{
		if (Input.GetButton("Fire1") && !pause.isPause)
		{
			if (!isCharger)
			{
				if (gun.curAmmo > 0 && !reloading)
				{
					if (Time.time > lastFire + gun.fireRate)
					{
						Fire(gun.multiShot, gun.damage, gun.splashRange, gun.recoil, spread, gun.fireDamage, gun.range, gun.penetrateAmount);
						if (characterScript.aiming)
						{
							if (characterScript.moveType == MoveType.Crouching)
							{
								spread += (gun.aimedRecoil * 0.7f);
							}
							else
							{
								spread += gun.aimedRecoil;
							}
						}
						else
						{
							switch (characterScript.moveType)
							{
								case MoveType.Walking:
									if (characterScript.IsMoving())
									{
										spread += (gun.recoil * 1.05f);
									}
									else
									{
										spread += gun.recoil;
									}
									break;
								case MoveType.Crouching:
									spread += (gun.recoil * 0.7f);
									break;
								case MoveType.Sliding:
									spread += gun.recoil;
									break;
								case MoveType.Sprinting:
									spread += (gun.recoil * 1.1f);
									break;
								default:
									break;
							}
						}
						//characterScript.Recoil(gun.recoil);
					}
				} 
				else if(Input.GetButtonDown("Fire1") && !reloading)
				{
					if (gun.storedAmmo > 0)
					{
						StartCoroutine("Reload");
					}
					else
					{
						//Debug.Log("No ammo!");
					}
				}
			}
		}
		else if (Input.GetButtonDown("Melee"))
		{
			RaycastHit hit;
			if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, melee.range, layermask))
			{
				//Debug.Log(hit.collider.gameObject);
				if (hit.collider.gameObject.GetComponent<Health>())
				{
					Destroy(Instantiate(hitmarkerPrefab, hit.point, Quaternion.identity), 5f);
					hit.collider.gameObject.GetComponent<Health>().Damage(melee.damage);
				} else if (hit.collider.gameObject.GetComponent<Hitbox>())
				{
					Destroy(Instantiate(hitmarkerPrefab, hit.point, Quaternion.identity), 5f);
					HitInfo info = new HitInfo() { distance = 0f, isExplosion = false, source = "Melee" };
					hit.collider.gameObject.GetComponent<Hitbox>().Damage(melee.damage, info);
				}
				else
				{
					Destroy(Instantiate(missmarkerPrefab, hit.point, Quaternion.identity), 5f);

				}
			}
		}
		else
		{
			if (Input.GetButtonDown("Reload") && !reloading)
			{
				if (gun.storedAmmo > 0)
				{
					StartCoroutine("Reload");
				}
				else
				{
					Debug.Log("No ammo!");
				}
			}
			var d = Input.GetAxis("SwitchWeapons");
			if (d > 0)
			{
				SwitchWeapons(true);
			}
			else if (d < 0)
			{
				SwitchWeapons(false);
			}
		}
		if (isCharger)
		{
			if (charging)
			{
				if (Time.time - chargeStart >= gun.maxChargeTime)
				{
					charging = false;
					ChargeFire(gun.maxChargeTime);
				}
				if (Input.GetButtonUp("Fire1"))
				{
					charging = false;
					ChargeFire(Time.time - chargeStart);
				}
			}
			else if (Input.GetButtonDown("Fire1"))
			{
				if (gun.curAmmo > 0 && !reloading)
				{
					if (Time.time > lastFire + gun.fireRate)
					{
						hUDManager.StartCharge(gun.minChargeTime, gun.maxChargeTime);
						charging = true;
						chargeStart = Time.time;
					}
				}
			}
		}
		spread = Mathf.Max(spread - (gun.recoilDecay * Time.deltaTime), 0f);
		if (characterScript.aiming)
		{
			hUDManager.SetInaccuracy(gun.aimedInaccuracy + spread);
		}
		else
		{
			hUDManager.SetInaccuracy(gun.inaccuracy + spread);
		}
	}
	IEnumerator Reload()
	{
		//Debug.Log("Reloading!");
		reloading = true;
		hUDManager.StartReload(gun.reloadTime);
		yield return new WaitForSeconds(gun.reloadTime);
		//Debug.Log("Done!");
		if (!gun.infiniteAmmo)
		{
			if (gun.storedAmmo > (gun.maxAmmo - gun.curAmmo))
			{
				gun.storedAmmo -= (gun.maxAmmo - gun.curAmmo);
				gun.curAmmo = gun.maxAmmo;

				hUDManager.SetAmmoText(gun.curAmmo, gun.storedAmmo);
			}
			else
			{
				gun.curAmmo += gun.storedAmmo;
				gun.storedAmmo = 0;

				hUDManager.SetAmmoText(gun.curAmmo, gun.storedAmmo);
			}
		}
		else
		{
			gun.curAmmo = gun.maxAmmo;
			hUDManager.SetAmmoText(gun.curAmmo, gun.storedAmmo);
		}
		reloading = false;
	}
	Vector3 RandomDirection(Transform _t, float _inaccuracy)
	{
		Vector3 newDir = Random.insideUnitCircle * (_inaccuracy) * spreadMod;
		newDir.z = 20f;
		//newDir.y += _inaccuracy;
		return _t.TransformDirection(newDir.normalized);
	}
	void ChargeFire(float _chargeTime)
	{
		hUDManager.EndCharge();
		if (_chargeTime >= gun.minChargeTime)
		{
			Debug.Log("Fired with " + _chargeTime + " seconds of charge");
			int multiShot = gun.multiShot;
			if (gun.multiShotCharge)
			{
				multiShot = Mathf.RoundToInt(_chargeTime / gun.maxChargeTime * gun.multiShot);
			}
			float damage = gun.damage;
			if (gun.damageCharge)
			{
				damage = _chargeTime / gun.maxChargeTime * gun.damage;
			}
			float splashRange = gun.splashRange;
			if (gun.splashRangeCharge)
			{
				splashRange = _chargeTime / gun.maxChargeTime * gun.splashRange;
			}
			float recoil = gun.recoil;
			if (gun.recoilCharge)
			{
				recoil = _chargeTime / gun.maxChargeTime * gun.recoil;
			}
			float inaccuracy = gun.inaccuracy;
			if (gun.inaccuracyCharge)
			{
				inaccuracy = _chargeTime / gun.maxChargeTime * gun.inaccuracy;
			}
			float fireDamage = gun.fireDamage;
			if (gun.fireDamageCharge)
			{
				fireDamage = _chargeTime / gun.maxChargeTime * gun.inaccuracy;
			}
			int penetration = gun.penetrateAmount;
			if (gun.penetrateCharge)
			{
				penetration = Mathf.FloorToInt(_chargeTime / gun.maxChargeTime * gun.penetrateAmount);
			}
			Fire(multiShot, damage, splashRange, recoil, inaccuracy, fireDamage, gun.range, penetration);
		}
		else
		{
			Debug.Log("Failed to fire");
		}
	}

	protected void Fire(int _multiShot, float _damage, float _splashRange, float _recoil, float _spread, float _fireDamage, float _range, int _penetration)
	{
		lastFire = Time.time;
		gun.curAmmo--;
		hUDManager.SetAmmoText(gun.curAmmo, gun.storedAmmo);
		gunObjects[gunIndex].GetComponent<Animation>().Play();
		if (!gun.projectile)
		{
			//raycast
			for (int i = 0; i < _multiShot; i++)
			{
				RaycastHit hit;
				if (gun.penetrative)
				{
					RaycastHit[] hits = Physics.RaycastAll(Camera.main.transform.position, RandomDirection(Camera.main.transform, (characterScript.aiming ? gun.aimedInaccuracy : gun.inaccuracy) + spread), _range, layermask);
					hits = hits.OrderBy(h => h.distance).ToArray();
					for(int j = 0; j <= _penetration; j++)
					{
						if (j >= hits.Length)
							break;
						if (hits[j].collider.gameObject.GetComponent<Health>())
						{
							Destroy(Instantiate(hitmarkerPrefab, hits[j].point, Quaternion.identity), 5f);
							hits[j].collider.gameObject.GetComponent<Health>().Damage(_damage);
							if (_fireDamage > 0f)
							{
								Debug.Log("Setting Fire");
								hits[j].collider.gameObject.GetComponent<Health>().SetFire(_fireDamage, gun.fireTime);
							}
						}
						else if (hits[j].collider.gameObject.GetComponent<Hitbox>())
						{
							Destroy(Instantiate(hitmarkerPrefab, hits[j].point, Quaternion.identity), 5f);
							HitInfo info = new HitInfo() { distance = hits[j].distance, source = gun.name, isExplosion = false };
							hits[j].collider.gameObject.GetComponent<Hitbox>().Damage(_damage, info);
							if (_fireDamage > 0f)
							{
								Debug.Log("Setting Fire");
								hits[j].collider.gameObject.GetComponent<Hitbox>().SetFire(_fireDamage, gun.fireTime);
							}
						}
						else
						{
							Destroy(Instantiate(missmarkerPrefab, hits[j].point, Quaternion.identity), 5f);
							break;
						}
					}
				} 
				else if (Physics.Raycast(Camera.main.transform.position, RandomDirection(Camera.main.transform, (characterScript.aiming ? gun.aimedInaccuracy : gun.inaccuracy) + spread), out hit, _range, layermask))
				{
					//Debug.Log(hit.collider.gameObject);
					if (hit.collider.gameObject.GetComponent<Health>())
					{
						Destroy(Instantiate(hitmarkerPrefab, hit.point, Quaternion.identity), 5f);
						hit.collider.gameObject.GetComponent<Health>().Damage(_damage);
						if (_fireDamage > 0f)
						{
							Debug.Log("Setting Fire");
							hit.collider.gameObject.GetComponent<Health>().SetFire(_fireDamage, gun.fireTime);
						}
					} 
					else if (hit.collider.gameObject.GetComponent<Hitbox>())
					{
						Destroy(Instantiate(hitmarkerPrefab, hit.point, Quaternion.identity), 5f);
						HitInfo info = new HitInfo() {  distance = hit.distance, isExplosion = false, source = gun.name };
						hit.collider.gameObject.GetComponent<Hitbox>().Damage(_damage, info);
						if (_fireDamage > 0f)
						{
							Debug.Log("Setting Fire");
							hit.collider.gameObject.GetComponent<Hitbox>().SetFire(_fireDamage, gun.fireTime);
						}
					}
					else
					{
						Destroy(Instantiate(missmarkerPrefab, hit.point, Quaternion.identity), 5f);

					}
				}
			}
		}
		else
		{
			for (int i = 0; i < _multiShot; i++)
			{
				GameObject newProj = Instantiate(gun.projectilePrefab, gunLoc.position, gunLoc.rotation);
				if (!gun.splash)
				{
					newProj.GetComponent<Projectile>().SetProperties(_damage, gun.name, transform);
				}
				else if (gun.explodeOnImpact)
				{
					newProj.GetComponent<Projectile>().SetProperties(_damage, _splashRange, gun.name, transform);
				}
				else
				{
					newProj.GetComponent<Projectile>().SetProperties(_damage, _splashRange, gun.explodeTimer, gun.name, transform);
				}
				newProj.GetComponent<Rigidbody>().AddForce(RandomDirection(gunLoc, 0) * gun.projectileSpeed);
			}
		}
	}
	void SwitchWeapons(bool up)
	{
		StopCoroutine("Reload");
		reloading = false;
		if (up)
		{
			if (gunIndex + 1 < guns.Count)
				gunIndex++;
			else
				gunIndex = 0;
		}
		else
		{
			if (gunIndex > 0)
				gunIndex--;
			else
				gunIndex = guns.Count - 1;
		}
		gun = guns[gunIndex];
		gunLoc = gunObjects[gunIndex].transform;
		//gun.curAmmo = gun.maxAmmo;
		isCharger = (gun.damageCharge || gun.fireDamageCharge || gun.inaccuracyCharge || gun.multiShotCharge || gun.recoilCharge || gun.splashRangeCharge);
		spread = 0f;
		projectilePrefab = gun.projectilePrefab;
		characterScript.SwitchWeapons(gunObjects[gunIndex], hiddenGunStockpile, gun.aimedFOV);
		hUDManager.SetGunText(gun.name);
		hUDManager.SetAmmoText(gun.curAmmo, gun.storedAmmo);
		hUDManager.EndReload();
		hUDManager.EndCharge();
		hUDManager.SetInaccuracy(gun.inaccuracy);
	}

	public void PickUpWeapon(Gun _gun)
	{
		Gun myGun = null;

		//find the gun in our inventory by name
		foreach (Gun g in guns)
		{
			if (g.name == _gun.name)
			{
				myGun = g;
				break;
			}
		}

		if (myGun != null)  // Pick up only ammo
		{
			myGun.storedAmmo += _gun.storedAmmo;
			hUDManager.SetAmmoText(gun.curAmmo, gun.storedAmmo);
			//Debug.Log("Picking up " + _gun.storedAmmo.ToString() + " ammo for " + myGun.name);
		}
		else    // Pick up new gun
		{
			_gun.curAmmo = _gun.maxAmmo;
			_gun.storedAmmo -= _gun.maxAmmo;

			guns.Add(_gun);
			gunObjects.Add(Instantiate(_gun.gunPrefab, hiddenGunStockpile, Quaternion.identity));
			gunIndex = guns.Count - 1;
			gun = guns[gunIndex];
			gunLoc = gunObjects[gunIndex].transform;

			isCharger = (gun.damageCharge || gun.fireDamageCharge || gun.inaccuracyCharge || gun.multiShotCharge || gun.recoilCharge || gun.splashRangeCharge);
			spread = 0f;

			characterScript.SwitchWeapons(gunObjects[gunIndex], hiddenGunStockpile, gun.aimedFOV);
			hUDManager.SetGunText(gun.name);
			hUDManager.SetAmmoText(gun.curAmmo, gun.storedAmmo);
		}

	}

	public void LoadWeapon(Gun _gun)
	{
		guns.Add(_gun);
		gunObjects.Add(Instantiate(_gun.gunPrefab, hiddenGunStockpile, Quaternion.identity));
	}

	public void SwitchWeapon(int index)
	{
		gunIndex = index;
		gun = guns[gunIndex];

		isCharger = (gun.damageCharge || gun.fireDamageCharge || gun.inaccuracyCharge || gun.multiShotCharge || gun.recoilCharge || gun.splashRangeCharge);
		spread = 0f;

		characterScript.SwitchWeapons(gunObjects[gunIndex], hiddenGunStockpile, gun.aimedFOV);
		hUDManager.SetGunText(gun.name);
		hUDManager.SetAmmoText(gun.curAmmo, gun.storedAmmo);
	}
	public void Clear()
	{
		guns.Clear();
		gunObjects.Clear();
	}
}
