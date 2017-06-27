using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutTable : MonoBehaviour {

	TextMesh tm;

	List<GameObject> loadout;

	// Use this for initialization
	void Start () {
		loadout = new List<GameObject>();
		tm = GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter( Collider col){
		if ( col.tag == "Miniature" ){
			loadout.Add(col.gameObject);
		}

		tm.text = "Loadout: " + loadout.Count + "/8";
	}

	void OnTriggerExit( Collider col){
		if ( col.tag == "Miniature") {
		loadout.Remove( col.gameObject );
		}

		tm.text = "Loadout: " + loadout.Count + "/8";
	}
}
