using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class Leaderboard : MonoBehaviour
{
	public string fileName = "leaderboard.dat";

	List<KeyValuePair<string, int>> defaultLeaderboard = new List<KeyValuePair<string, int>>
	{
		new KeyValuePair<string, int>("TRJ", 160000),
		new KeyValuePair<string, int>("MKA", 80000),
		new KeyValuePair<string, int>("BEA", 40000),
		new KeyValuePair<string, int>("ARL", 20000),
		new KeyValuePair<string, int>("JAM", 10000),
		new KeyValuePair<string, int>("MCR", 5000),
		new KeyValuePair<string, int>("PAG", 4000),
		new KeyValuePair<string, int>("RDZ", 3000),
		new KeyValuePair<string, int>("BEN", 2000),
		new KeyValuePair<string, int>("TAI", 1000)
	};
	List<KeyValuePair<string, int>> leaderboard = new List<KeyValuePair<string, int>>();
	protected Difficulty difficulty;

	public void Awake()
	{
		difficulty = GameObject.Find("Difficulty").GetComponent<Difficulty>();
	}
	private void OnDestroy()
	{
		if(difficulty)
			Destroy(difficulty.gameObject);
	}
	protected void SaveLeaderboard()
	{
		leaderboard = leaderboard.OrderByDescending(kv => kv.Value).ToList();
		if(leaderboard.Count != 10)
		{
			Debug.LogError("Leaderboard count is not equal to ten!");
			return;
		}
		using (var stream = File.Open(difficulty.LeaderBoard, FileMode.Create))
		{
			using (var writer = new BinaryWriter(stream, Encoding.UTF8, false))
			{
				foreach(var pair in leaderboard)
				{
					writer.Write(pair.Key);
					writer.Write(pair.Value);
				}
			}
		}
	}

	protected void LoadLeaderboard()
	{
		leaderboard.Clear();
		
		if (File.Exists(difficulty.LeaderBoard))
		{
			using (var stream = File.Open(difficulty.LeaderBoard, FileMode.Open))
			{
				using (var reader = new BinaryReader(stream, Encoding.UTF8, false))
				{
					for (int i = 0; i < 10; i++)
					{
						var name = reader.ReadString();
						var score = reader.ReadInt32();
						if (string.IsNullOrWhiteSpace(name))
						{
							Debug.LogError("No name");
						}
						leaderboard.Add(new KeyValuePair<string, int>(name, score));
					}
				}
			}
		} else
		{
			leaderboard.AddRange(defaultLeaderboard);
			SaveLeaderboard();
		}
	}

	public List<KeyValuePair<string, int>> GetLeaderBoard()
	{
		LoadLeaderboard();
		return leaderboard;
	}

	public void UpdateLeaderboard(List<KeyValuePair<string, int>> newLeaderboard)
	{
		if(newLeaderboard.Count != 10)
		{
			Debug.LogError("Leaderboard count is not 10");
			return;
		}
		leaderboard = newLeaderboard;
		SaveLeaderboard();
	}
}
