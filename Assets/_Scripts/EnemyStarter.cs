﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStarter : MonoBehaviour
{
	protected bool jumping;
	public Vector3 jumpForce;
    protected Rigidbody rigidbody;
	public GameObject enemyPrefab;
	public GameObject[] train;
	public GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
		train = GameObject.FindGameObjectsWithTag("Train");
		target = train[Random.Range(0, train.Length)];
    }

	private void OnTriggerEnter(Collider other)
	{
		if(other.name == target.name)
		{
			//Debug.Log("Jumping");
			rigidbody.AddForce(jumpForce + new Vector3(Random.Range(-50f, 50f), 0f, Random.Range(-50f, 50f)));
			jumping = true;
		}
	}
	private void OnCollisionEnter(Collision collision)
	{
		if (jumping)
		{
			jumping = false;
			var enemy = Instantiate(enemyPrefab, transform.position, transform.rotation);
			enemy.GetComponent<Enemy>().startHealth = GetComponent<Health>().CurHealth;
			Destroy(gameObject);
		}
	}

	public void SetEnemyType(GameObject prefab, string type)
	{
		enemyPrefab = prefab;
		GetComponent<Health>().enemyType = type;
		GetComponent<MeshFilter>().sharedMesh = prefab.GetComponent<MeshFilter>().sharedMesh;
		GetComponent<MeshRenderer>().sharedMaterial = prefab.GetComponent<MeshRenderer>().sharedMaterial;
		//transform.scal
	}
}