using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public LayerMask collisionMask;
	float speed = 10;
	float damage = 1;

	float lifetime = 3;
	float skinWidth = .1f;

	public Color trailColor;

	void Start(){
		Destroy (gameObject, lifetime);

		Collider[] initialCollsions = Physics.OverlapSphere (transform.position, .1f, collisionMask);
		if (initialCollsions.Length > 0) {
			OnHitObject (initialCollsions [0], transform.position);
		}

		GetComponent<TrailRenderer> ().material.SetColor ("_TintColor", trailColor);
	}

	public void SetSpeed(float newSpeed){
		speed = newSpeed;
	}

	void Update () {
		float moveDistance = speed * Time.deltaTime;
		CheckCollisions (moveDistance);
		transform.Translate (Vector3.forward * Time.deltaTime * speed);
	}

	void CheckCollisions(float moveDistance){
		Ray ray = new Ray (transform.position, transform.forward);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide)) {
			OnHitObject (hit.collider, hit.point);
		}
	}

	void OnHitObject(Collider other, Vector3 hitPoint){
		IDamageable damageableObject = other.GetComponent<IDamageable> ();
		if (damageableObject != null) {
			damageableObject.TakeHit(damage, hitPoint, transform.forward);
		}
		GameObject.Destroy (gameObject);
	}
}
