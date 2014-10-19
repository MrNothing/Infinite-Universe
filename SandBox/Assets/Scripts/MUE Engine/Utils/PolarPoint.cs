using UnityEngine;

[System.Serializable]
public class PolarPoint
{
	public float theta;
	public float phi;
	public float r;
	public PolarPoint(float _theta, float _phi, float _r)
	{
		theta = _theta;
		phi = _phi;
		r = _r;
	}
	
	public static Vector3 getCartesianCoordinates(PolarPoint polarCoords)
	{
		float a = polarCoords.r * Mathf.Cos(polarCoords.phi);
		return new Vector3(a * Mathf.Cos(polarCoords.theta), polarCoords.r * Mathf.Sin(polarCoords.phi), a * Mathf.Sin(polarCoords.theta));
	}
	
	public static PolarPoint getPolarCoordinates(Vector3 localPoint, float radius)
	{
		Vector3 cartesianCoordinate = localPoint;
		if( cartesianCoordinate.x == 0f )
			cartesianCoordinate.x = Mathf.Epsilon;
		float r = cartesianCoordinate.sqrMagnitude;
		
		float theta = Mathf.Atan(cartesianCoordinate.z / cartesianCoordinate.x);
		
		if( cartesianCoordinate.x < 0f )
			theta += Mathf.PI;
		
		float phi = Mathf.Asin(cartesianCoordinate.y / radius);
		
		return new PolarPoint(theta, phi, r);
	}
	
	public static float getDistanceFromAngle(float angle, float radius)
	{
		Vector2 origin = new Vector2(0, radius);
		Vector2 anglePoint = new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle));
		
		return Vector2.Distance(origin, anglePoint);
	}
	
	public static float getAngleFromDistance(float distance, float radius)
	{
		float K = (distance*distance)/(radius*radius)-2f;
		return Mathf.Acos(K/-2f)+Mathf.PI/2;
	}
	
	public static float normalizeAngle(float angle)
	{
		if(angle>Mathf.PI)
		{
			angle = 2*Mathf.PI-angle;
		}
		
		return angle;
	}
	
	public static PolarPoint flatten(PolarPoint input, float step)
	{
		return new PolarPoint (Mathf.Floor (input.theta / step) * step, Mathf.Floor (input.phi / step) * step, input.r);
	}
	
	public string ToString()
	{
		return theta+"_"+phi;
	}
	
	public static Vector3 pivotPointWithAxisXFromCenter(Vector3 input, float angle)
	{
		float Rotated_z = input.z * Mathf.Cos( angle ) - input.y * Mathf.Sin( angle );
		float Rotated_y = input.z * Mathf.Sin( angle ) + input.y * Mathf.Cos( angle ); 
		return new Vector3(input.x, Rotated_y, Rotated_z);
	}
	
	public static Vector3 pivotPointWithAxisYFromCenter(Vector3 input, float angle)
	{
		float Rotated_x = input.x * Mathf.Cos( angle ) - input.z * Mathf.Sin( angle );
		float Rotated_z = input.x * Mathf.Sin( angle ) + input.z * Mathf.Cos( angle );
		return new Vector3(Rotated_x, input.y, Rotated_z);
	}
	
	public static Vector3 pivotPointWithAxisZFromCenter(Vector3 input, float angle)
	{
		float Rotated_x = input.x * Mathf.Cos( angle ) - input.y * Mathf.Sin( angle );
		float Rotated_y = input.x * Mathf.Sin( angle ) + input.y * Mathf.Cos( angle );
		return new Vector3(Rotated_x, Rotated_y, input.z);
	}
}