using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float speed;
	public bool isMoving = false;
	public Vector3 target;
	public GameObject currentTile; // tile the player is standing on duh lol

	private void Start() {
		currentTile.layer = 2;
	}

	void Update () {
		if(!isMoving){
			if(Input.GetKey(KeyCode.W)){
				MoveToTile(Vector3.up);
			}else if(Input.GetKey(KeyCode.A)){
				MoveToTile(Vector3.left);
			}else if(Input.GetKey(KeyCode.S)){
				MoveToTile(Vector3.down);
			}else if(Input.GetKey(KeyCode.D)){
				MoveToTile(Vector3.right);
			}else if(Input.GetKey(KeyCode.Q)){
				MoveToTile(Vector3.left + Vector3.up);
			}else if(Input.GetKey(KeyCode.E)){
				MoveToTile(Vector3.right + Vector3.up);
			}else if(Input.GetKey(KeyCode.Z)){
				MoveToTile(Vector3.down + Vector3.left);
			}else if(Input.GetKey(KeyCode.C)){
				MoveToTile(Vector3.down + Vector3.right);
			}
		}

		if(isMoving){
			float step = speed * Time.deltaTime;
			transform.position = Vector3.MoveTowards(transform.position, target, step);
			if(transform.position == target){
				isMoving = false;
			}		
		}
	}

	public void MoveToTile(Vector3 direction){
		TileProperties tileProperties = GetTileProperties(direction);
		if(tileProperties && tileProperties.isWalkable){
			currentTile.layer = 0;
			currentTile = tileProperties.gameObject;
			currentTile.layer = 2;
			target = transform.position + direction;
			isMoving = true;
		}
	}

	public TileProperties GetTileProperties(Vector3 direction){
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1.25f);
        if (hit.collider != null)
        {
			return hit.transform.GetComponent<TileProperties>();
        }
		return null;
	}
}
