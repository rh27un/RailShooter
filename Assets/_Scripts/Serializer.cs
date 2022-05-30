using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class Serializer : MonoBehaviour
{
	[SerializeField]
	protected string fileName = "checkpoint.dat";
	protected TrainController trainController;
	protected Score score;
	protected Shooting shooting;
	protected PlayerHealth playerHealth;
	protected Transform terrain;
	public string guns;

	public List<Gun> loadableGuns = new List<Gun>();

	protected Transform spawnpoint;
	protected Transform player;
	protected float terrainXPos = -15666;
	private void Awake()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
		trainController = GetComponent<TrainController>();
		score = GetComponent<Score>();
		shooting = GameObject.FindGameObjectWithTag("Player").GetComponent<Shooting>();
		player = GameObject.FindGameObjectWithTag("Player").transform;
		spawnpoint = GameObject.FindGameObjectWithTag("Respawn").transform;
		playerHealth = player.gameObject.GetComponent<PlayerHealth>();
		if (File.Exists(fileName))
		{
			LoadCheckpoint();
		}
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.name == "Init" || scene.name == "Leaderboard" /*|| mode == LoadSceneMode.Additive*/)
			return;
		terrain = GameObject.FindGameObjectWithTag("Ground").transform;
		SaveCheckpoint();
		terrain.position = new Vector3(terrainXPos, terrain.position.y, terrain.position.z);

	}
	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F5))
		{
			SaveCheckpoint();
		}
		if (Input.GetKeyDown(KeyCode.F6))
		{
			LoadCheckpoint();
		}
	}

	public void SaveCheckpoint()
	{
		float trainDistance = trainController.GetDistance();
		float trainSpeed = trainController.GetCurSpeed();
		int zone = trainController.GetZone();
		terrainXPos = terrain.position.x;
		float curScore = score.GetScore();
		int maxLives = playerHealth.maxLives;
		int continues = playerHealth.continues;
		if (continues == 0)
		{
			return;
		} else if (continues != -1)
		{
			continues--;
		}
		guns = string.Empty;

		for(int i = 0; i < shooting.guns.Count; i++)
		{
			Gun gun = shooting.guns[i];
			guns += $"{gun.name}:{gun.curAmmo}/{gun.storedAmmo}";
			if(i < shooting.guns.Count - 1)
			{
				guns += ",";
			}

		}

		using (var stream = File.Open(fileName, FileMode.Create))
		{
			using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
			{
				writer.Write(trainDistance);
				writer.Write(trainSpeed);
				writer.Write(zone);
				writer.Write(terrainXPos);
				writer.Write(curScore);
				writer.Write(maxLives);
				writer.Write(continues);
				writer.Write(guns);
			}
		}
	}

	public void LoadCheckpoint()
	{
		float trainDistance;
		float trainSpeed;
		int zone;
		float curScore;
		int maxLives;
		int continues;
		if (File.Exists(fileName))
		{
			using (var stream = File.Open(fileName, FileMode.Open))
			{
				using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
				{
					trainDistance = reader.ReadSingle();
					trainSpeed = reader.ReadSingle();
					zone = reader.ReadInt32();
					terrainXPos = reader.ReadSingle();
					curScore = reader.ReadSingle();
					maxLives = reader.ReadInt32();
					continues = reader.ReadInt32();
					guns = reader.ReadString();
				}
			}

			trainController.SetDistance(trainDistance);
			trainController.SetCurSpeed(trainSpeed);
			trainController.SetZone(zone);
			score.Checkpoint(curScore);
			player.position = spawnpoint.position;
			player.rotation = spawnpoint.rotation;
			player.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
			playerHealth.Respawn(maxLives, continues);
			Debug.Log(continues);
			if (continues == 0)
			{
				File.Delete(fileName);
				return;
			}
		} 
		else
		{
			Debug.LogError("File " + fileName + " does not exist");
		}

		

	}
	public void DeleteCheckpoint()
	{
		File.Delete(fileName);
	}
}

[Serializable]
class Checkpoint
{

}