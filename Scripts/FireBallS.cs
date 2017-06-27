using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallS : MonoBehaviour {

	public float damageAmount;
	public float damageRange;
	bool live;

	// Use this for initialization
	void Start () {
		live = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter( Collision col){
		if ( live ){
			transform.GetComponent<Rigidbody>().isKinematic = true;
			transform.GetChild(1).GetComponent<ParticleSystem>().Play();
			Destroy(gameObject, 5f);

			Collider[] cols = Physics.OverlapSphere (transform.position, damageRange);
			foreach( Collider c in cols ){
				if ( c.tag == "Unit" ){
					c.GetComponent<Unit>().TakeDamage( damageAmount );
				}

			}

			live = false;
		}
	}

}
