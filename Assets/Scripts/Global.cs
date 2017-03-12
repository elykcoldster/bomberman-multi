using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Global : NetworkBehaviour {

	public static Global instance;
	public static string[] items = new string[]{ "bomb_up", "range_up", "speed_up" };
	public static float unit = 0.5f;
	public static Vector3[] dirs;

	[SyncVar] public Vector3 up = Vector3.up;
	[SyncVar] public Vector3 down = Vector3.down;
	[SyncVar] public Vector3 left = Vector3.left;
	[SyncVar] public Vector3 right = Vector3.right;

	[SyncVar] public float dropRate = 0.25f;

	public NetworkManager networkManager;
	public BombSpawn bombSpawn;

	void Awake() {
		// Screen.SetResolution (1280, 960, false);
		if (instance == null) {
			DontDestroyOnLoad (gameObject);
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		dirs = new Vector3[4];
		dirs [0] = Vector3.right;
		dirs [1] = Vector3.left;
		dirs [2] = Vector3.up;
		dirs [3] = Vector3.down;

		bombSpawn = ((GameObject)Instantiate (NetworkPrefab ("BombSpawn"))).GetComponent<BombSpawn> ();
		NetworkServer.Spawn (bombSpawn.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public GameObject NetworkPrefab(string name) {
		foreach (GameObject obj in networkManager.spawnPrefabs) {
			if (name == obj.name) {
				return obj;
			}
		}

		return null;
	}

	public void SpawnItem(Vector3 pos) {
		if (Random.value < dropRate) {
			string item = items [Random.Range (0, items.Length)];
			GameObject itemObj = (GameObject)Instantiate (NetworkPrefab (item), pos, Quaternion.identity);
			if (isServer) {
				NetworkServer.Spawn (itemObj);
			}
		}
	}
}
