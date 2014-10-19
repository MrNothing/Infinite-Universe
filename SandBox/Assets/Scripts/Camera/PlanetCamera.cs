using UnityEngine;
using System.Collections;

public class PlanetCamera : MonoBehaviour {
	
	public GameObject character;
	
    //can manually set position in the inspector
    //will change camera position relative to player
    public Vector3 positionVector;
	public Vector3 lookVector;
	public Vector3 boundsVector;
	public Vector2 sensitivity;
	
    //SmoothFollower class in Util.cs in Locomotion System
    private SmoothFollower posFollow;
	private SmoothFollower lookFollow;
	private Vector3 lastVelocityDir;
	private Vector3 lastPos;

   private PhysicsCharacterMotor phsxMotor;
	
	// Use this for initialization
	void Start () {
        // new SmoothFollower (float smoothingTime, float predition)
		posFollow = new SmoothFollower(0.2f,0.2f);
		lookFollow = new SmoothFollower(0.1f,0.0f);
		posFollow.Update(transform.position,0,true);
		lookFollow.Update(character.transform.position,0,true);
		lastVelocityDir = character.transform.forward;
		lastPos = character.transform.position;

        phsxMotor = GameObject.Find("Player").GetComponent<PhysicsCharacterMotor>();

	}
	
	// Update is called once per frame
	void LateUpdate () {
		
		if(Input.GetMouseButton(0))
		{
			lookVector.z -= Input.GetAxis("Mouse X")*sensitivity.x;
			
			if(lookVector.z>boundsVector.x)
				lookVector.z = boundsVector.x;
			
			if(lookVector.z<-boundsVector.x)
				lookVector.z = -boundsVector.x;
			
			lookVector.y -= Input.GetAxis("Mouse Y")*sensitivity.y;
			
			if(lookVector.y>boundsVector.y)
				lookVector.y = boundsVector.y;
			
			if(lookVector.y<-boundsVector.y)
				lookVector.y = -boundsVector.y;
		}
		
		lastVelocityDir += (character.transform.position-lastPos)*8;
		lastPos = character.transform.position;
		lastVelocityDir += character.transform.forward*Time.deltaTime;
		lastVelocityDir = lastVelocityDir.normalized;
		
        //offset of camera and character
        Vector3 horizontal = transform.position-character.transform.position;
		
        Vector3 horizontal2 = horizontal;
		Vector3 vertical = character.transform.up;
		Vector3.OrthoNormalize(ref vertical,ref horizontal2);
		if (horizontal.sqrMagnitude > horizontal2.sqrMagnitude) horizontal = horizontal2;

        //posFollow.Update(Vector3 targetPositionNew, float deltaTime)
        transform.position = posFollow.Update(
			character.transform.position + horizontal*Mathf.Abs(positionVector.z) + vertical*positionVector.y,
			Time.deltaTime*4
		);
		
		horizontal = lastVelocityDir;
		Vector3 look = lookFollow.Update(character.transform.position + horizontal*lookVector.z - vertical*lookVector.y, Time.deltaTime/5);

        //creates a cross product to stabilize the right vector on camera
        Vector3 crossX = Vector3.Cross(transform.forward, -character.transform.up);
        transform.right = crossX;
        
        
        transform.rotation = Quaternion.FromToRotation(transform.forward, look-transform.position) * transform.rotation;
	}

}//end class
