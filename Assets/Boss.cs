using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
	public Animator animator;

	private void Update()
	{
		Debug.Log(animator.GetCurrentAnimatorStateInfo(0));
	}
}
