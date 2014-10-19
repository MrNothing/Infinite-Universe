using UnityEngine;
using System.Collections;

public class SolarSystem : MonoBehaviour {

	public int maxPlanets = 10;
	public PerlinConfig noiseInfos;
	public int avaliablePlanets = 3; 
	// Use this for initialization
	void Start () {
		int currentPlanetCounter = (int)(Mathf.PerlinNoise(noiseInfos.offset.x, noiseInfos.offset.y)*avaliablePlanets);

		//max 10 panets
		for(float randomess = 0; randomess<maxPlanets; randomess++)
		{
			float distanceToSun = Mathf.PerlinNoise(randomess*noiseInfos.scale+noiseInfos.offset.x, randomess*noiseInfos.scale+noiseInfos.offset.y)*noiseInfos.height;

			Vector3 initialPlanetPosition = new Vector3(0, 0, distanceToSun);

			float rotation = Mathf.PerlinNoise(randomess*noiseInfos.scale/2+noiseInfos.offset.x, randomess*noiseInfos.scale/2+noiseInfos.offset.z)*Mathf.PI*2;
			initialPlanetPosition = PolarPoint.pivotPointWithAxisYFromCenter(initialPlanetPosition, rotation);

			//GameObject newPlanet = (GameObject)Instantiate(Resources.Load("Planets/"+currentPlanetId, typeof(GameObject)));
			GameObject newPlanet = GameObject.CreatePrimitive(PrimitiveType.Cube);
			newPlanet.name = "Planet_"+currentPlanetCounter;
			newPlanet.transform.position = initialPlanetPosition;
			newPlanet.transform.parent = transform;

			currentPlanetCounter++;
			if(currentPlanetCounter>avaliablePlanets)
				currentPlanetCounter = 0;
		}
	}
}
