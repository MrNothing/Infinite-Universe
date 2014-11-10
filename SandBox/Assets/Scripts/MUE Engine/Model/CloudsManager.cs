using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CloudsManager : MonoBehaviour {

	public bool optimized = false;
	public GameObject cloudModel;
	public GameObject optimizedCloudModel;
	Vector3 lastPosition = Vector3.zero;
	public float refreshDistance = 100;
	public int avgCloudsCount = 5;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (GlobalCore.currentPlanet != null) 
		{
			if(Vector3.Distance(GlobalCore.mainController.transform.position, lastPosition)>refreshDistance*2)
			{
				GenerateClouds();
				lastPosition = GlobalCore.mainController.transform.position;
			}
		}
	}

	List<MrNothingParticles> clouds = new List<MrNothingParticles>();
	List<MrNothingOptimizedParticles> cloudsOptimized = new List<MrNothingOptimizedParticles>();
	void GenerateClouds()
	{
		foreach(MrNothingParticles p in clouds)
		{
			clouds.Remove(p);
			Destroy(p.gameObject);
		}

		foreach(MrNothingOptimizedParticles p in cloudsOptimized)
		{
			cloudsOptimized.Remove(p);
			Destroy(p.gameObject);
		}

		int amount = Random.Range (avgCloudsCount / 2, avgCloudsCount+avgCloudsCount / 2);
		for(int i = 0; i<amount; i++)
		{
			Vector3 randomPos = GlobalCore.mainController.transform.position+new Vector3(Random.Range(-refreshDistance, refreshDistance), Random.Range(-refreshDistance, refreshDistance), Random.Range(-refreshDistance, refreshDistance));

			Vector3 coherentPos = GlobalCore.currentPlanet.transform.position+GlobalCore.currentPlanet.getFragment(randomPos).point+GlobalCore.currentPlanet.getNormalFromPoint(randomPos)*Random.Range(55f, 60f);

			GameObject go;

			if(!optimized)
			{
				go = (GameObject)Instantiate(cloudModel, coherentPos, Quaternion.identity);
				MrNothingParticles cloud = go.GetComponent<MrNothingParticles>();
				cloud.grid = new Vector3(Random.Range(5, 12), Random.Range(2, 5), Random.Range(5, 12));

				clouds.Add(cloud);
			}
			else
			{
				go = (GameObject)Instantiate(optimizedCloudModel, coherentPos, Quaternion.identity);
				MrNothingOptimizedParticles cloud = go.GetComponent<MrNothingOptimizedParticles>();
				cloud.grid = new Vector3(Random.Range(5, 10), Random.Range(2, 5), Random.Range(5, 10));
				
				cloudsOptimized.Add(cloud);
			}
		}
	}
}
