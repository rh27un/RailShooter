using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Difficulty : MonoBehaviour
{
	public int lives;
	public int continues;
	public bool infiniteContinues;

	public string LeaderBoard => $"{lives}l{continues}c.dat";
	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}
}
