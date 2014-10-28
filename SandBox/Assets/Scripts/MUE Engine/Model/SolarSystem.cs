using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SolarSystem : MonoBehaviour {

	public int maxPlanets = 10;
	public PerlinConfig noiseInfos;
	public int avaliablePlanets = 3; 
	SimplexNoiseGenerator simplex;
	// Use this for initialization
	void Start () {
		GlobalCore.sun = transform;
		simplex = new SimplexNoiseGenerator (GlobalCore.seed);

		int currentPlanetCounter = (int)(Mathf.PerlinNoise(noiseInfos.offset.x, noiseInfos.offset.y)*avaliablePlanets);

		List<Vector3[]> planetsPos = new List<Vector3[]>();

		//max 10 panets
		for(float randomess = 1; randomess<maxPlanets+1; randomess++)
		{
			float distanceToSunX = simplex.coherentNoise(randomess*noiseInfos.scale+noiseInfos.offset.x, randomess*noiseInfos.scale+noiseInfos.offset.y, randomess*noiseInfos.scale+noiseInfos.offset.z)*noiseInfos.height*10;
			float distanceToSunY = simplex.coherentNoise(randomess*noiseInfos.scale/2+noiseInfos.offset.x, randomess*noiseInfos.scale*2+noiseInfos.offset.y, randomess*noiseInfos.scale/2+noiseInfos.offset.z)*noiseInfos.height*10;
			float distanceToSunZ = simplex.coherentNoise(randomess*noiseInfos.scale*2+noiseInfos.offset.x, randomess*noiseInfos.scale*2+noiseInfos.offset.y, randomess*noiseInfos.scale/2+noiseInfos.offset.z)*noiseInfos.height*10;

			Vector3 initialPlanetPosition = new Vector3(distanceToSunX, distanceToSunY, distanceToSunZ);
			float planetScale = 0.5f+initialPlanetPosition.magnitude/noiseInfos.height*0.5f+Mathf.Abs (Mathf.PerlinNoise(randomess*noiseInfos.scale+noiseInfos.offset.x, randomess*noiseInfos.scale+noiseInfos.offset.y))*1.5f;
			
			bool allow = true;

			foreach(Vector3[] v in planetsPos)
			{
				if(Vector3.Distance(v[0], initialPlanetPosition)<v[1].x+700*planetScale*2)
				{
					allow = false;
					break;
				}
			}

			if(allow)
			{
				Vector3[] tmpInfos = new Vector3[2];
				tmpInfos[0] = initialPlanetPosition;
				tmpInfos[1] = new Vector3(700*planetScale, 0, 0);
				planetsPos.Add(tmpInfos);
				float rotation = Mathf.PerlinNoise(randomess*noiseInfos.scale+noiseInfos.offset.x, randomess*noiseInfos.scale+noiseInfos.offset.z)*180;
				float rotation2 = Mathf.PerlinNoise(randomess*noiseInfos.scale/2+noiseInfos.offset.x, randomess*noiseInfos.scale*2+noiseInfos.offset.z)*180;

				GameObject newPlanet = (GameObject)Instantiate(Resources.Load("Planets/Planet", typeof(GameObject)));
				//GameObject newPlanet = GameObject.CreatePrimitive(PrimitiveType.Cube);
				newPlanet.name = "Planet_"+currentPlanetCounter;
				newPlanet.transform.position = initialPlanetPosition;
				newPlanet.transform.parent = transform;
				Planet planetScript = newPlanet.GetComponent<Planet>();
				MeshLodTris lodScript = newPlanet.GetComponent<MeshLodTris>();

				float seaLevel = (Mathf.PerlinNoise(randomess*noiseInfos.scale*100+noiseInfos.offset.x, randomess*noiseInfos.scale*100+noiseInfos.offset.y))*25;
				Color planetAtmoColor = new Color();
				planetAtmoColor.r = Mathf.Abs (Mathf.PerlinNoise(randomess*noiseInfos.scale*50+noiseInfos.offset.x, randomess*noiseInfos.scale+noiseInfos.offset.y))*1;
				planetAtmoColor.g = Mathf.Abs (Mathf.PerlinNoise(randomess*noiseInfos.scale*150-noiseInfos.offset.x, randomess*noiseInfos.scale+noiseInfos.offset.y))*1.5f;
				planetAtmoColor.b = Mathf.Abs (Mathf.PerlinNoise(-randomess*noiseInfos.scale*300+noiseInfos.offset.x, randomess*noiseInfos.scale+noiseInfos.offset.y))*2;

				planetScript.seaLevel = 0;
				if(planetAtmoColor.grayscale<0.4)
				{
					planetScript.hasFog = false;
					planetScript.skyDome.renderer.enabled = false;
					planetScript.seaLevel = -50;
					planetScript.doodads = new Doodad[0];
				}

				planetScript.skyDome.renderer.material.SetColor("_Color", planetAtmoColor);
				planetScript.sun = this.transform;
				planetScript.scale = planetScale;

				planetScript.GetComponent<SphereCollider>().radius *= planetScript.scale;

				lodScript.area = 200*planetScript.scale;
				lodScript.GLOBAL_STEP = 80*planetScript.scale;

				/*if(planetScale>1.5 && planetScript.hasFog)
				{
					GameObject ring = (GameObject)Instantiate(Resources.Load("Planets/Ring", typeof(GameObject)));
					ring.GetComponent<AsteroidField>().range*=planetScale/2;
					ring.transform.position = newPlanet.transform.position;
					ring.transform.parent = newPlanet.transform;
					ring.transform.localScale *= planetScale/2;
					ring.transform.LookAt(transform.position);
				}*/

				currentPlanetCounter++;
				if(currentPlanetCounter>avaliablePlanets)
					currentPlanetCounter = 0;
			}
		}

		GameObject.FindObjectOfType<PlanetCharacterController> ().planets = (Planet[])GameObject.FindObjectsOfType(typeof(Planet));;
	}
}
