using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraTileRemoval : MonoBehaviour {

	public Transform ExtraTile;
	public float RemovalTime = 2f;

	void OnTriggerEnter(Collider other){
		if(other.tag!=null){
			Instantiate(ExtraTile,this.transform);
			Destroy(this.gameObject,RemovalTime);
		}
	}
}
