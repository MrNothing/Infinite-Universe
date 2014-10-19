using UnityEngine;
using System.Collections;

[System.Serializable]
public class EntityAnimations
{
	public string idle;
	public string run;
	public string walk;
	public float runAnimSpeed = 1;
	
	//actions
	public string attack;
	
	//air animations	
	public string jumpStart;
	public string floating;
	public string floatingForward;
	public string jumpEnd;
}

public class Entity : MonoBehaviour {
	public PlanetCharacterController myController=null;

	public GameObject Main;
	public Animation anim;

	public Vector3 rotationOffset;
	public EntityAnimations animations;
	string currentAnim="";
	[HideInInspector]
	public bool grounded = false;
	public bool isSpaceShip=false;
	// Use this for initialization
	void Start () {
		lastPosition=transform.position;
	}
	
	Vector3 lastPosition;
	public float distanceRatio = 1;	
	// Update is called once per frame
	void Update () {
	
		if (PlanetCharacterController.currentPlanet != null) 
		{
			if (Vector3.Distance (transform.position, PlanetCharacterController.currentPlanet.transform.position) - 0.1f < PlanetCharacterController.currentPlanet.getFragment (transform.position).point.magnitude) 
				grounded = true;
			else
				grounded = false;
		}
		else
			grounded = false;

		if ((transform.position-lastPosition).magnitude > 0.1f)
		{
			if(grounded)
				currentAnim = animations.run;
			else
				currentAnim = animations.floatingForward;

			anim[currentAnim].speed = animations.runAnimSpeed;

			Quaternion rot;
			if(myController!=null)
				rot = Quaternion.LookRotation(myController.mainCamera.transform.forward);
			else
				rot = Quaternion.LookRotation(transform.position-lastPosition);


			Main.transform.rotation = rot;

			if(!isSpaceShip)
			{
				Main.transform.localEulerAngles = new Vector3(0, Main.transform.localEulerAngles.y, 0)+rotationOffset;
			}
			else
			{
				distanceRatio = 1;
				if(PlanetCharacterController.currentPlanet != null)
				{
					distanceRatio = Vector3.Distance(transform.position, PlanetCharacterController.currentPlanet.transform.TransformPoint(PlanetCharacterController.currentPlanet.getFragment(transform.position).point))/PlanetCharacterController.currentPlanet.radius;
					distanceRatio*=5;
					if(distanceRatio>1)
						distanceRatio=1;
				}

				Quaternion rotGrounded = Quaternion.Euler(0, Main.transform.localEulerAngles.y, 0);
				Quaternion rotOnAir = Quaternion.Euler(Main.transform.localRotation.eulerAngles.x, Main.transform.localEulerAngles.y, Main.transform.localRotation.eulerAngles.z);

				Main.transform.localRotation = Quaternion.Lerp(rotOnAir, rotGrounded, 1-distanceRatio);
				Main.transform.localEulerAngles = new Vector3(Main.transform.localEulerAngles.x, Main.transform.localEulerAngles.y, 0);

			}
		}
		else
		{
			if(grounded)
				currentAnim = animations.idle;
			else
				currentAnim = animations.floating;

			anim[currentAnim].speed = 1;
		}
		
		lastPosition = transform.position;
		
		anim.CrossFade(currentAnim);
	}
}
