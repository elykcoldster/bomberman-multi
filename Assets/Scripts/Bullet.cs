using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {

	public float speed = 5f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D c) {
		if (c.transform.tag == "Wall") {
			print (c.name);
			// Destroy (gameObject);
		}
	}

	[ClientRpc]
	public void RpcSetDirection(Vector2 dir) {
		GetComponent<Rigidbody2D> ().velocity = dir.normalized * speed;
	}
}
