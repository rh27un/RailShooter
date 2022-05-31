using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
	protected Health health;
	protected HUDManager manager;
	public void Awake()
	{
		manager = GameObject.FindGameObjectWithTag("HUDManager").GetComponent<HUDManager>();
		health = gameObject.GetComponent<Health>();
		manager.StartBossFight(gameObject.name);
	}

	private void Update()
	{
		manager.SetBossHealth(health.CurHealth / health.maxHealth);
	}
}
