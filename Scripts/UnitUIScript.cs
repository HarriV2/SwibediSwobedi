using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnitUIScript : MonoBehaviour {
	Vector3 inputVector;
	float desZ;

	Transform unitWheel;

	GameObject selectedUnit;

	// Use this for initialization
	void Start () {
		desZ = 0;
		unitWheel = transform.GetChild(2);
		unitWheel.gameObject.SetActive( false );
	}
	
	// Update is called once per frame
	void Update () {
		if ( Input.GetMouseButton(0)){
			unitWheel.transform.localRotation = Quaternion.Euler(90,0,desZ);

			// Update is called once per frame
			Vector3 mouseDelta = inputVector - Input.mousePosition;
			inputVector = Input.mousePosition;


			Vector3 realInput = inputVector - new Vector3( Screen.width * 0.5f, Screen.height * 0.5f, 0);

			if ( desZ > 360 ) desZ = 0;
			if ( desZ < 0 ) desZ = 360;
			mouseDelta = mouseDelta.normalized;
			if ( realInput.x > 0 ){
				desZ -= mouseDelta.y * 3;
			} else {
				desZ += mouseDelta.y * 3;
			}

			if ( realInput.y > 0 ){
				desZ += mouseDelta.x * 3;
			} else {
				desZ -= mouseDelta.x * 3;
			}
		}

		if (Input.GetMouseButtonDown(0)){
			unitWheel.gameObject.SetActive( true );
		}

		if (Input.GetMouseButtonUp(0)){
			selectedUnit = GetSelected ().gameObject;
			unitWheel.gameObject.SetActive( false );
		}

		if ( Input.GetKeyDown(KeyCode.Space)){
			GameObject go = Instantiate( selectedUnit.gameObject, Vector3.up * 3, Quaternion.identity);
			go.SetActive(true);
			go.AddComponent<Rigidbody>();
			go.AddComponent<CapsuleCollider>();
		}
			
	}

	Transform GetSelected(){
		float z = unitWheel.localRotation.eulerAngles.y;
		Transform temp = null;
		if ( z < 45 || z > 315 ) return unitWheel.GetChild(0);
		if ( z > 45 && z < 135 ) return unitWheel.GetChild(1);
		if ( z > 135 && z < 225 ) return unitWheel.GetChild(2);
		if ( z > 225 || z < 315 ) return unitWheel.GetChild(3);
		return temp;
	}

}
