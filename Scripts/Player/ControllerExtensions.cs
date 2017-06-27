using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerExtensions : MonoBehaviour {
	public SteamVR_ControllerManager cm;
	public ControllerState state;
	List<Unit> lastSpawnedUnits;

	PlayerController player;
	SteamVR_TrackedObject trackedObj;

	LineRenderer lr;

	public Transform weaponTrans;
	public BowScript bowScript;
	public bool bowActive;
	public bool bowDrawn;

	public FireballWeapon fireballWeapon;

	public ControllerExtensions leftExt;
	public ControllerExtensions rightExt;

	public Transform utilities;
	public LineRenderer targetIndicator;

	public Transform unitWheel;

	Vector2 touchVector;
	float unitWheelRot;

	void Awake()
	{
		trackedObj = GetComponent<SteamVR_TrackedObject>();

	}

	// Use this for initialization
	void Start () {
		player = transform.parent.GetComponent<PlayerController> ();
		cm = transform.parent.GetComponent<SteamVR_ControllerManager> ();
		state = ControllerState.SpawningUnits;
		lastSpawnedUnits = new List<Unit> ();
		lr = GetComponent<LineRenderer> ();

		// left hand initialization
		if (gameObject == cm.left) {
			weaponTrans = transform.GetChild (1);
			bowActive = false;
		// right hand initialization
		} else {
			bowDrawn = false;
			fireballWeapon = GetComponent<FireballWeapon> ();
			unitWheel = transform.GetChild (0);
		}

		bowScript = cm.left.GetComponent<BowScript> ();

		// get both ControllerExtensions for multi-hand actions (bow)
		leftExt = cm.left.GetComponent<ControllerExtensions> ();
		rightExt = cm.right.GetComponent<ControllerExtensions> ();

		targetIndicator = utilities.GetChild (1).GetComponent<LineRenderer> ();

		unitWheelRot = 0;
	}

	// Update is called once per frame
	void Update () {
		var device = SteamVR_Controller.Input((int)trackedObj.index);

		// left hand controls
		// targeting assistance linerenderer
		if (gameObject == cm.left) {
			if (device.GetTouch (SteamVR_Controller.ButtonMask.Trigger)) {
				lr.enabled = true;
			} else {
				lr.enabled = false;
			}


			// using bow
			if (device.GetPress (SteamVR_Controller.ButtonMask.Grip)) {
				if (!weaponTrans.gameObject.activeSelf) {
					weaponTrans.gameObject.SetActive (true);
					bowActive = true;
				}
			} else {
				if (weaponTrans.gameObject.activeSelf) {
					bowActive = false;
					weaponTrans.gameObject.SetActive (false);
				}
			}

			// player tower teleporting
			if (device.GetAxis (Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x > 0.95f) {
				Ray ray = new Ray (transform.position, transform.forward);
				RaycastHit rayHit;
				if (Physics.Raycast( ray, out rayHit)) {
					if (rayHit.collider.tag == "Base" || rayHit.collider.tag == "Tower") {
						if (rayHit.collider.transform.parent.name == "White" && player.team == Team.White || rayHit.collider.transform.parent.name == "Black" && player.team == Team.Black) {
							if (rayHit.collider.tag == "Base") {
								transform.parent.position = new Vector3 (rayHit.collider.transform.position.x, 25.2f, rayHit.collider.transform.position.z);
							}
							if (rayHit.collider.tag == "Tower") {
								transform.parent.position = new Vector3 (rayHit.collider.transform.position.x, 17.5f, rayHit.collider.transform.position.z);
							}
						}
					}
				}
			}
		

		}

		// right hand controls;
		if (gameObject == cm.right) {
			// bow drawing
			if (leftExt.bowActive) {
				if (device.GetPress (SteamVR_Controller.ButtonMask.Trigger)) {
					bowScript.Draw (transform.position);
					device.TriggerHapticPulse (500);
					bowDrawn = true;
				}
			} else {

				//  targeting assist linerenderer 
				if (device.GetTouch (SteamVR_Controller.ButtonMask.Trigger)) {
					lr.enabled = true;
				} else {
					lr.enabled = false;
				}

				// unit spawning controls
				if (state == ControllerState.SpawningUnits) {						
					Ray ray = new Ray (transform.position, transform.forward);
					RaycastHit rayHit;

					if (device.GetPress (SteamVR_Controller.ButtonMask.Trigger) && device.GetAxis (Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger).x > 0.95f) {
						if (Physics.Raycast (ray, out rayHit)) {
							if (rayHit.collider.tag == "Ground") {
								if (Vector3.Distance (transform.position, rayHit.point) < 60) {
									if (player.UseResource (player.UnitPrefabs [GetSelectedUnitIndex ()].GetComponent<Unit> ().resourceCost)) {

										lastSpawnedUnits.AddRange(player.InstantiateUnits (rayHit.point, GetSelectedUnitIndex ()));
										state = ControllerState.WaitingForCommand;
										EnableTargetIndicator ();
									} 
								}
							}

						}
					}

					if (device.GetPress (SteamVR_Controller.ButtonMask.Trigger)) {
						if (Physics.Raycast (ray, out rayHit)) {
							if (rayHit.collider.tag == "Ground") {
								if (Vector3.Distance (transform.position, rayHit.point) < 60) {
									lr.startColor = Color.green;
								} else
									lr.startColor = Color.red;
							}
						}
					}

				// unit movement command
				} else if (state == ControllerState.WaitingForCommand) {

					Ray ray = new Ray (transform.position, transform.forward);
					RaycastHit rayHit;

					// update visuals for unit movement command
					if (Physics.Raycast (ray, out rayHit)) {
						if (rayHit.collider.tag == "Ground") {
							UpdateTargetPose (lastSpawnedUnits[0].transform.position, rayHit.point);
						}
					}

					// ending method
					if (device.GetPressUp (SteamVR_Controller.ButtonMask.Trigger)) {

						if (Physics.Raycast (ray, out rayHit)) {
							if (rayHit.collider.tag == "Ground") {
								float dist = Vector3.Distance (rayHit.point, lastSpawnedUnits[0].transform.position);
								if (dist > 1f && dist < 30f) {
									foreach (Unit u in lastSpawnedUnits) {
										u.setPos (rayHit.point);
									}
			
								}
							}

							DisableTargetIndicator ();
							lastSpawnedUnits.Clear ();
							state = ControllerState.SpawningUnits;
	
						}
					}


				}
			}

			// unit selection wheel controls
			if (device.GetTouchDown (SteamVR_Controller.ButtonMask.Touchpad)) {
				unitWheel.gameObject.SetActive (true);
			}

			if (device.GetTouchUp (SteamVR_Controller.ButtonMask.Touchpad)) {
				unitWheel.gameObject.SetActive (false);
			}

			if (device.GetTouch (SteamVR_Controller.ButtonMask.Touchpad)) {
				Vector2 touchDelta = touchVector - device.GetAxis (Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
				touchVector = device.GetAxis (Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);

				if (touchDelta != Vector2.zero) {
					if (touchVector.x > 0) {
						unitWheelRot += touchDelta.y * 30;
					} else {
						unitWheelRot -= touchDelta.y * 30;
					}

					if (touchVector.y > 0) {
						unitWheelRot -= touchDelta.x * 30;
					} else {
						unitWheelRot += touchDelta.x * 30;
					}

					if (unitWheelRot > 360)
						unitWheelRot = 0;
					if (unitWheelRot < 0)
						unitWheelRot = 360;

					float mod = unitWheelRot % 45;
					if (mod < 10 || mod > 35) {
						device.TriggerHapticPulse (1000);
					}


					unitWheel.transform.localRotation = Quaternion.Euler (0, unitWheelRot, 0);
				}
			}

			// fireball throwing controls
			if ( device.GetPress (Valve.VR.EVRButtonId.k_EButton_Grip )) {
				if ( device.GetPressDown (Valve.VR.EVRButtonId.k_EButton_Grip )){
					if (player.UseResource (5)) {
						fireballWeapon.InstantiateFireball ();
					}
				}
			}
			if ( device.GetPressUp (Valve.VR.EVRButtonId.k_EButton_Grip )){
				fireballWeapon.ReleaseFireball (device.velocity, device.angularVelocity);
			}

			// bow release
			if (!device.GetPress (Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger)) {
				if (bowDrawn) {
					bowScript.Release ();
					bowDrawn = false;
				}
			}


		}

		/*
		 *
		if (device.GetTouch (Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger)) {
			if (gameObject == cm.left) {
				if (device.GetTouchDown (Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger)) {
					Ray ray = new Ray (transform.position, transform.forward);
					RaycastHit rayHit;
					if (Physics.Raycast (ray, out rayHit)) {
						if (rayHit.collider.tag == "Ground") {
							if (state == ControllerState.SpawningUnits) {
								lastSpawnedUnit = player.InstantiateUnit (rayHit.point);
								state = ControllerState.WaitingForCommand;
							} else if (state == ControllerState.WaitingForCommand) {
								lastSpawnedUnit.setPos (rayHit.point);
								state = ControllerState.SpawningUnits;
							}
						}

					}
				}

			}


		}
		*/

	}

	int GetSelectedUnitIndex(){
		if (unitWheelRot < 22.5 || unitWheelRot > 337.5)
			return 0;
		if (unitWheelRot > 22.5 && unitWheelRot < 67.5)
			return 7;
		if (unitWheelRot > 67.5 && unitWheelRot < 112.5)
			return 6;
		if (unitWheelRot > 112.5 && unitWheelRot < 157.5)
			return 5;
		if (unitWheelRot > 157.5 && unitWheelRot < 202.5)
			return 4;
		if (unitWheelRot > 202.5 && unitWheelRot < 247.5)
			return 3;
		if (unitWheelRot > 247.5 && unitWheelRot < 292.5)
			return 2;
		if (unitWheelRot > 292.5 && unitWheelRot < 337.5)
			return 1;
		return 0;
	}

	void EnableTargetIndicator(){
		targetIndicator.gameObject.SetActive (true);
	}

	void UpdateTargetPose(Vector3 origin, Vector3 dest ){
		float dist = Vector3.Distance (origin, dest);
		if (dist > 1 && dist < 30) {
			targetIndicator.startColor = targetIndicator.endColor  = Color.green;
		} else {
			targetIndicator.startColor = targetIndicator.endColor = Color.red;
		}
			
		targetIndicator.SetPosition (0, origin + Vector3.up * 0.2f);
		targetIndicator.SetPosition (1, dest + Vector3.up * 0.2f );
	}

	void DisableTargetIndicator(){
		targetIndicator.gameObject.SetActive (false);
	}


}

public enum ControllerState { SpawningUnits, WaitingForCommand }
