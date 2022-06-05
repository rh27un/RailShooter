using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
	protected Health health;
	protected HUDManager hudManager;
	protected TrainController trainController;
	public void Awake()
	{
		trainController = GameObject.FindGameObjectWithTag("GameController").GetComponent<TrainController>();
		hudManager = GameObject.FindGameObjectWithTag("HUDManager").GetComponent<HUDManager>();
		health = gameObject.GetComponent<Health>();
		hudManager.StartBossFight(gameObject.name);
		trainController.StartBossTimer(10000f);
	}

	private void Update()
	{
		hudManager.SetBossHealth(health.CurHealth / health.maxHealth);
	}
}
