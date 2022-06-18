using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTrain : MonoBehaviour
{
	[SerializeField]
	protected float speed;

	private void Update()
	{
		transform.position += new Vector3(speed * Time.deltaTime, 0f, 0f);
	}
}
