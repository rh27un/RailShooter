using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : Health
{
	protected HUDManager hudManager;
	protected TrainController trainController;
	public void Awake()
	{
		trainController = GameObject.FindGameObjectWithTag("GameController").GetComponent<TrainController>();
		hudManager = GameObject.FindGameObjectWithTag("HUDManager").GetComponent<HUDManager>();
		hudManager.StartBossFight(gameObject.name);
		trainController.StartBossTimer(10000f);
	}

	public override void Damage(float _damage, HitInfo _info)
	{
		base.Damage(_damage, _info);
		hudManager.SetBossHealth(curHealth / maxHealth);
	}

	public override void Die(HitInfo _info)
	{
		base.Die(_info);
		hudManager.SetBossHealth(curHealth / maxHealth);
		hudManager.EndBossFight();
		trainController.EndBossTimer();
	}
}
