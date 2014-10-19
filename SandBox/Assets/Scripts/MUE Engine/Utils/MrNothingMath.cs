using UnityEngine;
using System.Collections;

public class MrNothingMath : MonoBehaviour {

	public static Vector3 projectPointToPlane(Vector3 point, Vector3 planePoint, Vector3 planeNormal)
	{
		//plane's equation...
		float t = (planeNormal.x * planePoint.x - planeNormal.x * point.x + planeNormal.y * planePoint.y - planeNormal.y * point.y + planeNormal.z * planePoint.z - planeNormal.z * point.z) / (planeNormal.x * planeNormal.x + planeNormal.y * planeNormal.y + planeNormal.z * planeNormal.z);
		
		return new Vector3(point.x+t*planeNormal.x, point.y+t*planeNormal.y, point.z+t*planeNormal.z);
	}

	float normalizeAngle(float angle)
	{
		if(angle>Mathf.PI)
		{
			angle = 2*Mathf.PI-angle;
		}
		
		return angle;
	}

	public static float rangeFactor(float t, float point, float range)
	{
		float ratio = Mathf.Abs (point - t) / range;
		if (ratio < 1) 
		{
			return 1 - ratio;
		} 
		else
			return 0;
	}
	
	public static float rangeFactor(float t, float point, float range, bool rightSide)
	{
		float ratio = (point - t) / range;
		if (ratio < 1 && ratio > 0) 
		{
			return 1 - ratio;
		} 
		else
			return 0;
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
