using UnityEngine;
using System.Collections;

public class Lights : MonoBehaviour {

	public GameObject LightGroup;

	// Use this for initialization
	void OnMouseDown () {
		if (LightGroup.activeInHierarchy){
			LightGroup.SetActive(false);
		}
		else
		{LightGroup.SetActive(true);}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
