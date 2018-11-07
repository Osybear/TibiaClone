using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class PlayerController : MonoBehaviour {

	public float speed;
	public bool isMoving = false;
	public Vector3 targetPos;

	public new Camera camera;
	public new SpriteRenderer renderer;
	public Animator animator;
	
	public Transform floors;
	public Transform currentFloor;

	public Transform groundTiles;
	public Transform wallTiles;
	public Transform ladderTilesUp;
	public Transform holeTiles;
	public Transform waterEdgeTiles;
	public Transform rampTiles;

	private void Start() {
		currentFloor = transform.parent;
		floors = currentFloor.parent;
		SetTileRefrences(currentFloor);
		SetFloorRender(transform.position);
	}

	void Update () {
		if(!isMoving){
			if(Input.GetKeyDown(KeyCode.W))
				MoveToTile(Vector3.up);
			if(Input.GetKeyDown(KeyCode.A))
				MoveToTile(Vector3.left);
			if(Input.GetKeyDown(KeyCode.S))
				MoveToTile(Vector3.down);
			if(Input.GetKeyDown(KeyCode.D))
				MoveToTile(Vector3.right);
		}

		if(isMoving){
			float step = speed * Time.deltaTime;
			transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

			float distance = Vector3.Distance(transform.position, targetPos);
			animator.SetFloat("distance", distance);
			if(transform.position == targetPos){ //when the player reaches target postion
				isMoving = false;
				animator.SetFloat("distance", 0);
			}
		}

		if(Input.GetMouseButtonDown(1)){
			Vector2 mousePos = camera.ScreenToWorldPoint(Input.mousePosition);
			Transform ladderUp = GetTile(mousePos, ladderTilesUp);

			if(ladderUp && Vector3.Distance(transform.position, ladderUp.position) < 1.5f){
				if(isMoving)
					isMoving = false;
				Transform newLayer = floors.GetChild(currentFloor.GetSiblingIndex() -1);
				newLayer.gameObject.SetActive(true);
				transform.SetParent(newLayer);
				transform.position = ladderUp.position + Vector3.left;
				SetTileRefrences(newLayer);
				currentFloor = newLayer;
			}
		}
		
	}

	public void MoveToTile(Vector3 direction){
		Vector3 nextPos = transform.position + direction;

		Transform wall = GetTile(nextPos, wallTiles);
		Transform ground = GetTile(nextPos, groundTiles);
		Transform hole = GetTile(nextPos, holeTiles);
		Transform waterEdge = GetTile(nextPos, waterEdgeTiles);
		Transform ramp = GetTile(nextPos, rampTiles);

		if(hole){
			Transform newFloor = floors.GetChild(currentFloor.GetSiblingIndex() +1);
			newFloor.gameObject.SetActive(true);
			transform.SetParent(newFloor);
			currentFloor.gameObject.SetActive(false); // current one is now old
			SetTileRefrences(newFloor);
			currentFloor = newFloor;
			transform.position = hole.position + Vector3.down + Vector3.right; // set positon where the ladder tile is located. down to the right
		}else if(ground && !wall && !waterEdge){
			SetSortingLayer(nextPos);
			SetFloorRender(nextPos);
			targetPos = nextPos;
			isMoving = true;
			SetAnimation(direction);
		}
	}
	
	//Handles the animator component on the player
	public void SetAnimation(Vector3 direction){
		if(direction == Vector3.down){
			animator.SetTrigger("down");
		}
	}

	public Transform GetTile(Vector3 pos, Transform tiles){
		if(!tiles) // if refrence is null return null
			return null;

		foreach(Transform tile in tiles){
			if(tile.tag.Equals("Holder")){
				foreach(Transform tile1 in tile){
					if(tile1.position == pos)
						return tile1;
				}
			}else if(tile.position == pos)
				return tile;
		}
		return null;
	}

	public Transform GetTile(Vector2 mousePos, Transform tiles){
		if(!tiles) // if refrence is null return null
			return null;
			
		foreach(Transform tile in tiles){
			SpriteRenderer tileSprite = tile.GetComponent<SpriteRenderer>();
			if(tileSprite.bounds.Contains(mousePos))
				return tile;
		}
		return null;
	}

	public void SetTileRefrences(Transform floor){
		groundTiles = floor.Find("GroundTiles");
		wallTiles = floor.Find("WallTiles");
		ladderTilesUp = floor.Find("LadderTiles");
		holeTiles = floor.Find("HoleTiles");
		waterEdgeTiles = floor.Find("WaterEdgeTiles");
	}

	public void SetSortingLayer(Vector3 nextPos){
		Transform wallUp = GetTile(nextPos + Vector3.up, wallTiles);
		Transform wallRight = GetTile(nextPos + Vector3.right, wallTiles);
		Transform wallDown = GetTile(nextPos + Vector3.down, wallTiles);
		Transform wallLeft = GetTile(nextPos + Vector3.left, wallTiles);
		Transform wallTopLeft = GetTile(nextPos + Vector3.left + Vector3.up, wallTiles);

		Transform ladderAt = GetTile(nextPos, ladderTilesUp);
		Transform ladderDown = GetTile(nextPos + Vector3.down, ladderTilesUp);

		if(wallDown || ladderDown){
			renderer.sortingOrder = 2;
		}

		if(wallTopLeft || ladderAt){
			renderer.sortingOrder = 4;
		}

		if(wallUp){
			bool pillar = wallUp.name.Contains("1283");
			bool horizontal = wallUp.name.Contains("1286");
			if(!pillar && !horizontal)
				renderer.sortingOrder = 4;
		}

		if(wallRight){
			bool horizontal = wallRight.name.Contains("1284");
			if(horizontal)
				renderer.sortingOrder = 2;
		}

		if(wallLeft){
			bool vectical = wallLeft.name.Contains("1286");
			if(vectical)
				renderer.sortingOrder = 4;			
		}

	}

	//if there is a tile on the floor above
	//set all other floor invisible
	//when the player is "underground" no need to check Floor Render above
	public void SetFloorRender(Vector3 nextPos){	
		int index = currentFloor.GetSiblingIndex() -1; // index for the floor above 
		if(index < 0) // out of bounds
			return;
		Transform floorAbove = floors.GetChild(index);
		Transform groundTiles = floorAbove.Find("GroundTiles");
		Transform holeTiles = floorAbove.Find("HoleTiles");

		Transform groundTile = GetTile(nextPos, groundTiles);
		Transform holeTile = GetTile(nextPos, holeTiles);
		
		if(groundTile || holeTile){
			for(int i = index; i >= 0; i--) // set all the floors above deactivated
				floors.GetChild(i).gameObject.SetActive(false);	
		}
		else
			for(int i = index; i >= 0; i--) // set all the floors above deactivated
				floors.GetChild(i).gameObject.SetActive(true);	
	}
}
	