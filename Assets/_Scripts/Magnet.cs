using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
	public bool magnet;
	public float time;

	public void StartMagnet(float time)
	{
		StartCoroutine(MagnetSlowly(time));
	}

	protected IEnumerator MagnetSlowly(float time)
	{
		magnet = true;
		yield return new WaitForSeconds(time);
		magnet = false;
	}
}
