using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (UnityEngine.AI.NavMeshAgent))]
[RequireComponent(typeof (Renderer))]
public class Enemy : BaseEntity {

	public enum State{
		Idle,
		Chasing,
		Attacking
	};

	public ParticleSystem deathEffect;
	State currentState;

	UnityEngine.AI.NavMeshAgent pathfinder;
	BaseEntity targetEntity;
	Transform target;
	Material material;

	Color originalColor;

	float attackDistanceThreshold = 1f;
	float timeBetweenAttacks = 1;

	float nextAttackTime;
	float myCollisionRadius;
	float targetCollisionRadius;

	bool hasTarget;

	float damage = 1;

	void Awake(){
		pathfinder = GetComponent<UnityEngine.AI.NavMeshAgent> ();

		if (GameObject.FindGameObjectWithTag ("Player") != null) {
			hasTarget = true;

			target = GameObject.FindGameObjectWithTag ("Player").transform;
			targetEntity = target.GetComponent<BaseEntity> ();

			myCollisionRadius = GetComponent<CapsuleCollider> ().radius;
			targetCollisionRadius = target.GetComponent < CapsuleCollider > ().radius;
		}
	}

	protected override void Start(){
		base.Start ();

		if (GameObject.FindGameObjectWithTag ("Player") != null) {
			currentState = State.Chasing;
			targetEntity.OnDeath += OnTargetDeath;

			StartCoroutine (UpdatePath ());
		}
	}

	public void SetCharacteristics(float moveSpeed, int hitsToKillPlayer,float enemyHealth,Color skinColor){
		pathfinder.speed = moveSpeed;
		if (hasTarget) {
			damage = Mathf.Ceil(targetEntity.startingHealth / hitsToKillPlayer);
		}
		startingHealth = enemyHealth;

		material = GetComponent<Renderer> ().material;
		material.color = skinColor;
		originalColor = material.color;

	}

	public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection){
		if (damage >= health) {
			Destroy(Instantiate(deathEffect.gameObject,hitPoint,Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, deathEffect.startLifetime);
		}
		base.TakeHit (damage, hitPoint, hitDirection);
	}

	void OnTargetDeath(){
		hasTarget = false;
		currentState = State.Idle;
	}

	void Update(){

		if (hasTarget) {
			if (Time.time > nextAttackTime) {
				float sqDstToTarget = (target.position - transform.position).sqrMagnitude;
				//attack range squared
				if (sqDstToTarget < Mathf.Pow (attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2)) {
					nextAttackTime = Time.time + timeBetweenAttacks;
					StartCoroutine (Attack ());
				}
			}
		}


	}


	IEnumerator UpdatePath(){
		//Refresh in seconds
		float refreshRate = .25f;

		while (hasTarget) {
			if (currentState == State.Chasing) {
				Vector3 dirToTarget = (target.position - transform.position).normalized;
				Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThreshold / 2);
				if (!dead) {
					pathfinder.SetDestination (targetPosition);
				}
			}
			yield return new WaitForSeconds (refreshRate);
		}
	}

	IEnumerator Attack(){

		currentState = State.Attacking;
		pathfinder.enabled = false;
		transform.LookAt (target);

		Vector3 dirToTarget = (target.position - transform.position).normalized;
		Vector3 originalPosition = transform.position;
		Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius);

		float attackSpeed = 3;
		float percent = 0;

		material.color = Color.red;
		bool hasAppliedDamage = false;

		while (percent <= 1) {

			if (percent >= .5f && !hasAppliedDamage) {
				hasAppliedDamage = true;
				targetEntity.TakeDamage (damage);
			}
			percent += Time.deltaTime * attackSpeed;
			float interpolation = (-Mathf.Pow (percent, 2) + percent) * 4;
			transform.position = Vector3.Lerp (originalPosition, attackPosition, interpolation);

			yield return null;

		}

		material.color = originalColor;
		currentState = State.Chasing;
		pathfinder.enabled = true;
	}
}
