using UnityEngine;
using System.Collections;
using System;
public class LookAtCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		try{
		transform.LookAt(Camera.mainCamera.transform);
		}catch(Exception e){}//transform.Rotate(new Vector3(Random.Range(0, 5),0,0));
	}
}
