using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
	NotShooting = 0,
	Shooting = 1
}
public class Enemy : MonoBehaviour
{
	public EnemyStats stats;
	protected Health health;
	public Vector3 target;
	public Transform player;
	public CharacterController playerController;
	protected NavMeshAgent agent;
	public float startHealth = float.PositiveInfinity;

	//
	protected float lastFire;
	protected EnemyState curState;
	protected bool canSee;
	protected float dontFireTime;
	public GameObject aimMarkerPrefab;
	//protected Transform aimMarker;
	[SerializeField]
	protected float predictMod;

	void Awake()
	{
		health = gameObject.AddComponent<Health>();
		health.maxHealth = Mathf.Min(startHealth, stats.maxHealth);
		health.enemyType = stats.name;
		player = GameObject.FindGameObjectWithTag("Player").transform;
		playerController = player.GetComponent<CharacterController>();
		agent = GetComponent<NavMeshAgent>();
		dontFireTime = stats.dontFireTime + Random.Range(-1f, 1f);
		//aimMarker = Instantiate(aimMarkerPrefab).transform;
		//curState = EnemyState.Shooting;
		StartCoroutine("SwitchStates");
	}

	private void Update()
	{
		var toPlayerVector = transform.position - player.position;
		float distanceToPlayer = toPlayerVector.magnitude;
		if (distanceToPlayer > stats.idealPlayerDistance)
		{
			agent.SetDestination(player.position);
		}
		else
		{
			agent.SetDestination(transform.position + toPlayerVector);
		}
		var timeToPlayer = distanceToPlayer / (stats.projectileSpeed * predictMod);
		var predictedPosition = player.position + (playerController.velocity * timeToPlayer);
		if (predictedPosition.y < transform.position.y)
			predictedPosition.y = transform.position.y;
		//aimMarker.position = predictedPosition;
		switch (curState)
		{
			case EnemyState.Shooting:
				if (canSee)
				{
					if(Time.time > lastFire + (stats.fireRate))
					{
						if (!stats.firePredictiveProjectiles)
						{
							var newObject = Instantiate(stats.projectilePrefab, transform.position, transform.rotation);
							newObject.GetComponent<Rigidbody>().AddForce(toPlayerVector.normalized * -stats.projectileSpeed);
							//newObject.GetComponent<EnemyProjectile>().damage = stats.damage;
							lastFire = Time.time;
							Destroy(newObject, 10f);
						}
						else
						{
							
							var toPredictedVector = transform.position - predictedPosition;
							var newObject = Instantiate(stats.projectilePrefab, transform.position, transform.rotation);
							newObject.GetComponent<Rigidbody>().AddForce(toPredictedVector.normalized * -stats.projectileSpeed);
							//newObject.GetComponent<EnemyProjectile>().damage = stats.damage;
							lastFire = Time.time;
							Destroy(newObject, 10f);
						}
					}
				}
				break;
		}
	}

	IEnumerator SwitchStates()
	{
		if (curState == EnemyState.NotShooting)
			yield return new WaitForSeconds(dontFireTime);
		else
			yield return new WaitForSeconds(stats.fireTime);
		RaycastHit hit;
		if (Physics.Raycast(transform.position, player.position - transform.position, out hit, 100f))
			{
			canSee = hit.collider.tag == "Player";
		}
		curState = curState == EnemyState.NotShooting ? EnemyState.Shooting : EnemyState.NotShooting;
		StartCoroutine("SwitchStates");
	}

	public void DropWeapon()
	{
		if (stats.gun.infiniteAmmo)
			return;
		Gun gun = stats.gun;
		GameObject newObject = Instantiate(gun.gunPrefab, transform.position, Quaternion.identity);
		SphereCollider pickupCollider = newObject.AddComponent<SphereCollider>();
		pickupCollider.isTrigger = true;
		GunPickup pickupScript = newObject.AddComponent<GunPickup>();
		pickupScript.gun = gun;
		pickupScript.ammo = Random.Range(gun.maxAmmo, gun.maxAmmo * 2);
		//Debug.Log("Dropping " + gun.name + " with " + pickupScript.ammo.ToString() + " ammo");
	}
}
