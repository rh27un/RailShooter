using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    protected Health health;
    [SerializeField]
    protected float damageMod;
    void Awake()
    {
        health = gameObject.GetComponentInParent<Health>();
    }

    public void Damage(float _damage)
	{
        //Debug.Log("Hit " + gameObject.name + " for " + _damage * damageMod + " damage");
        health.Damage(_damage * damageMod, gameObject.name);
	}
    public void SetFire(float _fireDamage, float _burnTime)
	{
        health.SetFire(_fireDamage, _burnTime);
	}
}
