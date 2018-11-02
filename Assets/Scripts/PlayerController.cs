using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour {
	/*
		 sorting order
		 0 - Ground Tiles
		 1 - Anything Above Ground Tiles
		 2 - Player, Above anything on the ground, but below walls
		 3 - Walls
		 4 - Player, Above Walls
	*/

	public float speed;
	public bool isMoving = false;
	public Vector3 target;
	public Camera mainCamera;
	public SpriteRenderer spriteRenderer;
	public List<Sprite> directionalCharacterSprites;

	public Transform groundTiles;
	public Transform wallTiles;
	public Transform ladderTilesUp;
	public Transform holeTiles;
	public Transform waterEdgeTiles;
	
	public Transform layers;
	public Transform currentLayer;

	//used for player rendering. check againts wall tile name
	string[] vecticalWallIds = new string[] {"1286"};
	string[] horizontalWallIds = new string[] {"1284"};

	private void Start() {
		currentLayer = transform.parent;
		layers = currentLayer.parent;
		SetTileRefrences(currentLayer);
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
				Transform newLayer = layers.GetChild(currentLayer.GetSiblingIndex() -1);
				newLayer.gameObject.SetActive(true);
				transform.SetParent(newLayer);
				transform.position = ladderUp.position + Vector3.left;
				SetTileRefrences(newLayer);
				currentLayer = newLayer;
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

		if(hole){
			if(spriteRenderer.sortingOrder == 2)
				spriteRenderer.sortingOrder = 4;
			Transform newLayer = layers.GetChild(currentLayer.GetSiblingIndex() +1);
			transform.SetParent(newLayer);
			currentLayer.gameObject.SetActive(false); // current one is now old
			transform.position = hole.position + Vector3.right + Vector3.down;
			SetTileRefrences(newLayer);
			currentLayer = newLayer;
		}else if(ground && !wall && !waterEdge){
			SetSortingLayer(nextPos);
			SetFloorRender(nextPos);
			target = nextPos;
			isMoving = true; 
		}
	}

	public Transform GetTile(Vector3 pos, Transform tiles){
		foreach(Transform tile in tiles){
			if(tile.tag == "Holder"){
				foreach(Transform tile1 in tile){
					if(tile1.position == pos)
						return tile1;
				}
			}else if(tile.position == pos)
				return tile;
		}
		return null;
	}

	//check bounds for right clicks
	public Transform GetTile(Vector2 mousePos, Transform tiles){
		foreach(Transform tile in tiles){
			SpriteRenderer tileSprite = tile.GetComponent<SpriteRenderer>();
			if(tileSprite.bounds.Contains(mousePos))
				return tile;
		}
		return null;
	}

	//index position within floor GO
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

		//sorting order should only be set to 1 if the wall is a horizontal one

		if(wallDown || ladderDown){
			spriteRenderer.sortingOrder = 2;
		}

		if(wallTopLeft || wallUp || ladderAt){
			spriteRenderer.sortingOrder = 4;
		}

		if(wallRight){
			bool horizontal = WallType(wallRight.name, horizontalWallIds);
			if(horizontal)
				spriteRenderer.sortingOrder = 2;
		}

		if(wallLeft){
			bool vectical = WallType(wallLeft.name, vecticalWallIds);
			if(vectical)
				spriteRenderer.sortingOrder = 4;			
		}

	}

	//check a tile name if it contains id
	public bool WallType(string tileName, string[] ids){
		foreach(string id in ids){
			if(tileName.Contains(id))
				return true;
		}
		return false;
	}

	//does this for all floors above the floor the current player is at
	public void SetFloorRender(Vector3 nextPos){
		int index = currentLayer.GetSiblingIndex() -1;	
		if(index < 0) // out of bounds
			return;
		Transform floorAbove = layers.GetChild(index);
		Transform groundTiles = floorAbove.Find("GroundTiles");
		Transform tileRight = GetTile(nextPos + Vector3.left, groundTiles);

		if(tileRight)
			floorAbove.gameObject.SetActive(false);
		else
			floorAbove.gameObject.SetActive(true);
	}
}
