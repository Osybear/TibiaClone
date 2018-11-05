using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour {
	public float speed;
	public bool isMoving = false;
	public bool isUnderground = false; // if underground then dont check render floor above
	public Vector3 target;

	public Camera mainCamera;
	public SpriteRenderer spriteRenderer;
	public List<Sprite> directionalCharacterSprites;

	public Transform groundTiles;
	public Transform wallTiles;
	public Transform ladderTilesUp;
	public Transform holeTiles;
	public Transform waterEdgeTiles;
	public Transform rampTiles;

	public Transform floors;
	public Transform currentFloor;

	private void Start() {
		currentFloor = transform.parent;
		floors = currentFloor.parent;
		SetTileRefrences(currentFloor);
		SetFloorRender(transform.position);
	}

	void Update () {
		if(!isMoving){
			if(Input.GetKey(KeyCode.W)){
				MoveToTile(Vector3.up);
			}

			if(Input.GetKey(KeyCode.A)){
				MoveToTile(Vector3.left);
			}

			if(Input.GetKey(KeyCode.S)){
				MoveToTile(Vector3.down);
			}

			if(Input.GetKey(KeyCode.D)){
				MoveToTile(Vector3.right);
			}

			if(Input.GetKey(KeyCode.E)){
				MoveToTile(Vector3.right + Vector3.up);
			}

			if(Input.GetKey(KeyCode.Q)){
				MoveToTile(Vector3.left + Vector3.up);
			}

			if(Input.GetKey(KeyCode.Z)){
				MoveToTile(Vector3.left + Vector3.down);
			}
		
			if(Input.GetKey(KeyCode.C)){
				MoveToTile(Vector3.right + Vector3.down);
			}
		}

		if(Input.GetMouseButtonDown(1)){
			Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
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

		if(isMoving){
			float step = speed * Time.deltaTime;
			transform.position = Vector3.MoveTowards(transform.position, target, step);
			if(transform.position == target){
				isMoving = false;
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
			transform.position = hole.position;
			isUnderground = true;
		}else if(ground && !wall && !waterEdge){
			SetSortingLayer(nextPos);
			SetFloorRender(nextPos);
			target = nextPos;
			isMoving = true; 
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
		ladderTilesUp = floor.Find("LadderTilesUp");
		holeTiles = floor.Find("Holes");
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
			spriteRenderer.sortingOrder = 2;
		}

		if(wallTopLeft || ladderAt){
			spriteRenderer.sortingOrder = 4;
		}

		if(wallUp){
			bool pillar = wallUp.name.Contains("1283");
			bool horizontal = wallUp.name.Contains("1286");
			if(!pillar && !horizontal)
				spriteRenderer.sortingOrder = 4;
		}

		if(wallRight){
			bool horizontal = wallRight.name.Contains("1284");
			if(horizontal)
				spriteRenderer.sortingOrder = 2;
		}

		if(wallLeft){
			bool vectical = wallLeft.name.Contains("1286");
			if(vectical)
				spriteRenderer.sortingOrder = 4;			
		}

	}

	//if there is a tile on the floor above
	//set all other floor invisible
	//when the player is "underground" no need to check Floor Render above
	public void SetFloorRender(Vector3 nextPos){
		if(isUnderground)
			return;

		int index = currentFloor.GetSiblingIndex() -1; // index for the floor above 
		if(index < 0) // out of bounds
			return;
		Transform floorAbove = floors.GetChild(index);
		Transform groundTiles = floorAbove.Find("GroundTiles");
		Transform tile = GetTile(nextPos, groundTiles);
		
		if(tile){
			for(int i = index; i >= 0; i--) // set all the floors above deactivated
				floors.GetChild(i).gameObject.SetActive(false);	
		}
		else
			for(int i = index; i >= 0; i--) // set all the floors above deactivated
				floors.GetChild(i).gameObject.SetActive(true);	
	}
}
	