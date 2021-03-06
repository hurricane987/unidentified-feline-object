using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSpawn : MonoBehaviour {

	public int howFarOut = 10; 
	public Vector3 startPos = new Vector3 (-15f, 0f, 25f);
	public float blimpMin = -0.5f;
	public float blimpMax = 0.5f;

	public GameObject ground;

	public GameObject spawnCollider;
	public GameObject game;

	private SpawnPeople spawnPeople;
	private SpawnBuildings spawnBuildings;
	private SpawnDecor spawnDecor;
	private SpawnAirObstacles spawnAirObstacles;

	private float groundWidth;
	private Vector3 spawnPos;
	private bool canSpawnAirObjects = false;

	// Use this for initialization
	void Awake () {
		groundWidth = ground.transform.localScale.x;
		spawnPos = startPos;
		game = GameObject.Find ("Game");
		spawnBuildings = game.GetComponent<SpawnBuildings> ();
		spawnPeople = game.GetComponent<SpawnPeople> ();
		spawnDecor = game.GetComponent<SpawnDecor> ();
		spawnAirObstacles = game.GetComponent<SpawnAirObstacles> ();
		for (int i = 0; i < howFarOut; i++) {
			Spawn ();
		}	
		canSpawnAirObjects = true;
	}

	int Flip (int max) {
		return (int)Mathf.Floor (Random.Range (0, max));
	}

	Vector3 GetBlimpPosition(Transform pos) {
		return new Vector3 (pos.position.x, pos.position.y + Random.Range (blimpMin, blimpMax), pos.position.z);
	}

	public void Spawn() {

		GameObject groundObj = Instantiate (ground, spawnPos, ground.transform.rotation);
		Transform spawnColliderTransform = groundObj.transform.Find("Slot").Find ("SpawnColliderPosition");
		Instantiate (spawnCollider, spawnColliderTransform.position, Quaternion.identity);
		int category = Flip (20);
		if (category <= 10) {
			foreach (Transform child in groundObj.transform) {
				if (child.name == "Slot") {
					spawnBuildings.SpawnApartments (child);
				}
			}
		} else {
			Transform slot = groundObj.transform.Find ("Slot");
			Transform alley = Utils.FindChildByTag (groundObj.transform, "Alley");
			Transform airPosition = Utils.FindChildByTag (groundObj.transform, "AirObstacle");
			Transform park = groundObj.transform.Find ("ParkPosition");
			if (category > 10 && category <= 17) {
				spawnDecor.SpawnPark (park);
				if (Utils.RandomNumber (3) > 0 && canSpawnAirObjects) {
					spawnAirObstacles.Spawn (airPosition);
				}
			} else {
				spawnBuildings.SpawnOffices (slot);
				spawnDecor.SpawnAlley (alley);
			}
		}

		foreach (Transform childTr in groundObj.transform) {
			int flip = Utils.RandomNumber (3);
			if (flip > 0) {
				spawnPeople.Spawn (childTr);
			} else if(childTr.name != "ParkPosition") {
				spawnDecor.SpawnTrees (childTr);
			}
		}

		//since each child has a mesh and therefore width, need to multiply by number of chilldren
		spawnPos += new Vector3 (groundWidth * 3.0f, 0f, 0f);
	}
}
