using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTrainSpawner : MonoBehaviour
{
	[SerializeField]
	protected List<GameObject> prefabs = new List<GameObject>();
	[SerializeField]
	protected float spawnRate;
	protected float lastSpawn = 0f;

	private void Update()
	{
		if(Time.time > lastSpawn + spawnRate)
		{
			Destroy(Instantiate(prefabs[Random.Range(0, prefabs.Count)], transform.position, Quaternion.identity), 5f);
			lastSpawn = Time.time;
		}
	}
}
