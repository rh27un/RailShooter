using UnityEngine;

public class HealthPickup : MonoBehaviour
{
	[SerializeField]
	protected float maxHealth;
	[SerializeField]
	protected float minHealth;
    protected float health;
	[SerializeField]
    public bool overheal;
	public void Start()
	{
		health = Random.Range(minHealth, maxHealth);
	}

	public void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			other.GetComponent<PlayerHealth>().Heal(health, overheal);
			Destroy(gameObject);
		}
	}
}
