using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRender : MonoBehaviour {

	public Transform invisibleHolder;
	public Transform iniParent;
	public bool activated = true;
	public int index;

	private void Awake() {
		index = transform.GetSiblingIndex();
	}

	private void Start() {
		invisibleHolder = transform.parent.parent.parent.GetChild(0);
		iniParent = transform.parent;
		transform.SetParent(invisibleHolder);
	}

	
	//OnBecameInvisible gets called when the applications quits
	//and throws an error for all of these scripts
	private void OnApplicationQuit() {
		activated = false;
	}

	//set the parent to a holder with no sorting group so max is never hit
	private void OnBecameInvisible() {
		if(activated){
			transform.SetParent(invisibleHolder);
		}
	}
	
	private void OnBecameVisible() {
		transform.SetParent(iniParent);
	}
}
