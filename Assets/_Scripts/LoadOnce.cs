using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LoadOnce : MonoBehaviour
{
	private void Awake()
	{
		DontDestroyOnLoad(gameObject);
		if (GameObject.FindGameObjectsWithTag("LoadOnce").Length > 1)
		{
			Destroy(gameObject);
		} else
		{
			foreach(var loadOnce in gameObject.GetComponentsInChildren<ILoadOnce>())
			{
				loadOnce.IsFirstToLoad = true;
			}
		}
	}
}

public interface ILoadOnce
{
	bool IsFirstToLoad { get; set; }
}