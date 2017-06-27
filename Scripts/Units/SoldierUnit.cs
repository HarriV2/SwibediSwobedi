using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierUnit : Unit {

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
				if (ticks >= 1) {
					attack ();
					ticks = 0;
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
		
	void attack(){
		if (target.tag == "Tower" || target.tag == "Base") {
			DealDamage(target.GetComponent<Building>());
		}
		else if (target.tag == "Unit") {
			DealDamage(target.GetComponent<Unit>());
		}
	}

}

public enum State{
	move,
	targetEnemy,
	attack,
	die,
}
