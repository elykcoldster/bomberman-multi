using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

	public GameObject parent;
	public int range;

	// Use this for initialization
	void Start () {
		Destroy (gameObject, 2f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnDestroy() {
		DestroyBricks ();
		parent.GetComponent<PlayerController> ().DecreaseBomb ();
	}

	void DestroyBricks() {
		int layerMask = ~(1 << LayerMask.NameToLayer ("Bomb"));
		float r = (float)range;
		RaycastHit2D right = Physics2D.Raycast (transform.position, Vector2.right, r / 2, layerMask);
		RaycastHit2D left = Physics2D.Raycast (transform.position, Vector2.left, r / 2, layerMask);
		RaycastHit2D up = Physics2D.Raycast (transform.position, Vector2.up, r / 2, layerMask);
		RaycastHit2D down = Physics2D.Raycast (transform.position, Vector2.down, r / 2, layerMask);

		if (right) {
			if (right.collider.tag == "Destructable") {
				right.collider.GetComponent<Destructable> ().Explode ();
			} else if (right.collider.tag == "Player") {
				right.collider.GetComponent<PlayerController> ().Die ();
			}
		}
		if (left) {
			if (left.collider.tag == "Destructable") {
				left.collider.GetComponent<Destructable> ().Explode ();
			} else if (left.collider.tag == "Player") {
				left.collider.GetComponent<PlayerController> ().Die ();
			}
		}
		if (up) {
			if (up.collider.tag == "Destructable") {
				up.collider.GetComponent<Destructable> ().Explode ();
			} else if (up.collider.tag == "Player") {
				up.collider.GetComponent<PlayerController> ().Die ();
			}
		}
		if (down) {
			if (down.collider.tag == "Destructable") {
				down.collider.GetComponent<Destructable> ().Explode ();
			} else if (down.collider.tag == "Player") {
				down.collider.GetComponent<PlayerController> ().Die ();
			}
		}
	}
}
