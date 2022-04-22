using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	[SerializeField]
	protected List<GameObject> prefabs;
	protected Dictionary<string, GameObject> prefabMapper;
	private void Awake()
	{
		prefabMapper = prefabs.ToDictionary(p => p.GetComponent<Enemy>().stats.name, p => p);
	}
	public void Spawn(List<string> enemiesToSpawn)
	{
		var points = transform.GetComponentsInChildren<EnemyStarter>();
		for(int i = 0; i < 3; i++)
		{
			points[i].SetEnemyType(prefabMapper[enemiesToSpawn[i]], enemiesToSpawn[i]);
		}
	}
}
