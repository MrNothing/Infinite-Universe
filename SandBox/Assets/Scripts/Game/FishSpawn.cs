using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FishSpawn : MonoBehaviour {

	private float altitude;
	
	// Use this for initialization
	void Start () 
	{
		if (this.transform != null && PlanetCharacterController.currentPlanet != null){
			altitude = Vector3.Distance(this.transform.position, PlanetCharacterController.currentPlanet.transform.position);	
			StartCoroutine ("RotateFishes");
		}
		else
		{
			StartCoroutine("InitializeFishes");
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		this.transform.Translate (Vector3.forward*Time.deltaTime);
	}

	IEnumerator InitializeFishes()
	{
		if (this.transform != null && PlanetCharacterController.currentPlanet != null){
			altitude = Vector3.Distance(this.transform.position, PlanetCharacterController.currentPlanet.transform.position);	
			StartCoroutine ("RotateFishes");
			yield break;
		}
		else{
			yield return new WaitForSeconds (5);
			StartCoroutine("InitializeFishes");
		}
	}

	IEnumerator RotateFishes()
	{
		while (true)
			{
			if(Vector3.Distance(this.transform.position, PlanetCharacterController.currentPlanet.transform.position) > altitude)
			{
				this.transform.RotateAround(this.transform.position, this.transform.right, 0.5f);
			}
			else
			{
				this.transform.RotateAround(this.transform.position, this.transform.right, -0.5f);
			}
			this.transform.RotateAround(this.transform.position, this.transform.up, Random.Range (-0.5f,0.5f));
			yield return new WaitForSeconds(.1f);

		}
	}
}
