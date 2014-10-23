using UnityEngine;
using System.Collections;

public class DebugOutput : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void OnGUI () {
		if(GlobalCore.loadingQueue>0)
		{
			GUI.Label(new Rect(0, 0, 200, 20), "Loading queue: "+GlobalCore.loadingQueue);
		}
	}
}
