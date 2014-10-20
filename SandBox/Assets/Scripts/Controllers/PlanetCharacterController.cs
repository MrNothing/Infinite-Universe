using UnityEngine;
using System.Collections;

public enum PlanetCharacterControllerMode
{
	Humanoid, Spaceship
}

public class PlanetCharacterController : MonoBehaviour {
	
	public float moveSpeed=1;
	public float accelerationPower = 0.1f;
	public float deccelerationPower = 0.01f;
	float forwardAccel = 0;

	public Transform mainCamera;
	public float gravity=1;
	public Vector3 gravityCenter;

	public bool autoSlow = true;
	public bool canFly = true;

	public PlanetCharacterControllerMode mode = PlanetCharacterControllerMode.Humanoid;
	
	//public Transform water;
	// Use this for initialization
	
	public Planet[] planets;
	AsteroidField[] asteroids;
	
	//public Transform water;
	
	Vector3 lastPosition;

	public Entity player;

	//SunShafts sunShaft;
	
	void Start () {
		planets = (Planet[])GameObject.FindObjectsOfType(typeof(Planet));
		asteroids = (AsteroidField[])GameObject.FindObjectsOfType(typeof(AsteroidField));
		lastPosition = transform.position;
	}

	float realMoveSpeed = 1;

	Vector3 gravityVec;

	// Update is called once per frame
	void Update () {
		
		getGravityCenter();
		
		AdjustToGravity();

		if (autoSlow)
			realMoveSpeed = moveSpeed * (0.025f + realMoveSpeed);
		else
			realMoveSpeed = moveSpeed;

		if (!canFly && PlanetCharacterController.currentPlanet != null)
			gravityVec = (PlanetCharacterController.currentPlanet.transform.position - transform.position).normalized * gravity;
		else
			gravityVec = Vector3.zero;

		if(mode==PlanetCharacterControllerMode.Humanoid)
		{
			transform.position += mainCamera.forward*Time.deltaTime*realMoveSpeed*Input.GetAxis("Vertical") + mainCamera.right*Time.deltaTime*realMoveSpeed*Input.GetAxis("Horizontal") + gravityVec;
		}
		else
		{
			/*forwardAccel+=Input.GetAxis("Vertical")*accelerationPower;

			if(forwardAccel>2)
				forwardAccel = 2;

			if(forwardAccel<-2)
				forwardAccel = -2;

			float realforwardAccel = forwardAccel;

			if(realforwardAccel>1)
				realforwardAccel = 1;

			if(realforwardAccel<-1)
				realforwardAccel = -1;*/

			transform.position += mainCamera.forward*Time.deltaTime*realMoveSpeed*Input.GetAxis("Vertical") + mainCamera.right*Time.deltaTime*realMoveSpeed*Input.GetAxis("Horizontal")/3 + gravityVec;

			/*if(forwardAccel>0)
				forwardAccel-=deccelerationPower;
			if(forwardAccel<0)
				forwardAccel+=deccelerationPower;
				*/
		}

		if (PlanetCharacterController.currentPlanet != null) 
		{
			if(!PlanetCharacterController.currentPlanet.isIsland)
			{
				if (Vector3.Distance (transform.position, PlanetCharacterController.currentPlanet.transform.position) < PlanetCharacterController.currentPlanet.getFragment (transform.position).point.magnitude) {
					transform.position = PlanetCharacterController.currentPlanet.transform.position + PlanetCharacterController.currentPlanet.getFragment (transform.position).point;
				}
			}
			else
			{
				//calculate collision with this shit somehow...
				//if (Vector3.Distance (transform.position, PlanetCharacterController.currentPlanet.transform.position) < PlanetCharacterController.currentPlanet.getIslandFragment (transform.position).magnitude) {
				//	transform.position = PlanetCharacterController.currentPlanet.transform.position + PlanetCharacterController.currentPlanet.getIslandFragment (transform.position);
				//}
			}
		}

		player.transform.position = transform.position;
		player.transform.rotation = transform.rotation;
		/*if(Vector3.Distance(transform.position, lastPosition)>15)
		{	
			updateWater= true;
			lastPosition = transform.position;
		}*/
	}
	
