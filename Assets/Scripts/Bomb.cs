using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bomb : NetworkBehaviour {

	static float unit = 0.5f;

	SpriteRenderer sr;

	[SyncVar]
	public GameObject parent;
	public int range;

	// Use this for initialization
	void Start () {
		Destroy (gameObject, 2f);
		sr = GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnDestroy() {
		// SpawnFire ();
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
		GameObject fire = (GameObject)Instantiate (Global.instance.NetworkPrefab("fire"), transform.position, Quaternion.identity);

		int layerMask = ~(1 << LayerMask.NameToLayer ("Bomb"));
		float r = (float)range;
		RaycastHit2D right = Physics2D.Raycast (transform.position, Vector2.right, r / 2 + sr.bounds.size.x / 2, layerMask);
		RaycastHit2D left = Physics2D.Raycast (transform.position, Vector2.left, r / 2 + sr.bounds.size.x / 2, layerMask);
		RaycastHit2D up = Physics2D.Raycast (transform.position, Vector2.up, r / 2 + sr.bounds.size.y / 2, layerMask);
		RaycastHit2D down = Physics2D.Raycast (transform.position, Vector2.down, r / 2 + sr.bounds.size.y / 2, layerMask);

		ExplodeDirection (right.collider, Vector2.right);
		ExplodeDirection (left.collider, Vector2.left);
		ExplodeDirection (up.collider, Vector2.up);
		ExplodeDirection (down.collider, Vector2.down);
	}

	void ExplodeDirection(Collider2D c, Vector2 dir) {
		DrawFlames (c, dir);
		if (c) {
			if (c.tag == "Destructable") {
				c.GetComponent<Destructable> ().Explode ();
			} else if (c.tag == "Player") {
				c.GetComponent<PlayerController> ().RpcDie ();
			}
		}
	}

	void DrawFlames(Collider2D c, Vector2 dir) {
		if (c) {
			int dist = Mathf.RoundToInt ((c.transform.position - transform.position).magnitude / unit);
			GameObject fire = (GameObject)Instantiate (Global.instance.NetworkPrefab ("fire"), transform.position + dist * unit * new Vector3 (dir.x, dir.y, 0f), Quaternion.identity);
			NetworkServer.Spawn (fire);

			string state = dir.x == 0f ? (dir.y > 0f ? "up" : "down") : (dir.x > 0f ? "right" : "left");
			fire.GetComponent<Fire> ().FireState (state);
		} else {
			for (int i = 0; i < range; i++) {
				if (i == range - 1) {
					GameObject fire = (GameObject)Instantiate (Global.instance.NetworkPrefab ("fire"), transform.position + (i + 1) * unit * new Vector3 (dir.x, dir.y, 0f), Quaternion.identity);
					NetworkServer.Spawn (fire);

					string state = dir.x == 0f ? (dir.y > 0f ? "up" : "down") : (dir.x > 0f ? "right" : "left");
					fire.GetComponent<Fire> ().FireState (state);
				}
			}
		}
	}
}
