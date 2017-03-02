using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

	public float speed = 5.0f;
	public float animationSpeed = 2.0f;

	Animator anim;
	SpriteRenderer sr;

	void Start() {
		anim = GetComponent<Animator> ();
		sr = GetComponent<SpriteRenderer> ();
	}

	void Update()
	{
		if (!isLocalPlayer) {
			return;
		}
		Move ();
	}

	public override void OnStartLocalPlayer() {
		// GetComponent<MeshRenderer> ().material.color = Color.blue;
	}

	void Move() {
		var x = Input.GetAxis("Horizontal");
		var y = Input.GetAxis("Vertical");

		if (x != 0f || y != 0f) {
			anim.SetBool ("move", true);
			if (x != 0f) {
				anim.SetFloat ("h", x);
			} else if (y != 0f) {
				anim.SetFloat ("v", y);
			}
		} else {
			anim.SetBool ("move", false);
			anim.SetFloat ("h", 0f);
			anim.SetFloat ("v", 0f);
		}
		transform.Translate(new Vector2(x, y) * speed * Time.deltaTime);
	}
}
