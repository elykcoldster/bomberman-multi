using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

	public float speed = 5.0f;
	public float animationSpeed = 2.0f;

	Animator anim;
	SpriteRenderer sr;
	Rigidbody2D rb;

	void Start() {
		anim = GetComponent<Animator> ();
		sr = GetComponent<SpriteRenderer> ();
		rb = GetComponent<Rigidbody2D> ();
	}

	void Update()
	{
		SpriteOrder ();
		if (!isLocalPlayer) {
			return;
		}
		Move ();
		if (Input.GetButtonDown ("Fire1")) {
			CmdBomb ();
		}
	}

	public override void OnStartLocalPlayer() {
		// GetComponent<MeshRenderer> ().material.color = Color.blue;
	}

	void Move() {
		var x = Input.GetAxis("Horizontal");
		var y = Input.GetAxis("Vertical");

		if (x != 0f || y != 0f) {
			anim.SetBool ("move", true);
			Turn (x, y);
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
		// transform.Translate(new Vector2(x, y) * speed * Time.deltaTime);
		rb.velocity = (Vector2.right * x + Vector2.up * y) * speed;
	}

	void Turn(float h, float v) {
		anim.SetBool ("right", h == 0f ? false : (h > 0f ? true : false));
		anim.SetBool ("left", h == 0f ? false : (h < 0f ? true : false));
		anim.SetBool ("up", v == 0f ? false : (v > 0f ? true : false));
		anim.SetBool ("down", v == 0f ? false : (v < 0f ? true : false));
	}
	[Command]
	void CmdBomb() {
		GameObject bomb = Instantiate (Global.instance.NetworkPrefab ("bomb"), transform.position, Quaternion.identity);
		NetworkServer.Spawn (bomb);
	}

	void SpriteOrder() {
		sr.sortingOrder = -Mathf.RoundToInt (transform.position.y * 100f);
	}
}
