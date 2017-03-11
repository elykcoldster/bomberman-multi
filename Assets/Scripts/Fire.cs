using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Destroy (gameObject, 1f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void FireState(string state) {
		GetComponentInChildren<SpriteRenderer> ().GetComponent<Animator> ().SetTrigger (state);
	}
}
