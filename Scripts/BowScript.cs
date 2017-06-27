using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowScript : MonoBehaviour {

	Transform bowTrans;
	public Transform upperStringBase, lowerStringBase, upperString, lowerString;
	public Transform arrow;

	float drawStrength;

	Rigidbody arrowRb;

	// Use this for initialization
	void Start () {
		bowTrans = transform.GetChild (1);
		drawStrength = 0;
		arrowRb = arrow.GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Draw( Vector3 handPos){
		if (!arrowRb.isKinematic)
			arrowRb.isKinematic = true;

		bowTrans.rotation = Quaternion.LookRotation ( transform.position - handPos);

		Vector3 upperMid = (upperStringBase.position + handPos) / 2;
		Vector3 lowerMid = (lowerStringBase.position + handPos) / 2;

		float upperDist = Vector3.Distance (upperStringBase.position, handPos);
		float lowerDist = Vector3.Distance (lowerStringBase.position, handPos);

		upperString.transform.position = upperMid;
		upperString.localScale = new Vector3 (0.01f, upperDist / 6, 0.01f);
		upperString.rotation = Quaternion.LookRotation (upperStringBase.position - handPos) * Quaternion.Euler (-90, 0, 0);

		lowerString.transform.position = lowerMid;
		lowerString.localScale = new Vector3 (0.01f, lowerDist / 6, 0.01f);
		lowerString.rotation = Quaternion.LookRotation (lowerStringBase.position - handPos) * Quaternion.Euler (-90, 0, 0);

		arrow.position = handPos;
		arrow.rotation = Quaternion.LookRotation (bowTrans.position - handPos);

		drawStrength = Vector3.Distance (handPos, bowTrans.position);
	}

	public void Release(){

		arrowRb.isKinematic = false;
		arrow.SetParent (null);

		arrowRb.AddForce (arrow.forward * (drawStrength / 10 ) * 1000, ForceMode.VelocityChange);

		upperString.localRotation = Quaternion.identity;
		upperString.localPosition = new Vector3 (0, 0.3931f, -0.189f);
		upperString.localScale = new Vector3 (0.01f, 0.40f, 0.01f);

		lowerString.localRotation = Quaternion.identity;
		lowerString.localPosition = new Vector3 (0, -0.393f, -0.189f);
		lowerString.localScale = new Vector3 (0.01f, 0.40f, 0.01f);

	}
		
}
