using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bomb : NetworkBehaviour {
	SpriteRenderer sr;

	[SyncVar]
	public GameObject parent;
	public int range;

	Collider2D[] dirCols;

	// Use this for initialization
	void Start () {
		Destroy (gameObject, 2f);
		sr = GetComponent<SpriteRenderer> ();

		dirCols = new Collider2D [4];
	}

	void OnTriggerExit2D(Collider2D c) {
		if (c.transform == parent.transform) {
			GetComponent<Collider2D> ().isTrigger = false;
		}
	}

	void OnDestroy() {
		// SpawnFire ();
		parent.GetComponent<PlayerController>().BombAudio();
		Explode ();
		parent.GetComponent<PlayerController> ().DecreaseBomb ();
	}

	void SpawnFire() {
		for (int i = 0; i < 4; i++) {
			float x = 0.5f * Mathf.Round (transform.position.x / 0.5f) + 0.5f * (i % 2 == 0 ? 0 : (i < 2 ? 1 : -1));
			float y = 0.5f * Mathf.Round (transform.position.y / 0.5f) + 0.5f * (i % 2 == 0 ? (i < 2 ? 1 : -1) : 0);
			GameObject bullet = (GameObject)Instantiate (Global.instance.NetworkPrefab ("bullet"), new Vector2 (x, y), Quaternion.identity);
			NetworkServer.Spawn (bullet);
			bullet.GetComponent<Bullet> ().RpcSetDirection (new Vector2 ((i % 2 == 0 ? 0 : (i < 2 ? 1 : -1)), (i % 2 == 0 ? (i < 2 ? 1 : -1) : 0)));
		}
	}

	void Explode() {
		int layerMask = ~(1 << LayerMask.NameToLayer ("Ignore"));
		gameObject.layer = LayerMask.NameToLayer ("Ignore");
		float r = (float)range;
		RaycastHit2D right = Physics2D.Raycast (transform.position, Vector2.right, r / 2 + sr.bounds.size.x / 2, layerMask);
		RaycastHit2D left = Physics2D.Raycast (transform.position, Vector2.left, r / 2 + sr.bounds.size.x / 2, layerMask);
		RaycastHit2D up = Physics2D.Raycast (transform.position, Vector2.up, r / 2 + sr.bounds.size.y / 2, layerMask);
		RaycastHit2D down = Physics2D.Raycast (transform.position, Vector2.down, r / 2 + sr.bounds.size.y / 2, layerMask);

		dirCols [0] = right.collider;
		dirCols [1] = left.collider;
		dirCols [2] = up.collider;
		dirCols [3] = down.collider;

		ExplodeDirection ();

		Global.instance.bombSpawn.DrawFlames (transform.position, range);
	}

	void ExplodeDirection() {
		foreach (Collider2D c in dirCols) {
			if (c) {
				if (c.tag == "Destructable") {
					c.GetComponent<Destructable> ().Explode ();
				} else if (c.tag == "Player") {
					c.GetComponent<PlayerController> ().Die ();
				}
			}
		}
	}

	void RpcDrawFlames() {
		for (int n = 0 ; n < dirCols.Length; n++) {
			Collider2D c = dirCols [n];
			float x = Global.dirs [n].x, y = Global.dirs [n].y;

			if (c) {
				int dist = Mathf.RoundToInt ((c.transform.position - transform.position).magnitude / Global.unit);
				GameObject fire = (GameObject)Instantiate (Global.instance.NetworkPrefab ("fire"), transform.position + dist * Global.unit * new Vector3 (x, y, 0f), Quaternion.identity);
				NetworkServer.Spawn (fire);

				string state = x == 0f ? (y > 0f ? "up" : "down") : (x > 0f ? "right" : "left");
				fire.GetComponent<Fire> ().FireState (state);
			} else {
				for (int i = 0; i < range; i++) {
					if (i == range - 1) {
						GameObject fire = (GameObject)Instantiate (Global.instance.NetworkPrefab ("fire"), transform.position + (i + 1) * Global.unit * new Vector3 (x, y, 0f), Quaternion.identity);
						NetworkServer.Spawn (fire);

						string state = x == 0f ? (y > 0f ? "up" : "down") : (x > 0f ? "right" : "left");
						fire.GetComponent<Fire> ().FireState (state);
					}
				}
			}
		}
	}
}
