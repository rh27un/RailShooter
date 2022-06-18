using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "enemyData", menuName = "Enemies/Enemy", order = 1)]
public class EnemyStats : ScriptableObject
{
	public int maxHealth;
	public Gun gun;
	public bool firePredictiveProjectiles;
	public float projectileSpeed;
	public float dontFireTime;
	public float fireTime;
	public float waitBeforeAttack;
	public bool isCommander;
	public float idealPlayerDistance;
	public float fireRate;
	public float damage;
	public GameObject projectilePrefab;
	[Range(0f, 1f)]
	public float baseDropRate; //Chance out of 1 enemy will drop any powerup
	//Drop weights for each powerup
	public float baseHealthDropRate;
	public float baseOverhealDropRate;
	public float baseExtraDamageDropRate;
	public float baseBonusPointsDropRate;
	public float baseMagnetDropRate;
	public float baseBottomlessClipDropRate;
}
