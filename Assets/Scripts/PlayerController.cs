using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour {

	public float speed;
	public bool isMoving = false;
	public Vector3 target;
	public Camera mainCamera;
	public SpriteRenderer spriteRenderer;
	public List<Sprite> directionalCharacterSprites;

	public Transform floorTiles;
	public Transform wallTiles;
	public Transform ladderTilesUp;
	public Transform ladderTilesDown;

	public Transform layers;
	public Transform currentLayer;

	private void Start() {
		currentLayer = transform.parent;
		layers = currentLayer.parent;
		SetTileRefrences(currentLayer);
		SetLayerRender(transform.position);
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
			Transform ladderUp = GetLadderUp(mousePos);

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
		Transform wall = GetWall(nextPos);
		Transform floor = GetFloor(nextPos);
		Transform ladderDown = GetLadderDown(nextPos);

		if(ladderDown){
			if(spriteRenderer.sortingOrder == 1)
				spriteRenderer.sortingOrder = 3;
			Transform newLayer = layers.GetChild(currentLayer.GetSiblingIndex() +1);
			transform.SetParent(newLayer);
			currentLayer.gameObject.SetActive(false); // current one is now old
			transform.position = ladderDown.position + Vector3.right + Vector3.down;
			SetTileRefrences(newLayer);
			currentLayer = newLayer;
		}else if(floor && !wall){
			SetSortingLayer(nextPos);
			SetLayerRender(nextPos);
			target = nextPos;
			isMoving = true; 
		}
	}

	public Transform GetWall(Vector3 nextPos){
		foreach(Transform tile in wallTiles){
			if(tile.position == nextPos)
				return tile;
		}
		return null;
	}

	public Transform GetFloor(Vector3 nextPos){
		foreach(Transform tile in floorTiles){
			if(tile.position == nextPos)
				return tile;
		}
		return null;
	}
	
	public Transform GetFloor(Vector3 pos, Transform floorTiles){
		foreach(Transform tile in floorTiles){
			if(tile.position == pos)
				return tile;
		}
		return null;
	}

	//check bounds for right click
	public Transform GetLadderUp(Vector3 mousePos){
		foreach(Transform tile in ladderTilesUp){
			SpriteRenderer tileSprite = tile.GetComponent<SpriteRenderer>();
			if(tileSprite.bounds.Contains(mousePos))
				return tile;
		}
		return null;
	}

	//check player pos
	public Transform GetLadderUp1(Vector3 nextPos){
		foreach(Transform tile in ladderTilesUp){
			if(tile.position == nextPos)
				return tile;
		}
		return null;
	}

	public Transform GetLadderDown(Vector3 nextPos){
		foreach(Transform tile in ladderTilesDown){
			if(tile.position == nextPos)
				return tile;
		}
		return null;
	}

	public void SetTileRefrences(Transform layer){
		floorTiles = layer.GetChild(0);
		wallTiles = layer.GetChild(1);
		ladderTilesUp = layer.GetChild(2);
		ladderTilesDown = layer.GetChild(3);
	}

	//set playerRenderer sorting layer
	public void SetSortingLayer(Vector3 nextPos){
		//sort order 3 for player render above
		//sort order 1 for player render below
		Transform wallUp = GetWall(nextPos + Vector3.up);
		Transform wallRight = GetWall(nextPos + Vector3.right);
		Transform wallDown = GetWall(nextPos + Vector3.down);
		Transform wallLeft = GetWall(nextPos + Vector3.left);
		
		Transform ladderAt = GetLadderUp1(nextPos);
		Transform ladderDown = GetLadderUp1(nextPos + Vector3.down);

		if(wallDown || wallRight || ladderDown)
			spriteRenderer.sortingOrder = 1;

		if(wallLeft || wallUp || ladderAt)
			spriteRenderer.sortingOrder = 3;
	}

	//renders layers based on player pos
	//set for all layers that the player is below
	//it will have to loop through all layers	
	public void SetLayerRender(Vector3 nextPos){
		int index = currentLayer.GetSiblingIndex() -1;	
		if(index < 0)
			return;
		Transform layer = layers.GetChild(index);

		Transform floorAboveUp = GetFloor(nextPos + Vector3.up, layer.GetChild(0));
		Transform floorAboveRight = GetFloor(nextPos + Vector3.right, layer.GetChild(0));
		Transform floorAboveDown = GetFloor(nextPos + Vector3.down, layer.GetChild(0));
		Transform floorAboveLeft = GetFloor(nextPos + Vector3.left, layer.GetChild(0));

		if(floorAboveUp || floorAboveRight || floorAboveDown || floorAboveLeft)
			layer.gameObject.SetActive(false);
		else
			layer.gameObject.SetActive(true);
	}
}
