using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

	[SyncVar]
	public float speed = 5.0f, speedMult = 1.0f;

	Animator anim;
	SpriteRenderer sr;
	Rigidbody2D rb;

	[SyncVar]
	public int maxBombs, numBombs, maxRange;
	[SyncVar]
	public Vector3 spawnPoint;
	bool dead;

	void Start() {
		anim = GetComponent<Animator> ();
		sr = GetComponent<SpriteRenderer> ();
		rb = GetComponent<Rigidbody2D> ();

		maxBombs = 1;
		maxRange = 1;
		numBombs = 0;
		dead = false;
	}

	void Update()
	{
		SpriteOrder ();
		if (!isLocalPlayer) {
			return;
		}
		if (dead) {
			return;
		}
		Move ();
		if (Input.GetButtonDown ("Fire1")) {
			CmdBomb ();
		}
	}

	public override void OnStartLocalPlayer() {
		CmdSetSpawn ();
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
		rb.velocity = (Vector2.right * x + Vector2.up * y) * speed * speedMult;
		anim.speed = speedMult;
	}

	void Turn(float h, float v) {
		anim.SetBool ("right", h == 0f ? false : (h > 0f ? true : false));
		anim.SetBool ("left", h == 0f ? false : (h < 0f ? true : false));
		anim.SetBool ("up", v == 0f ? false : (v > 0f ? true : false));
		anim.SetBool ("down", v == 0f ? false : (v < 0f ? true : false));
	}

	void SpriteOrder() {
		sr.sortingOrder = -Mathf.RoundToInt (transform.position.y * 100f);
	}

	[Command]
	void CmdSetSpawn() {
		spawnPoint = transform.position;
	}

	[Command]
	void CmdBomb() {
		float yoff = GetComponent<BoxCollider2D> ().offset.y;
		if (numBombs < maxBombs) {
			Global.instance.bombSpawn.SpawnBomb (gameObject, maxRange, yoff);
			numBombs++;
		}
	}

	[Command]
	void CmdIncreaseRange() {
		maxRange++;
	}

	[Command]
	void CmdIncreaseBombs() {
		maxBombs++;
	}

	[Command]
	void CmdIncreaseSpeed() {
		speedMult += 0.25f;
	}

	[Command]
	void CmdDie() {
		rb.velocity = Vector2.zero;
		anim.SetTrigger ("dead");
		dead = true;
		StartCoroutine (RespawnInSeconds(2f));	
	}

	[ClientRpc]
	void RpcDie() {
		rb.velocity = Vector2.zero;
		anim.SetTrigger ("dead");
		dead = true;
		StartCoroutine (RespawnInSeconds(2f));	
	}

	public void DecreaseBomb() {
		numBombs--;
	}

	public void Die() {
		if (isServer) {
			RpcDie ();
		} else {
			CmdDie ();
		}
	}

	public void IncreaseSpeed() {
		if (isServer) {
			CmdIncreaseSpeed ();
		}
	}

	public void IncreaseRange() {
		if (isServer) {
			CmdIncreaseRange ();
		}
	}

	public void IncreaseBombs() {
		if (isServer) {
			CmdIncreaseBombs ();
		}
	}
		
	IEnumerator RespawnInSeconds(float t) {
		yield return new WaitForSeconds (t);
		dead = false;
		anim.SetTrigger ("reset");
		rb.position = spawnPoint;
	}
}
