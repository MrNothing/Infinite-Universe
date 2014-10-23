using UnityEngine;
using System.Collections;

public class FishIA : MonoBehaviour {

	float altitude;

	// Use this for initialization
	void Start () {
		StartCoroutine("rotateFish");
		altitude = Vector3.Distance(this.transform.position, PlanetCharacterController.currentPlanet.transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.Translate (Vector3.forward*Time.deltaTime);
	}

	IEnumerator rotateFish(){
		for(;;) {
			if(Vector3.Distance(this.transform.position, PlanetCharacterController.currentPlanet.transform.position) > altitude)
				this.transform.RotateAround(this.transform.position, this.transform.right, Random.Range (-15,0));
			else
				this.transform.RotateAround(this.transform.position, this.transform.right, Random.Range (0,15));
			this.transform.RotateAround(this.transform.position, this.transform.up, Random.Range (-15,15));
			yield return new WaitForSeconds(.5f);
		}
	}
}
