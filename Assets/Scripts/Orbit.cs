using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour {

	public float extraTileRotateSpeed;

	void Update(){
		transform.Rotate (0f, extraTileRotateSpeed, 0f);
	}
}
