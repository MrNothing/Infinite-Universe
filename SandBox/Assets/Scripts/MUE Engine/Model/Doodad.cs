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
	public Color vertex = new Color(0.5f, 0, 0, 0);
	public float offsetY = 0;
}
