using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairController : MonoBehaviour {

	public LayerMask targetMask;
	public float rotateSpeed = 40f;
	public SpriteRenderer dot;
	public Color dotHighlightColor;
	Color originalDotColor;
	public MapGenerator Map;
	float mapSize;

	void Start(){
		originalDotColor = dot.color;
	}
	void Update () {
		mapSize = (Map.currentMap.mapSize.x * Map.currentMap.mapSize.y);
		transform.Rotate (Vector3.forward * rotateSpeed * Time.deltaTime);
	}

	public void DetectTargets(Ray ray){
		if (Physics.Raycast (ray, mapSize, targetMask)) {
			dot.color = dotHighlightColor;
		} else {
			dot.color = originalDotColor;
		}
	}
}
