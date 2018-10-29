using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSortOrder : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Renderer>().sortingOrder = -(int)Mathf.Floor(transform.position.y);	
	}
	
}
