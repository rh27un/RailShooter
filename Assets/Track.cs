using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Track : MonoBehaviour
{
	protected TrainController controller;
	[SerializeField]
	protected float frontPoint;
	[SerializeField]
	protected float backPoint;

	private void Start()
	{
		controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<TrainController>();
	}
	private void Update()
	{
		transform.position += Vector3.right * controller.GetCurSpeed() * Time.deltaTime;
		if(transform.position.x >= backPoint)
		{
			transform.position = Vector3.up * 0.1f + Vector3.right * frontPoint;
		}
	}
}
