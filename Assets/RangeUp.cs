using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeUp : Item {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D c) {
		base.OnTriggerEnter2D (c);
		if (c.transform.tag == "Player") {
			c.GetComponent<PlayerController> ().IncreaseRange();
		}
	}
}
