using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	[SerializeField] protected float damage;
    [SerializeField] protected bool splash;
    [SerializeField] protected float splashRange;
    [SerializeField] protected bool impact;
    [SerializeField] protected float timer;
	[SerializeField] protected GameObject explosion;
	[SerializeField] protected GameObject sphere;
	public void SetProperties(float _dmg){
        damage = _dmg;
		splash = false;
        impact = true;
    }
	public void SetProperties(float _dmg, float _splash){
		damage = _dmg;
		splash = true;
		splashRange = _splash;
        impact = true;
	}
	public void SetProperties(float _dmg, float _splash, float _timer){
		damage = _dmg;
		splash = true;
		splashRange = _splash;
        impact = false;
		timer = _timer;
		StartCoroutine("Timer");
	}
    void OnCollisionEnter(Collision col){
        if(impact){
			if(splash){
				Explode(col.contacts[0].point);
			} else {
				if(col.collider.gameObject.GetComponent<Health>()){
            		col.collider.gameObject.GetComponent<Health>().Damage(damage);
       			} else if (col.collider.gameObject.GetComponent<Hitbox>())
				{
					col.collider.gameObject.GetComponent<Hitbox>().Damage(damage);

				}
				Destroy(gameObject);
			}
		}
    }
	IEnumerator Timer(){
		yield return new WaitForSeconds(timer);
		Explode(transform.position);
	}
	void Explode(Vector3 point){
		Debug.Log("Kaboom!");
		Collider[] hitColliders = Physics.OverlapSphere(point, splashRange);
		foreach(Collider c in hitColliders){
			if (c.GetComponent<Health>())
			{
				Vector3 distance = transform.position - c.transform.position;
				var damageDealt = damage * (splashRange / distance.magnitude);
				Debug.Log("Dealt " + damageDealt + " damage");
				c.GetComponent<Health>().Damage(damageDealt);
			}
			else if (c.GetComponent<Hitbox>())
			{
				Vector3 distance = transform.position - c.transform.position;
				var damageDealt = damage * (splashRange / distance.magnitude);
				Debug.Log("Dealt " + damageDealt + " damage");
				c.GetComponent<Hitbox>().Damage(damageDealt);
			}
			if(c.GetComponent<Rigidbody>()){
				c.GetComponent<Rigidbody>().AddExplosionForce(damage, point, splashRange);
			}
		}
		Destroy(gameObject);
		Instantiate(explosion, point, Quaternion.identity);
		var newSphere = Instantiate(sphere, point, Quaternion.identity);
		newSphere.transform.localScale = Vector3.one * splashRange;
		Destroy(newSphere, 1f);
	}
}
