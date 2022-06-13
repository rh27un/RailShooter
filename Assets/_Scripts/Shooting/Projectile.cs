using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	[SerializeField] protected string source;
	[SerializeField] protected float damage;
    [SerializeField] protected bool splash;
    [SerializeField] protected float splashRange;
    [SerializeField] protected bool impact;
    [SerializeField] protected float timer;
	[SerializeField] protected GameObject explosion;
	[SerializeField] protected GameObject sphere;
	[SerializeField] protected Transform sourceTransform;
	public void SetProperties(float _dmg, string _src, Transform _trn = null){
        damage = _dmg;
		splash = false;
        impact = true;
		source = _src;
		sourceTransform = _trn;
	}
	public void SetProperties(float _dmg, float _splash, string _src, Transform _trn = null)
	{
		damage = _dmg;
		splash = true;
		splashRange = _splash;
        impact = true;
		source= _src;
		sourceTransform = _trn;
	}
	public void SetProperties(float _dmg, float _splash, float _timer, string _src, Transform _trn = null)
	{
		damage = _dmg;
		splash = true;
		splashRange = _splash;
        impact = false;
		timer = _timer;
		StartCoroutine("Timer");
		source = _src;
		sourceTransform = _trn;
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
					float distance = 0f;
					if (sourceTransform != null) {
						distance = (col.collider.transform.position - sourceTransform.position).magnitude;
					}
					HitInfo info = new HitInfo() { source = source, distance = distance, isExplosion = true };
					col.collider.gameObject.GetComponent<Hitbox>().Damage(damage, info);

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
				var damageDealt = (-distance.magnitude + splashRange) / splashRange * damage;
				Debug.Log("Dealt " + damageDealt + " damage");
				c.GetComponent<Health>().Damage(damageDealt);
			}
			else if (c.GetComponent<Hitbox>())
			{
				Vector3 distance = transform.position - c.transform.position;
				var damageDealt = (-distance.magnitude + splashRange) / splashRange * damage;
				Debug.Log("Dealt " + damageDealt + " damage");

				float sourceDistance = 0f;
				if (sourceTransform != null)
				{
					sourceDistance = (c.transform.position - sourceTransform.position).magnitude;
				}
				HitInfo info = new HitInfo() { source = source, distance = sourceDistance, isExplosion = true };
				c.GetComponent<Hitbox>().Damage(damageDealt, info);
			}
			if(c.GetComponent<Rigidbody>()){
				c.GetComponent<Rigidbody>().AddExplosionForce(damage, point, splashRange);
			}
		}
		Destroy(gameObject);
		Instantiate(explosion, point, Quaternion.identity);
		var newSphere = Instantiate(sphere, point, Quaternion.identity);
		newSphere.transform.localScale = Vector3.one * splashRange * 2;
		Destroy(newSphere, 1f);
	}
}
