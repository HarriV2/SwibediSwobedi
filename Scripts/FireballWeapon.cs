using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballWeapon : MonoBehaviour {

	public GameObject fireballPrefab;

	GameObject go;

	FixedJoint joint;

	// Use this for initialization
	void Start () {
		joint = gameObject.AddComponent<FixedJoint> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void InstantiateFireball(){
		go = Instantiate (fireballPrefab, transform.position, Quaternion.identity, transform);
		joint.connectedBody = go.GetComponent<Rigidbody> ();
	}

	public void ReleaseFireball(Vector3 deviceVel, Vector3 deviceAngular){
		joint.connectedBody = null;
		Rigidbody rb = go.GetComponent<Rigidbody> ();
		go.transform.parent = null;
		rb.isKinematic = false;
		rb.velocity = deviceVel * 3;
		rb.angularVelocity = deviceAngular;
	}
}
