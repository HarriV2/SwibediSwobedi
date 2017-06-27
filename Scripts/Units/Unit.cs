using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

	public PlayerController player;
	//public float timer = 0;
	public float ticks;
	public Team team;
	public Vector3 targetpos= Vector3.zero;
	public bool dead = false;
	public UnitType unitType;
	public float maxHealth, currentHealth;
	public float attackRange, attackDamage, aggroRange, armorRating, movementSpeed;
	public State state;

	public GameObject moveTarget;
	public GameObject target;
	public bool move=true;

	public Transform healthBar;
	public Material enemyHPBM;

	public float resourceCost;

	public int spawnedAtOnce;
	public float spawningOffsetY;
	public float radius;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	public virtual void UpdateAI(){
		//currentHealth += Time.deltaTime;
	}
		
	public void DealDamage (Unit u){
		u.TakeDamage (attackDamage); 
	}

	public void DealDamage (Building b){
		b.TakeDamage (attackDamage); 
	}

	public void TakeDamage(float amount){
		if (!dead) {
			currentHealth -= amount;


			// if ground unit

			if (!healthBar.gameObject.activeSelf) {
				healthBar.gameObject.SetActive (true);
				if (team != player.team) {
					healthBar.GetChild (1).GetChild (0).GetComponent<Renderer> ().material = enemyHPBM;
				}
			}


			float step = amount / maxHealth;
			float barX = healthBar.GetChild (1).localScale.x - step;
			barX = Mathf.Clamp (barX, 0, 1);

			healthBar.GetChild (1).localScale = new Vector3 (barX, 1, 1);

			if (currentHealth <= 0) {
				Die ();
			}
		}
	}

	public void Move(Vector3 targetPos){
		targetPos.y = transform.position.y;
		transform.LookAt (targetPos);
		Vector3 movement = (targetPos - transform.position).normalized * movementSpeed * Time.deltaTime;
		movement.y = 0;
		//Debug.Log ("movement: "  + movement);
		transform.position += movement;

	}
	public void setPos(Vector3 pos){
		targetpos = pos;
	}

	public virtual void Die(){
		if (!dead) {
			// healthBar.gameObject.SetActive (false);
			dead = true;
			GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
			Destroy (gameObject, 10f);
		}
	}
	public void moveCheck(){

		if (state == State.move) {
			target = null;
			int value = GetTower ();
			moveTarget = GameManager.Instance.buildingList [value].gameObject;
		}
	}

	int GetTower(){
		if (team == Team.White) {
			if (transform.position.x < 0) {
				if (GameManager.Instance.buildingList [1].team == Team.Black)
					return 1;
				if (GameManager.Instance.buildingList [2].team == Team.Black)
					return 2;
				if (GameManager.Instance.buildingList [3].team == Team.Black)
					return 3;
				if (GameManager.Instance.buildingList [4].team == Team.Black)
					return 4;
				else
					return 9;
			}
			else{
				if (GameManager.Instance.buildingList [5].team == Team.Black)
					return 5;
				if (GameManager.Instance.buildingList [6].team == Team.Black)
					return 6;
				if (GameManager.Instance.buildingList [7].team == Team.Black)
					return 7;
				if (GameManager.Instance.buildingList [8].team == Team.Black)
					return 8;
				else
					return 9;
			}
		}
		else{
			if (transform.position.x >= 0) {
				if (GameManager.Instance.buildingList [8].team == Team.White)
					return 8;
				if (GameManager.Instance.buildingList [7].team == Team.White)
					return 7;
				if (GameManager.Instance.buildingList [6].team == Team.White)
					return 6;
				if (GameManager.Instance.buildingList [5].team == Team.White)
					return 5;
				else
					return 0;
			}
			else{
				if (GameManager.Instance.buildingList [4].team == Team.White)
					return 4;
				if (GameManager.Instance.buildingList [3].team == Team.White)
					return 3;
				if (GameManager.Instance.buildingList [2].team == Team.White)
					return 2;
				if (GameManager.Instance.buildingList [1].team == Team.White)
					return 1;
				else
					return 0;
			}
		}
	}

	public void aggroCheck(){
		float closestDistance = aggroRange;
		Collider[] colliders = Physics.OverlapSphere(transform.position, aggroRange );
		foreach (Collider col in colliders) {
			if (col.tag == "Unit" && !col.transform.GetComponent<Unit> ().dead) {
				if (col.transform.GetComponent<Unit> ().team != team) {

					Vector3 enemypos = col.transform.position;
					enemypos.y = transform.position.y;
					if ((Vector3.Distance (col.transform.position, transform.position)-col.transform.GetComponent<Unit>().radius) < closestDistance) {
						closestDistance = Vector3.Distance (col.transform.position, transform.position);
						target = col.gameObject;
					}
					state = State.targetEnemy;
					move = false;
				} 

			} else if ((col.tag == "Base" || col.tag == "Tower")) {
				if (col.transform.GetComponent<Building> ().team != team) {
					
					int value = GetTower ();

					if (col.gameObject == GameManager.Instance.buildingList [value].gameObject) {

						Vector3 enemypos = col.transform.position;
						enemypos.y = transform.position.y;
						Debug.Log ("distance: " + Vector3.Distance (enemypos, transform.position) + "closest: " + closestDistance);
						if ((Vector3.Distance (col.transform.position, transform.position)-col.transform.GetComponent<Building>().radius) < closestDistance) {
							Debug.Log ("bleep");
							closestDistance = Vector3.Distance (col.transform.position, transform.position);
							target = col.gameObject;

						}
						state = State.targetEnemy;
						move = false;
					}
				}
			}
		}
	}

	public void attackCheck(){
		if (!move) {
			Collider[] cols = Physics.OverlapSphere (transform.position, attackRange);
			foreach (Collider col in cols) {
				if ((col.tag == "Base" || col.tag == "Tower")) {
					int value = GetTower ();

					if (col.gameObject == GameManager.Instance.buildingList [value].gameObject) {

						Vector3 enemypos = col.transform.position;
						enemypos.y = transform.position.y;
						if (col.transform.GetComponent<Building> ().team != team && (Vector3.Distance (enemypos, transform.position)-col.transform.GetComponent<Building>().radius) < attackRange) {
							state = State.attack;
						}
					}
				}
				if ((col.tag == "Unit") && !col.transform.GetComponent<Unit> ().dead) {
					Vector3 enemypos = col.transform.position;
					enemypos.y = transform.position.y;
					if (col.transform.GetComponent<Unit> ().team != team && (Vector3.Distance (enemypos, transform.position)-col.transform.GetComponent<Unit>().radius) < attackRange) {
						state = State.attack;
					}
				}

			}
		}
	}
	public void CheckPos(){

		if (team == Team.White) {
			if (transform.position.z > moveTarget.transform.position.z) {
				targetpos = Vector3.zero;
			}
		} else {
			if (transform.position.z < moveTarget.transform.position.z) {
				targetpos = Vector3.zero;
			}
		}
	}


}

public enum UnitType { Soldier = 0 }
