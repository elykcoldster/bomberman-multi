using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BombSpawn : NetworkBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SpawnBomb(GameObject parent, int range, float yoffset) {
		GameObject bomb = (GameObject)Instantiate(Global.instance.NetworkPrefab ("bomb"), parent.transform.position + Vector3.up * yoffset, Quaternion.identity);
		bomb.GetComponent<Bomb> ().parent = parent;
		bomb.GetComponent<Bomb> ().range = range;
		NetworkServer.Spawn (bomb);
	}

	/*public void DrawFlames(Vector3 pos, int range) {
		if (isServer) {
			RpcDrawFlames (pos, range);
		} else {
			CmdDrawFlames (pos, range);
		}
	}*/

	[Server]
	public void DrawFlames(Vector3 pos, int range) {
		GameObject fireBase = (GameObject)Instantiate (Global.instance.NetworkPrefab ("fire_base"), pos, Quaternion.identity);
		NetworkServer.Spawn (fireBase);
		bool[] collide = new bool[4];
		int layerMask = ~(1 << LayerMask.NameToLayer ("Fire"));

		for (int n = 0 ; n < 4; n++) {
			for (int i = 0; i < range; i++) {
				if (collide [n]) {
					break;
				}
				float x = GetDir (n).x, y = GetDir (n).y;
				string state = (i < range - 1) ? (x == 0f ? "fire_v" : "fire_h") : (x == 0f ? (y > 0f ? "fire_up" : "fire_down") : (x > 0f ? "fire_right" : "fire_left"));

				Vector3 shiftVec = (i + 1) * Global.unit * GetDir (n);
				Vector2 checkVec = new Vector2 (shiftVec.x, shiftVec.y);

				RaycastHit2D hit = Physics2D.Raycast (new Vector2(pos.x, pos.y), checkVec, checkVec.magnitude, layerMask);
				if (!hit) {
					GameObject fire = (GameObject)Instantiate (Global.instance.NetworkPrefab (state), pos + shiftVec, Quaternion.identity);
					NetworkServer.Spawn (fire);
				} else if (hit.collider.tag != "Wall") {
					collide [n] = true;
					GameObject fire = (GameObject)Instantiate (Global.instance.NetworkPrefab (state), pos + shiftVec, Quaternion.identity);
					NetworkServer.Spawn (fire);
				} else {
					collide [n] = true;
				}
			}
		}
	}

	[ClientRpc]
	void RpcDrawFlames(Vector3 pos, int range) {
		for (int n = 0 ; n < 4; n++) {
			float x = GetDir (n).x, y = GetDir (n).y;

			GameObject fire = (GameObject)Instantiate (Global.instance.NetworkPrefab ("fire"), pos + range * Global.unit * GetDir(n), Quaternion.identity);
			NetworkServer.Spawn (fire);

			string state = x == 0f ? (y > 0f ? "up" : "down") : (x > 0f ? "right" : "left");
			fire.GetComponent<Animator> ().SetTrigger (state);
		}
	}

	[Command]
	void CmdDrawFlames(Vector3 pos, int range) {
		for (int n = 0 ; n < 4; n++) {
			float x = GetDir (n).x, y = GetDir (n).y;

			GameObject fire = (GameObject)Instantiate (Global.instance.NetworkPrefab ("fire"), pos + range * Global.unit * GetDir(n), Quaternion.identity);
			NetworkServer.Spawn (fire);

			string state = x == 0f ? (y > 0f ? "up" : "down") : (x > 0f ? "right" : "left");
			fire.GetComponent<Animator> ().SetTrigger (state);
		}
	}

	public Vector3 GetDir(int n) {
		switch (n) {
		case 0:
			return Global.instance.right;
		case 1:
			return Global.instance.left;
		case 2:
			return Global.instance.up;
		case 3:
			return Global.instance.down;
		default:
			return Vector3.zero;
		}
	}
}
