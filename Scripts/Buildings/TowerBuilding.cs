using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBuilding : Building {
	public GameObject arrowPrefab;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (ticks >= 1) {
			if (target != null) {
				ticks = 0;
				StartCoroutine (Attack ());
			}
		}
	}

	public void UpdateAI(){
		target = null;
		AttackCheck ();
		ticks++;
	}

	IEnumerator Attack(){
		if (target != null) {
			if (!target.GetComponent<Unit> ().dead) {
				GameObject attacktarget = target;
				yield return StartCoroutine (ShootProjectile (target.transform));

				if (target != null) {
					if (!attacktarget.GetComponent<Unit> ().dead) {
						if (attacktarget.tag == "Unit") {
							DealDamage (attacktarget.GetComponent<Unit> ());
						}
					}
				}
			}
		}
	}

	IEnumerator ShootProjectile( Transform target ){
		if (target == null)
			yield break;

		float height = 10;
		GameObject projectile = Instantiate (arrowPrefab);
		projectile.transform.position = transform.position;


		float progress = -1;
		float y = 0;

		Vector3 originalPosition = transform.position;
		Vector3 directionVector;
		Vector3 middlepoint = (transform.position + target.position) / 2;

		while (progress < 1) {
			if (target == null) {
				Destroy (projectile);
				yield break;
			}

			progress += Time.deltaTime;

			Quaternion tempRot = Quaternion.LookRotation (target.position - originalPosition);
			Vector3 desRotEuler = new Vector3 (180 + progress * 90, 0, 0);
			tempRot *= Quaternion.Euler (desRotEuler);

			projectile.transform.rotation = tempRot;

			y = Mathf.Pow (progress, 2);
			middlepoint = (originalPosition + target.position) / 2;
			directionVector = (target.position - originalPosition) / 2;

			projectile.transform.position = middlepoint + directionVector * progress - Vector3.up * y  * height + Vector3.up * height;
			yield return null;
		}

		Destroy (projectile);
	}
}
