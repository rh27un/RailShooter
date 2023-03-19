using System.Collections;
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
	public SphereCollider collider;
	public TrainController trainController;
	public float speedToRadius;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
		train = GameObject.FindGameObjectsWithTag("TrainFlat");
		target = train[Random.Range(0, train.Length)];
		GetComponent<Health>().enemyType = enemyPrefab.name.Replace(" Variant", string.Empty);
		collider = GetComponent<SphereCollider>();
		trainController = GameObject.FindGameObjectWithTag("GameController").GetComponent<TrainController>();
		collider.radius = trainController.GetCurSpeed() * speedToRadius;
    }

	void Update()
	{
		if(transform.position.x > 200f)
		{
			GameObject.FindGameObjectWithTag("GameController").GetComponent<TrainController>().Die(GetComponent<Health>().enemyType);
			Destroy(gameObject);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject == target)
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
			if (collision.collider.tag == "TrainFlat")
			{
				jumping = false;
				var enemy = Instantiate(enemyPrefab, transform.position, transform.rotation);
				enemy.GetComponent<Enemy>().startHealth = GetComponent<Health>().CurHealth;
				Destroy(gameObject);
			}
			else
			{

				rigidbody.AddForce(-rigidbody.velocity / Time.deltaTime);
			}
		}
	}

	public void SetEnemyType(GameObject prefab, string type)
	{
		name = prefab.name.Replace(" Variant", string.Empty);
		enemyPrefab = prefab;
		GetComponent<Health>().enemyType = type;
		GetComponent<MeshFilter>().sharedMesh = prefab.GetComponent<MeshFilter>().sharedMesh;
		GetComponent<MeshRenderer>().sharedMaterial = prefab.GetComponent<MeshRenderer>().sharedMaterial;
		//transform.scal
	}
}
