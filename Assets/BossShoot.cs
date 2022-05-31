using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossShoot : StateMachineBehaviour
{
	protected Transform player;
	protected bool canSee;
	[SerializeField]
	protected List<EnemyStats> stats = new List<EnemyStats>();
	protected CharacterController playerController;
	[SerializeField]
	protected List<Vector3> guns = new List<Vector3>();
	protected float[] lastFires;
	[SerializeField]
	protected float predictMod;
	//protected float lastFire = 0f;
	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
		playerController = player.GetComponent<CharacterController>();
		lastFires = new float[guns.Count];
		for(int i = 0; i < lastFires.Length; i++)
		{
			lastFires[i] = 0f;
		}
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		for(int i = 0; i < guns.Count; i++)
		{
			var pos = animator.transform.TransformPoint(guns[i]);
			var toPlayerVector = pos - player.position; 
			float distanceToPlayer = toPlayerVector.magnitude;
			RaycastHit hit;
			if (Physics.Raycast(pos, player.position - pos, out hit, 100f))
			{
				canSee = hit.collider.tag == "Player";
			}
			var timeToPlayer = distanceToPlayer / (stats[i].projectileSpeed * predictMod);
			var predictedPosition = player.position + (playerController.velocity * timeToPlayer);
			//if (predictedPosition.y < t.position.y)
			//	predictedPosition.y = t.position.y;
			if (canSee)
			{
				if (Time.time > lastFires[i] + (stats[i].fireRate))
				{
					if (!stats[i].firePredictiveProjectiles)
					{
						var newObject = Instantiate(stats[i].projectilePrefab, pos, Quaternion.identity);
						newObject.GetComponent<Rigidbody>().AddForce(toPlayerVector.normalized * -stats[i].projectileSpeed);
						//newObject.GetComponent<EnemyProjectile>().damage = stats.damage;
						lastFires[i] = Time.time;
						Destroy(newObject, 10f);
					}
					else
					{

						var toPredictedVector = pos - predictedPosition;
						var newObject = Instantiate(stats[i].projectilePrefab, pos, Quaternion.identity);
						newObject.GetComponent<Rigidbody>().AddForce(toPredictedVector.normalized * -stats[i].projectileSpeed);
						//newObject.GetComponent<EnemyProjectile>().damage = stats.damage;
						lastFires[i] = Time.time;
						Destroy(newObject, 10f);
					}
				}
			}
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	//{
	//    
	//}

	// OnStateMove is called right after Animator.OnAnimatorMove()
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	//{
	//    // Implement code that processes and affects root motion
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK()
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	//{
	//    // Implement code that sets up animation IK (inverse kinematics)
	//}
}
