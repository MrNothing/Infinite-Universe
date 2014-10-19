using UnityEngine;
using System.Collections;
 
public class WowPlanetCamera : MonoBehaviour {
    public Transform target;
    public float upDistance = 7.0f;
    public float backDistance = 10.0f;
    public float trackingSpeed = 3.0f;
    public float rotationSpeed = 9.0f;
	
	public Vector3 rotationOffset;
     
    private Vector3 v3To;
    private Quaternion qTo;
	
	public Transform pivot;
    public Transform zoomCamera;
	
	Vector3 cameraRot=Vector3.zero;
	public Vector2 bounds=Vector3.zero;
	public Vector2 sensitivity=Vector3.zero;
	
	public float zoomSpeed=1;
	float zoom;
	float realZoom = 1;
	
	public Vector2 zoomLimits = new Vector2(1, 10);
	
	Vector2 lastMousePos = Vector2.zero;

	public bool enableClick = false;
	
    void LateUpdate () {
		
		if(Input.GetMouseButton(0) || !enableClick)
		{
			cameraRot.x -= Input.GetAxis("Mouse Y")*sensitivity.x;
			cameraRot.y += Input.GetAxis("Mouse X")*sensitivity.y;
			
			if(cameraRot.x>bounds.x)
				cameraRot.x = bounds.x;
			
			if(cameraRot.x<-bounds.x)
				cameraRot.x = -bounds.x;
			
		}
		
		v3To = target.position - target.forward * backDistance + target.up * upDistance;
	    transform.position = Vector3.Lerp (transform.position, v3To, trackingSpeed * Time.deltaTime);
	    transform.rotation = target.rotation;
    	
		pivot.localEulerAngles = cameraRot;
		
		zoom-=Input.GetAxis("Mouse ScrollWheel")*zoomSpeed;
		
		if(zoom<zoomLimits.x)
			zoom = zoomLimits.x;
		
		if(zoom>zoomLimits.y)
			zoom = zoomLimits.y;
		
		realZoom+=(zoom-realZoom)/10;

		zoomCamera.transform.localPosition = new Vector3(0.5f, 0, -realZoom);

		if (PlanetCharacterController.currentPlanet != null) 
		{
			if(!PlanetCharacterController.currentPlanet.isIsland)
			{
				if (Vector3.Distance (zoomCamera.transform.position, PlanetCharacterController.currentPlanet.transform.position) < PlanetCharacterController.currentPlanet.getFragment (zoomCamera.transform.position).point.magnitude) {
					zoomCamera.transform.position = PlanetCharacterController.currentPlanet.transform.position + PlanetCharacterController.currentPlanet.getFragment (zoomCamera.transform.position).point;
				}
			}
			else
			{
				//if (Vector3.Distance (transform.position, PlanetCharacterController.currentPlanet.transform.position) < PlanetCharacterController.currentPlanet.getIslandFragment (transform.position).magnitude) {
				//	zoomCamera.position = PlanetCharacterController.currentPlanet.transform.position + PlanetCharacterController.currentPlanet.getIslandFragment (transform.position);
				//}
			}
		}

	}
}