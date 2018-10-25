using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Note : ScriptableObject {

	[TextArea(20,20)]
	public string note;
}
