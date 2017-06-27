using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public Transform unitContainer;
	float timer = 0;
	public Transform buildings;
	public Building[] buildingList;

	public static GameManager Instance;
	// Use this for initialization
	void Start () {
		Instance = this;
		buildingList = new Building[10];
		for (int i = 0; i < 10; i++) {
			buildingList [i] = buildings.GetChild (i).GetComponent<Building> ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (timer >= 1) {
			BroadcastMessage("UpdateAI");
			timer = 0;
		}
	}
}
