using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour {

	public float currentHealth, maxHealth;
	public float attackRange, attackDamage;
	public bool destroyed;
	public GameObject target;
	public float ticks=0;

	public Team team;

	// Use this for initialization
	void Start () {
		destroyed = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void DealDamage (Unit u){
		u.TakeDamage (attackDamage); 
	}

	public void TakeDamage(float amount){
		currentHealth -= amount;
		if (currentHealth <= 0) {
			Die ();
		}
	}

	public void Die(){
		//destroyed = true;
		SwitchTeam ();
		RestoreHealth ();
		//Destroy (gameObject, 3f);
	}
	void RestoreHealth(){
		currentHealth = maxHealth;
	}

	void SwitchTeam(){
		if (team == Team.Black) {
			team = Team.White;
			transform.GetChild (0).gameObject.SetActive (true);
			transform.GetChild (1).gameObject.SetActive (false);
		} else {
			team = Team.Black;
			transform.GetChild (1).gameObject.SetActive (true);
			transform.GetChild (0).gameObject.SetActive (false);
		}
	}

	public void AttackCheck(){
		float closestDistance = attackRange;
			Collider[] cols = Physics.OverlapSphere (transform.position, attackRange);
			foreach (Collider col in cols) {

				if ((col.tag == "Unit") && !col.transform.GetComponent<Unit> ().dead) {
					if (col.transform.GetComponent<Unit> ().team != team) {
					if (Vector3.Distance (col.transform.position, transform.position) < closestDistance) {
						closestDistance = Vector3.Distance (col.transform.position, transform.position);
						target = col.gameObject;

					}
					}
				}

			}

	}
}
