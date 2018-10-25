using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController1 : MonoBehaviour {

	public float speed;
	public bool isMoving = false;
	public Vector3 target;
	public Tilemap floorTileMap;
	public Tilemap wallTileMap;

	void Update () {
		if(!isMoving){
			if(Input.GetKey(KeyCode.W)){
				MoveToTile(Vector3Int.up);
			}

			if(Input.GetKey(KeyCode.A)){
				MoveToTile(Vector3Int.left);
			}

			if(Input.GetKey(KeyCode.S)){
				MoveToTile(Vector3Int.down);
			}

			if(Input.GetKey(KeyCode.D)){
				MoveToTile(Vector3Int.right);
			}

			if(Input.GetKey(KeyCode.E)){
				MoveToTile(Vector3Int.right + Vector3Int.up);
			}

			if(Input.GetKey(KeyCode.Q)){
				MoveToTile(Vector3Int.left + Vector3Int.up);
			}

			if(Input.GetKey(KeyCode.Z)){
				MoveToTile(Vector3Int.left + Vector3Int.down);
			}
		
			if(Input.GetKey(KeyCode.C)){
				MoveToTile(Vector3Int.right + Vector3Int.down);
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


	public void MoveToTile(Vector3Int direction){
		Vector3Int targetCell = floorTileMap.layoutGrid.WorldToCell(transform.position) + direction;
		TileBase tileAtCell = floorTileMap.GetTile(targetCell);
		TileBase obstacleAtCell = wallTileMap.GetTile(targetCell);

		if(!obstacleAtCell && tileAtCell){
			target = floorTileMap.layoutGrid.CellToWorld(targetCell) + floorTileMap.tileAnchor;
			isMoving = true; 
		}
	}
}
