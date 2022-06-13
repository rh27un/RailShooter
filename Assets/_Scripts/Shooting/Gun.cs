using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "gunData", menuName = "Guns/Gun", order = 1)]
public class Gun : ScriptableObject
{
	[Header("Visuals")]
	public GameObject gunPrefab;
	public GameObject projectilePrefab;

	[Header("Ammo & Reload")]
	public int maxAmmo; //maximum ammunition
	public bool infiniteAmmo;
	[HideInInspector]
	public int curAmmo;
	[HideInInspector]
	public int storedAmmo;
	public int multiShot; //projectiles fired from one bullet
	public float fireRate; //time in seconds between each shot
	public float reloadTime; //time in seconds on each reload
	public float range; //range in metres;

	[Header("Damage")]
	public float damage; //damage per hit
	public bool splash; //if true, causes damage to all enemies in a range
	public float splashRange; //the range at which splash damage is applied in metres
	public float fireDamage; //damage per second to victim
	public float fireTime;
	public bool penetrative; //does raycast penetrate enemies
	public int penetrateAmount; //how many penetrations can the raycast handle

	[Header("Recoil & Inaccuracy")]
	public float recoil; //how much spread increases with fire
	public float aimedRecoil;
	public float inaccuracy;
	public float aimedFOV;
	[Header("Projectile")]
	public bool projectile; //fires a projectile if true, uses raycast if false
	public float projectileSpeed; //speed at which projectile is fired in m/s
	public bool explodeOnImpact; //projectiles explode on impact if true, wait for timer if false
	public float explodeTimer;  //time in seconds before projectiles explode

	[Header("Charge")] //charges increase gun attributes by holding fire
	public float minChargeTime; //time in seconds required to fire
	public float maxChargeTime; //time in seconds required for max charge
	public bool multiShotCharge; //charges multishot
	public bool damageCharge; //charges damage
	public bool splashRangeCharge; //charges splash range
	public bool recoilCharge; //charges recoil
	public bool inaccuracyCharge; //charges inaccuracy
	public bool fireDamageCharge; //charges fire damage   
	public bool penetrateCharge; //charges penetration
}