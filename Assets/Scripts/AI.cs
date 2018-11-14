using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class AI : MonoBehaviour {

	public Transform player;

    public float nextMove = 0;

	public float speed;
	public bool isMoving = false;
	public Vector3 targetPos;

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
	}

	void Update () {
		if(!isMoving && nextMove > 1f){
			if(player.position.x < transform.position.x)
				SetTarget(Vector3.left);
			if(player.position.y < transform.position.y)
				SetTarget(Vector3.down);
			if(player.position.x > transform.position.x)
				SetTarget(Vector3.right);
			if(player.position.y > transform.position.y)
				SetTarget(Vector3.up);

			nextMove = 0;
		}

		nextMove += UnityEngine.Time.deltaTime;

		if(isMoving){
			float step = speed * Time.deltaTime;
			float distance = Vector3.Distance(transform.position, targetPos);
			transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

			if(transform.position == targetPos)
				isMoving = false;
		}
	}	

	public void SetTarget(Vector3 direction){
		targetPos = transform.position + direction;
		if(targetPos != player.transform.position)
			isMoving = true;
	}

	public void SetTileRefrences(Transform floor){
		groundTiles = floor.Find("GroundTiles");
		wallTiles = floor.Find("WallTiles");
		ladderTilesUp = floor.Find("LadderTiles");
		holeTiles = floor.Find("HoleTiles");
		waterEdgeTiles = floor.Find("WaterEdgeTiles");
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
}
	