using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainStarter : MonoBehaviour {

	protected TrainController train;
	// Use this for initialization
	void Start () {
		train = GameObject.FindGameObjectWithTag("GameController").GetComponent<TrainController>();
	}
	
	void OnTriggerEnter(Collider other){
		if(other.tag == "Player"){
			train.ChangeSpeed(50f, 1f);
			train.gameObject.GetComponent<Serializer>().SaveCheckpoint();
			Destroy(gameObject);
		}
	}
}
