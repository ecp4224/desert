using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour {

	public CrosshairController crosshair;
	GunController gunController;
	Camera viewCamera;

	// Use this for initialization
	void Start () {
		Cursor.visible = false;
		viewCamera = Camera.main;
		gunController = GetComponent<GunController> ();
	}
	
	// Update is called once per frame
	void Update () {
		
		//Rotation input
		Ray ray = viewCamera.ScreenPointToRay (Input.mousePosition);
		Plane groundPlane = new Plane (Vector3.up, Vector3.up * gunController.GunHeight);
		float rayDistance;

		if (groundPlane.Raycast (ray, out rayDistance)) {
			Vector3 point = ray.GetPoint (rayDistance);
			Debug.DrawLine (ray.origin, point, Color.red);
			crosshair.transform.position = point;
			crosshair.DetectTargets (ray);

		}
	}
}
