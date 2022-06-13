using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct HitInfo
{
	public float distance;
	public string source;
	public string target;
	public string targetHitbox;
	public bool isExplosion;
	//public bool isFire;
}

public class Hitbox : MonoBehaviour
{
	protected Health health;
	[SerializeField]
	protected float damageMod;
	void Awake()
	{
		health = gameObject.GetComponentInParent<Health>();
	}

	public void Damage(float _damage, HitInfo _info)
	{
		//Debug.Log("Hit " + gameObject.name + " for " + _damage * damageMod + " damage");
		_info.targetHitbox = gameObject.name;
		health.Damage(_damage * damageMod, _info);
	}
	public void SetFire(float _fireDamage, float _burnTime)
	{
		health.SetFire(_fireDamage, _burnTime);
	}
}
