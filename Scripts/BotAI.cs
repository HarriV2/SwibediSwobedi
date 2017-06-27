using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotAI : MonoBehaviour {

	public GameObject[] unitPrefabs;


	float spawnTimer = 0;
	public float spawnInterval;
	public bool team;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		spawnTimer += Time.deltaTime;
		if (spawnTimer > spawnInterval) {
			SpawnUnit (Random.Range (0, unitPrefabs.Length ));
			spawnTimer = 0;
		}



	}


	void SpawnUnit(int i){
		int pos = Random.Range (0, transform.childCount);

		GameObject go = Instantiate (unitPrefabs [i], transform.GetChild (pos).position, Quaternion.identity, GameManager.Instance.unitContainer.GetChild(1));
		Unit u = go.GetComponent<Unit> ();
		u.player = GameManager.Instance.player;
		u.team = Team.Black;
		go.transform.position += Vector3.up * u.spawningOffsetY;

	}

}
