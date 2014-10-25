using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class MrNothingTerrain : MonoBehaviour 
{
	[HideInInspector]
	public List<Material> details = new List<Material>();
	[HideInInspector]
	public int brushType=0;
	[HideInInspector]
	public int channel=0;
	[HideInInspector]
	public int selectedDetail=0;

	[HideInInspector]
	public float brushSize=1;
	[HideInInspector]
	public float brushIntensity=1;
	[HideInInspector]
	public float max=10;
	[HideInInspector]
	public float min=10;

	[HideInInspector]
	public Vector3[] initVertices=null;

	public Hashtable detailsIndex  = new Hashtable();
	// Use this for initialization
	void Start () {
		
	}
}
