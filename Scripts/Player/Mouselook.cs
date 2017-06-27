using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouselook : MonoBehaviour {

	public bool enabled;
	float x, y;

	// Use this for initialization
	void Start () {
		if (enabled) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		x = y = 0;
	}

	// Update is called once per frame
	void Update () {
		if (enabled) {
			x += Input.GetAxis ("Mouse X");
			y -= Input.GetAxis ("Mouse Y");

			transform.localRotation = Quaternion.Euler (y, 0, 0);
			transform.parent.rotation = Quaternion.Euler (0, x, 0);

			Vector3 inputVector = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
			inputVector = transform.parent.TransformVector (inputVector) * 3;
			transform.parent.position += inputVector * Time.deltaTime;
		}
			


	}
}
