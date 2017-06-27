using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour {


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter( Collision col){
		if (col.collider.tag == "Unit" && col.impulse.magnitude > 10f  ) {
			col.collider.GetComponent<Unit> ().TakeDamage (10);
			col.collider.GetComponent<Rigidbody> ().velocity = Vector3.zero;

		}

	}
}
