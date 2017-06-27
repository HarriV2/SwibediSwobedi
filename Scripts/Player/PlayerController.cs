using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public Team team;
	public bool MouseTeleport;

	public GameObject[] UnitPrefabs;
	public int selectedUnitIndex;

	public Transform resourceBar;
	public TextMesh resourceText;

	public float maxResource;
	public float currentResource;
	public float resourcePerSecond;

	// Use this for initialization
	void Start () {
		currentResource = maxResource / 2;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (currentResource < maxResource) {
			currentResource += resourcePerSecond * Time.deltaTime;
		}
		currentResource = Mathf.Clamp (currentResource, 0, maxResource);
		resourceBar.GetChild (0).localScale = new Vector3 (currentResource / maxResource,1,1);
		resourceText.text = currentResource.ToString ("F0") + "/\n" + maxResource.ToString ("F0");
	}

	// Uses resource if you have enough, and returns true. returns false if you don't have enough.
	public bool UseResource(float amount){
		if (amount < currentResource) {
			currentResource -= amount;
			return true;
		} else {
			return false;
		}
	}

	/*
	public GameObject GetSelectedPrefab(int i){
		return UnitPrefabs [i];
	}
	*/

	/*
	public Unit InstantiateUnit(Vector3 pos, int unitIndex){
		GameObject go = Instantiate (UnitPrefabs[unitIndex], pos , Quaternion.identity, null);
		Unit u = go.GetComponent<Unit> ();
		go.transform.position += u.spawningOffsetY * Vector3.up;
		return u;
	}
	*/

	public List<Unit> InstantiateUnits( Vector3 pos, int unitIndex ){
		List<Unit> units = new List<Unit> ();
		int amount = UnitPrefabs [unitIndex].GetComponent<Unit> ().spawnedAtOnce;

		for (int i = 0; i < amount; i++) {
			GameObject go = Instantiate (UnitPrefabs[unitIndex], pos + Vector3.right * i * 2 , Quaternion.identity, null);
			Unit u = go.GetComponent<Unit> ();
			u.player = this;
			go.transform.position += u.spawningOffsetY * Vector3.up;
			go.transform.parent = GameManager.Instance.unitContainer;
			units.Add (u);
		}
		return units;
	}

	bool RayCast(ref RaycastHit rh){
		if (Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out rh)) {
			
			return true;
		}
		return false;
	}

}

public enum Team {White = 0, Black = 1 }
