using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSystem : MonoBehaviour {

	public enum WeaponType{
		ohSword,
		thSword,
		Spear
	}

	public float dashDistance = 20f;
	public float attackDistance = 2f;
	public float stepDistance = .01f;
	public float dashSpeed = 1.5f;
	public Transform target;

	//time taken to move from start to finish
	public float timeTakenDuringLerp = 1f;

	public float distanceBetweenUnits;

	private bool _isLerping;
	private float _timeStartedLerping;

	private Rigidbody myRigidbody;
	private Rigidbody targetRigidbody;

	private float myCollisionRadius;
	private float targetCollisionRadius;

	void Start(){
		
		myRigidbody = GetComponent<Rigidbody> ();
		targetRigidbody = target.GetComponent<Rigidbody> ();

		myCollisionRadius = GetComponent<CapsuleCollider> ().radius;
		targetCollisionRadius = target.GetComponent<CapsuleCollider> ().radius;

	}

	void Update(){

		if(target!=null){
			distanceBetweenUnits = Vector3.Distance (transform.position, target.position);
		}

		if (Input.GetMouseButtonDown (0)) {
			StartAttack ();
		}

	}

	void FixedUpdate(){
		if (_isLerping) {
			Attack ();
		}

	}

	void StartAttack(){
		
		_isLerping = true;
		_timeStartedLerping = Time.time;
	}
	void Attack(){

		if (distanceBetweenUnits <= dashDistance) {
			if (_isLerping) {

				// direction of movement
				// finish position of lerp
				Vector3 dirToTarget = (target.position - transform.position).normalized;
				Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius);

				float timeSinceStarted = Time.time - _timeStartedLerping;
				float percentageComplete = timeSinceStarted / timeTakenDuringLerp;

				float knockbackDistance = 3f;

				if (target != null) {
					transform.position = Vector3.Lerp (transform.position, attackPosition, percentageComplete * dashSpeed);
					//targetRigidbody.AddForce (dirToTarget * knockbackDistance, ForceMode.Impulse);
				}

				if (distanceBetweenUnits <= attackDistance) {
					targetRigidbody.AddForce (dirToTarget * knockbackDistance, ForceMode.Impulse);
					_isLerping = false;
				}

				if (percentageComplete >= 1f) {
					_isLerping = false;
				}
			}
		} else {

			if (_isLerping) {
				Vector3 dirToTarget = (target.position - transform.position).normalized;
				Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius);

				float timeSinceStarted = Time.time - _timeStartedLerping;
				float percentageComplete = timeSinceStarted / timeTakenDuringLerp;


				transform.position = Vector3.Lerp (transform.position, transform.position + Vector3.forward * stepDistance, percentageComplete * dashSpeed);

				if (percentageComplete >= 0.2f) {
					_isLerping = false;
				}
			}		
		}
	}
}
