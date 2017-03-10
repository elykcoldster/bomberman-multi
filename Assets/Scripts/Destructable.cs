using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour {

	Collider2D[] colliders;
	SpriteRenderer sr;
	Animator anim;

	// Use this for initialization
	void Start () {
		colliders = GetComponents<Collider2D> ();
		sr = GetComponent<SpriteRenderer> ();
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetVisible(int v) {
		sr.enabled = v == 1 ? true : false;
		foreach (Collider2D c in colliders) {
			c.enabled = v == 1 ? true : false;
		}
	}

	public void Explode() {
		anim.SetTrigger ("explode");
	}

	public void Reset() {
		anim.SetTrigger ("reset");
		SetVisible (1);
	}
}
