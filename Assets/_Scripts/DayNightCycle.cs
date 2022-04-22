using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour {

	public float daySpeed = 1f;
	
	// Update is called once per frame
	void Update () {
		transform.Rotate(- daySpeed * Time.deltaTime, 0f, 0f);
	}
}
