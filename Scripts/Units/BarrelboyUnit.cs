using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelboyUnit : Unit {

	public float explosionRadius;

	// Use this for initialization
	void Start () {
		//buildings = GameObject.Find ("Buildings").transform;
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ();
		state = State.move;

	}

	// Update is called once per frame
	void Update () {
		
		if (!dead) {
			switch (state) {
			case State.move:
				if (targetpos != Vector3.zero) {
					Move (targetpos);
				} else if (target != null) {
					Move (target.transform.position);
				}
				break;

			case State.targetEnemy:
				if (target != null) {
					Move (target.transform.position);
				}
				break;

			case State.attack:
				if (ticks >= 1) {
					Attack ();
					Die ();
				}
				break;

			case State.die:

				break;
			}
		}
	}


	public override void UpdateAI(){

		if (targetpos != Vector3.zero && Vector3.Distance (targetpos, transform.position) < 1) {
			targetpos = Vector3.zero;
		}
		moveCheck ();
		aggroCheck ();
		attackCheck ();

		if (move) {
			state = State.move;
		}
		move = true;
		ticks++;
		//Debug.Log ((state.ToString () + "State"));

	}
		
	void Attack(){
		transform.GetChild (2).GetComponent<ParticleSystem> ().Play ();

		Collider[] col = Physics.OverlapSphere (transform.position, explosionRadius);
		foreach (Collider c in col) {
			if (c.tag == "Unit") {
				if (c.gameObject != gameObject) {
					DealDamage (c.GetComponent<Unit> ());
				}
			}
		}

	}

	public override void Die(){
		if (!dead) {

			base.Die ();
			Attack ();
			GetComponent<Rigidbody> ().AddForce (Vector3.up * 9, ForceMode.VelocityChange);
		}
	}

}
	