    public static Planet currentPlanet=null;
    bool updateWater=false;
	void getGravityCenter()
	{
		Vector3 offset = Vector3.zero;

		PlanetCharacterController.currentPlanet=null;
		realMoveSpeed = 1;

		float bestRatio=2;
		float bestDistanceRatio = 2;
		for (int i=0; i<asteroids.Length; i++) 
		{
			float distanceRatio = (Vector3.Distance(transform.position, asteroids[i].transform.position)-asteroids[i].fieldSize)/asteroids[i].fieldSize;
			if(distanceRatio>1)
				distanceRatio=1;

			if(distanceRatio<0.3f)
				distanceRatio=0.3f;

			if(bestDistanceRatio>distanceRatio)
			{
				realMoveSpeed = distanceRatio*realMoveSpeed;
				bestDistanceRatio = distanceRatio;
			}
		}

		for(int i=0; i<planets.Length; i++)
		{
			//float distanceRatio = (Vector3.Distance(transform.position, planets[i].transform.position)-planets[i].radius)/planets[i].radius;
			float distanceRatio;
			if(!planets[i].isIsland)
				distanceRatio = Vector3.Distance(transform.position, planets[i].transform.TransformPoint(planets[i].getFragment(transform.position).point))/planets[i].radius;
			else
				distanceRatio = Vector3.Distance(transform.position, planets[i].transform.TransformPoint(planets[i].getIslandFragment(transform.position)))/planets[i].radius;
				
			float fogRatio; 

			if(!planets[i].isIsland)
				fogRatio = Vector3.Distance(transform.position, planets[i].transform.TransformPoint(planets[i].getFragment(transform.position).point))/planets[i].skyDistance;
			else
				fogRatio = Vector3.Distance(transform.position, planets[i].transform.TransformPoint(planets[i].getIslandFragment(transform.position)))/planets[i].skyDistance;
				
			if(distanceRatio>1)
				distanceRatio=1;

			if(!planets[i].isIsland)
				offset+=(planets[i].transform.position-transform.position)*(1-distanceRatio);
			else
				offset+=-planets[i].IslandAxis.normalized*planets[i].radius*(1-distanceRatio);

			if(bestDistanceRatio>distanceRatio)
			{
				realMoveSpeed = distanceRatio*realMoveSpeed;
				bestDistanceRatio = distanceRatio;
			}

			if(bestRatio>fogRatio)
			{
				PlanetCharacterController.currentPlanet = planets[i];
				bestRatio = fogRatio;
			}
				//water.transform.position = (transform.position-planets[i].transform.position).normalized*planets[i].radius;
				//water.transform.up = (transform.position-planets[i].transform.position).normalized;
			//}
		}

		gravityCenter = transform.position+offset+new Vector3(0, -500, 0)*bestDistanceRatio;

		if (PlanetCharacterController.currentPlanet != null && PlanetCharacterController.currentPlanet.hasFog) {
			RenderSettings.fogDensity = 0.01f * (1 - bestRatio);
			if(RenderSettings.fogDensity>0.02f)
				RenderSettings.fogDensity=0.02f;

			Color atmosphereColor;
			
			try
			{
				atmosphereColor = PlanetCharacterController.currentPlanet.skyDome.renderer.material.GetColor("_Color");
				atmosphereColor.a = 1;
			}
			catch
			{
				atmosphereColor = new Color(1,1,1,1);
			}

			Color colo = PlanetCharacterController.currentPlanet.getAtmosphereColor (transform.position)*atmosphereColor;
			if(colo.r>1)
				colo.r = 1;
			if(colo.g>1)
				colo.g = 1;
			if(colo.b>1)
				colo.b = 1;
			if(colo.a>1)
				colo.a = 1;

			if(bestRatio>1)
				bestRatio =1;

			PlanetCharacterController.currentPlanet.skyDome.renderer.material.SetFloat("_SunAmplification", 0.5f+1f * (1f - Mathf.Abs(colo.r)* bestRatio));
			try
			{
				//PlanetCharacterController.currentPlanet.clouds.renderer.material.SetColor ("_SunColor", new Color(colo.r, colo.g, colo.b, 0.5f));
			}
			catch
			{}

			RenderSettings.fogColor = colo;
		} else {
			if(PlanetCharacterController.currentPlanet != null)
			{
				try
				{
					//Color colo = PlanetCharacterController.currentPlanet.getAtmosphereColor (transform.position);
					//PlanetCharacterController.currentPlanet.clouds.renderer.material.SetColor ("_SunColor", new Color(colo.r, colo.g, colo.b, 0.5f));
				}
				catch
				{

				}

				try
				{
					PlanetCharacterController.currentPlanet.skyDome.renderer.material.SetFloat("_SunAmplification", 1f);
				}
				catch
				{

				}
				RenderSettings.fogDensity = 0;
			}
		}
	}
	
	private void AdjustToGravity() {
		Vector3 currentUp = transform.up;
				
		float damping = Mathf.Clamp01(Time.deltaTime*5);
		
		Vector3 desiredUp = (transform.position - gravityCenter).normalized;

		desiredUp = (currentUp+desiredUp).normalized;
		Vector3 newUp = (currentUp+desiredUp*damping).normalized;
		
		float angle = Vector3.Angle(currentUp,newUp);
		if (angle>0.01) {
			Vector3 axis = Vector3.Cross(currentUp,newUp).normalized;
			Quaternion rot = Quaternion.AngleAxis(angle,axis);
			transform.rotation = rot * transform.rotation;
		}
	}
}
