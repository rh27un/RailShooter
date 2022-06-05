using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class TrainController : MonoBehaviour {

	public float speed;
	public float acceleration;
	[SerializeField] protected float distance;
	[SerializeField] protected float curSpeed;
	[SerializeField] protected float zoneLength;
	protected float nextZone;
	[SerializeField] protected int zone = 2;
	[Header("Enemy Spawn")]
	[SerializeField] protected float enemySpawnDistance;	// Min distance between enemy spawns
	//[SerializeField] protected int enemySpawnThreshold;     // Max living enemies before next spawn
	[SerializeField] protected GameObject leftEnemySpawn;
	[SerializeField] protected GameObject rightEnemySpawn;
	[SerializeField] protected Transform leftSpawnPoint;
	[SerializeField] protected Transform rightSpawnPoint;
	[SerializeField] protected List<Stage> stageData = new List<Stage>();
	[SerializeField] protected List<string> enemiesToSpawn = new List<string>();
	public TMP_Text textText;

	protected Dictionary<string, int> enemyCount = new Dictionary<string, int>();

	protected float lastSpawn;

	protected GameObject terrain;

	protected float bossTimeStart;
	protected float bossTime;
	protected bool isBoss;
	public void StartBossTimer(float time)
	{
		bossTimeStart = distance;
		bossTime = time;
		isBoss = true;
	}
	public float BossTimer()
	{
		return (Time.time - bossTimeStart) / bossTime;
	}
	public void EndBossTimer()
	{
		isBoss = false;
	}

	void FailBoss()
	{
		GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().LoseBoss();
		isBoss = false;
	}
	void updateTestText()
	{
		string text = string.Empty;

		foreach(var e in enemyCount)
		{
			text += $"{e.Key}: {e.Value}\n";
		}

		textText.text = text;
	}
	void Awake(){
		SceneManager.sceneLoaded += OnSceneLoaded;
		nextZone = zoneLength;
		enemyCount = enemiesToSpawn.ToDictionary(e => e, e => 0);
		textText = GameObject.Find("ENEMYTEST").GetComponent<TMP_Text>();
		updateTestText();
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.name == "Init" || scene.name == "Leaderboard")
			return;
		terrain = GameObject.FindGameObjectWithTag("Ground");
		//nextZone = zoneLength;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}
	void LateUpdate()
	{
		if (!terrain)
			return;
		if(curSpeed < speed){
			curSpeed = Mathf.Clamp(curSpeed + acceleration * Time.deltaTime, curSpeed, speed);
		} else if (curSpeed > speed){
			curSpeed = Mathf.Clamp(curSpeed - acceleration * Time.deltaTime, speed, curSpeed);
		}
		distance += curSpeed * Time.deltaTime;
		if(distance > bossTimeStart + bossTime && isBoss)
		{
			FailBoss();
		}

		if(distance > lastSpawn + enemySpawnDistance)
		{
			SpawnEnemies();
			lastSpawn = distance;
		}
		
		if(terrain)
			terrain.transform.position += new Vector3(curSpeed * Time.deltaTime, 0f , 0f);
		
		if(distance > nextZone){
			nextZone += zoneLength;
			zone++;
			Destroy(terrain);
			SceneManager.LoadSceneAsync(zone, LoadSceneMode.Additive);
		}
	}

	List<string> GetEnemiesToSpawn(List<string> toSpawn)
	{
		
		int added = 0;
		enemyCount = enemyCount.OrderBy(c => c.Value).ToDictionary(c => c.Key, c => c.Value);
		var stage = stageData[zone - 3];
		var stageDistance = (distance - (nextZone - zoneLength)) / zoneLength;

		//for every type of enemy, in ascending order of count, check if there are fewer than the threshold to spawn more
		for (int i = 0; i < enemyCount.Count; i++)
		{
			var enemyType = enemyCount.ElementAt(i).Key;
			var threshold = Mathf.FloorToInt(stage.spawnCurves[enemiesToSpawn.IndexOf(enemyType)].Evaluate(stageDistance) * stage.spawnScale);
			//if so, queue up one to spawn
			if (enemyCount[enemyType] < threshold)
			{
				toSpawn.Add(enemyType);
				enemyCount[enemyType]++;
				added++;
				// if we have 3 enemies queued up to spawn, we can spawn them
				if (toSpawn.Count >= 3)
					break;
			}
		}
		// if we have 3 enemies queued up to spawn, we can spawn them
		if (toSpawn.Count == 3)
		{
			return toSpawn;
		}
		// if we have less then 3, then we can try add more
		else if (added > 0)
		{
			return GetEnemiesToSpawn(toSpawn);
		}
		else // if there are no more to add, don't spawn anything
		{
			foreach(string type in toSpawn)
			{
				enemyCount[type]--;
			}
			return null;
		}
	}
	private void SpawnEnemies()
	{
		List<string> toSpawn = GetEnemiesToSpawn(new List<string>());

		if(toSpawn != null && toSpawn.Count == 3)
		{
			updateTestText();
			if (Random.Range(0, 2) == 0)
			{
				Instantiate(leftEnemySpawn, leftSpawnPoint.position, leftSpawnPoint.rotation, terrain.transform).GetComponent<Spawner>().Spawn(toSpawn);
			}
			else
			{
				Instantiate(rightEnemySpawn, rightSpawnPoint.position, rightSpawnPoint.rotation, terrain.transform).GetComponent<Spawner>().Spawn(toSpawn);
			}
		}
	}

	public void Die(string enemyType)
	{
		if (enemyCount.ContainsKey(enemyType))
		{
			enemyCount[enemyType]--;
			updateTestText();
		}
	}
	public void ChangeSpeed(float _speed, float _accel){
		acceleration = _accel;
		speed = _speed;
	}

	public int GetZone()
	{
		return zone;
	}
	public void SetZone(int _zone)
	{
		zone = _zone;
		nextZone = zoneLength * (zone - 2);
		SceneManager.LoadScene(zone, LoadSceneMode.Single);
	}
	public float GetDistance()
	{
		return distance;
	}
	public void SetDistance(float _distance)
	{
		distance = _distance;
	}
	public float GetCurSpeed()
	{
		return curSpeed;
	}
	public void SetCurSpeed(float _speed)
	{
		curSpeed = _speed;
	}
}