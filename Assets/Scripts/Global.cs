using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Global : MonoBehaviour {

	public static Global instance;

	public NetworkManager networkManager;

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
}
