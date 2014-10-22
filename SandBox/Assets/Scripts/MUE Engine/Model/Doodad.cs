using UnityEngine;
using System.Collections;

[System.Serializable]
public class Doodad {
	//optional
	public string name;
	public GameObject gameObject;
	public float frequency = 1;
	public float minHeight = 0;
	public float maxHeight = 15;
	public float shadow = 0.5f;
	public float offsetY = 0;
	public float minScaleFactor = 0.5f;
	public float maxScaleFactor = 1.5f;
	public float maxRotation = 180;
}
