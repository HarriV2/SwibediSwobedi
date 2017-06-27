using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuRoomController : MonoBehaviour {

	SteamVR_TrackedObject trackedObj;
	public LayerMask miniatures;

	FixedJoint joint;

	void Awake()
	{
		trackedObj = GetComponent<SteamVR_TrackedObject>();

	}
	// Use this for initialization
	void Start () {
		joint = GetComponent<FixedJoint> ();
	}
	
	// Update is called once per frame
	void Update () {
		var device = SteamVR_Controller.Input((int)trackedObj.index);

		if (device.GetPress (SteamVR_Controller.ButtonMask.Trigger)) {
			if (device.GetPressDown (SteamVR_Controller.ButtonMask.Trigger)) {
				GameObject go = GetOverlappingMiniature ();

				if (joint.connectedBody == null ) {
					if (go != null) {
						go.transform.position = transform.position;
						joint.connectedBody = go.GetComponent<Rigidbody> ();
					}
				}
			}
		}

		if (device.GetPressUp (SteamVR_Controller.ButtonMask.Trigger)) {
			if (joint.connectedBody != null) {
				Rigidbody temp = joint.connectedBody.GetComponent<Rigidbody> ();
				temp.velocity = device.velocity;
				temp.angularVelocity = device.velocity;

				joint.connectedBody = null;
			}
		}


	}






	GameObject GetOverlappingMiniature(){
		Collider[] overlap = Physics.OverlapSphere (transform.position, 0.2f, miniatures);
		if (overlap.Length > 0) {
			return overlap [0].gameObject;
		}
		return null;
	}
}
