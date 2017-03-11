using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {

	public float speed = 5f;

	Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
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
