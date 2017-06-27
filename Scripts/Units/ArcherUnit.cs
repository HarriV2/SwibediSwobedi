using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherUnit : Unit {
	public GameObject arrowPrefab;
	// Use this for initialization
	void Start () {
		//buildings = GameObject.Find ("Buildings").transform;
		state = State.move;

	}
	// Update is called once per frame
	void Update () {
			
		if (!dead) {
			switch (state) {
			case State.move:
				if (targetpos != Vector3.zero) {
					Move (targetpos);
				} else if (moveTarget != null) {
					Move (moveTarget.transform.position);
				}
				break;

			case State.targetEnemy:
				if (target != null) {
					Move (target.transform.position);
				}
				break;

			case State.attack:
				if (ticks >= 2) {
					StartCoroutine(Attack ());
					ticks = 0;
				}
				break;

			case State.die:

				break;
			}
		}
	}

	public override void UpdateAI(){
		Vector3 tempTargetPos= targetpos;
		tempTargetPos.y = transform.position.y;
		if (targetpos != Vector3.zero && Vector3.Distance (tempTargetPos, transform.position) < 1) {
			targetpos = Vector3.zero;
		}
		aggroCheck ();
		attackCheck ();
		moveCheck ();
		CheckPos ();

		if (move) {
			state = State.move;
		}
		move = true;
		ticks++;
		//Debug.Log ((state.ToString () + "State"));

	}


	IEnumerator Attack(){
		if (target != null) {
			GameObject attacktarget = target;
			yield return StartCoroutine (ShootProjectile (target.transform));

			if (target != null && attacktarget != null) {
				//DealDamage ( target.GetComponent<Unit>());
				if (attacktarget.tag == "Tower" || attacktarget.tag == "Base") {
					if (!attacktarget.GetComponent<Building> ().destroyed) {
						DealDamage (attacktarget.GetComponent<Building> ());
					}
				} else if (attacktarget.tag == "Unit") {
					if (!attacktarget.GetComponent<Unit> ().dead) {
						DealDamage (attacktarget.GetComponent<Unit> ());
					}
				}
			}
		}
	}


	IEnumerator ShootProjectile( Transform target ){
		if (target == null)
			yield break;

		float height = 10;
		GameObject projectile = Instantiate (arrowPrefab);
		projectile.transform.position = transform.position;


		float progress = -1;
		float y = 0;

		Vector3 originalPosition = transform.position;
		Vector3 directionVector;
		Vector3 middlepoint = (transform.position + target.position) / 2;

		while (progress < 1) {
			if (target == null) {
				Destroy (projectile);
				yield break;
			}

			progress += Time.deltaTime;

			Quaternion tempRot = Quaternion.LookRotation (target.position - originalPosition);
			Vector3 desRotEuler = new Vector3 (180 + progress * 90, 0, 0);
			tempRot *= Quaternion.Euler (desRotEuler);

			projectile.transform.rotation = tempRot;

			y = Mathf.Pow (progress, 2);
			middlepoint = (originalPosition + target.position) / 2;
			directionVector = (target.position - originalPosition) / 2;

			projectile.transform.position = middlepoint + directionVector * progress - Vector3.up * y  * height + Vector3.up * height;
			yield return null;
		}
			
		Destroy (projectile);
	}

}
