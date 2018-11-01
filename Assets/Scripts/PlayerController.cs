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

	public Transform groundTiles;
	public Transform wallTiles;
	public Transform ladderTilesUp;
	public Transform ladderTilesDown;
	public Transform waterEdgeTiles;
	
	public Transform layers;
	public Transform currentLayer;

	private void Start() {
		currentLayer = transform.parent;
		layers = currentLayer.parent;
		SetTileRefrences(currentLayer);
		//SetLayerRender(transform.position);
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
			Transform ladderUp = GetTileMouse(mousePos, ladderTilesUp);

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
		Transform ladderDown = GetTile(nextPos, ladderTilesDown);
		Transform waterEdge = GetTile(nextPos, waterEdgeTiles);

		if(ladderDown){
			if(spriteRenderer.sortingOrder == 1)
				spriteRenderer.sortingOrder = 3;
			Transform newLayer = layers.GetChild(currentLayer.GetSiblingIndex() +1);
			transform.SetParent(newLayer);
			currentLayer.gameObject.SetActive(false); // current one is now old
			transform.position = ladderDown.position + Vector3.right + Vector3.down;
			SetTileRefrences(newLayer);
			currentLayer = newLayer;
		}else if(ground && !wall && !waterEdge){
			SetSortingLayer(nextPos);
			//SetLayerRender(nextPos);
			target = nextPos;
			isMoving = true; 
		}
	}

	public Transform GetTile(Vector3 nextPos, Transform tiles){
		foreach(Transform tile in tiles){
			if(tile.position == nextPos)
				return tile;
		}
		return null;
	}

	//check bounds for right click
	public Transform GetTileMouse(Vector3 mousePos, Transform tiles){
		foreach(Transform tile in tiles){
			SpriteRenderer tileSprite = tile.GetComponent<SpriteRenderer>();
			if(tileSprite.bounds.Contains(mousePos))
				return tile;
		}
		return null;
	}

	//index position within floor GO
	public void SetTileRefrences(Transform floor){
		groundTiles = floor.GetChild(0);
		wallTiles = floor.GetChild(1);
		ladderTilesUp = floor.GetChild(2);
		ladderTilesDown = floor.GetChild(3);
		waterEdgeTiles = floor.GetChild(4);
	}

	//set playerRenderer sorting floor
	public void SetSortingLayer(Vector3 nextPos){
		//sort order 3 for player render above
		//sort order 1 for player render below
		Transform wallUp = GetTile(nextPos + Vector3.up, wallTiles);
		Transform wallRight = GetTile(nextPos + Vector3.right, wallTiles);
		Transform wallDown = GetTile(nextPos + Vector3.down, wallTiles);
		Transform wallLeft = GetTile(nextPos + Vector3.left, wallTiles);
		
		Transform ladderAt = GetTile(nextPos, ladderTilesUp);
		Transform ladderDown = GetTile(nextPos + Vector3.down, ladderTilesUp);

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
		Transform floor = layers.GetChild(index);

		Transform floorAboveUp = GetTile(nextPos + Vector3.up, floor.GetChild(0));
		Transform floorAboveRight = GetTile(nextPos + Vector3.right, floor.GetChild(0));
		Transform floorAboveDown = GetTile(nextPos + Vector3.down, floor.GetChild(0));
		Transform floorAboveLeft = GetTile(nextPos + Vector3.left, floor.GetChild(0));

		if(floorAboveUp || floorAboveRight || floorAboveDown || floorAboveLeft)
			floor.gameObject.SetActive(false);
		else
			floor.gameObject.SetActive(true);
	}
}
