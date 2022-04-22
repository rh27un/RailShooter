using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Init : MonoBehaviour
{
	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
		SceneManager.LoadScene("Town");
	}
}
