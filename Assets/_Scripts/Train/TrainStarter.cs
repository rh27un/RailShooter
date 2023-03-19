using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainStarter : MonoBehaviour {

	protected TrainController train;
	public float speed;
	public float acceleration;
	// Use this for initialization
	void Start () {
		train = GameObject.FindGameObjectWithTag("GameController").GetComponent<TrainController>();
	}
	
	void OnTriggerEnter(Collider other){
		if(other.tag == "Player"){
			train.ChangeSpeed(speed, acceleration);
			train.gameObject.GetComponent<Serializer>().SaveCheckpoint();
			Destroy(gameObject);
		}
	}
}
