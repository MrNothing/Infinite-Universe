using UnityEngine;
using System.Collections;

public class AthmosphereScript : MonoBehaviour {
	
	public Transform sun;
	public GameObject planet;
	MeshFilter myFilter;
	// Use this for initialization
	void Awake()
	{
		sun = GlobalCore.sun;
	}

	void Start () {
		myFilter = GetComponent<MeshFilter>();
	}
	
	// Update is called once per frame
	void Update () {
		renderer.material.SetVector("_Sun", (sun.transform.position-transform.position).normalized);
		try
		{
			planet.renderer.material.SetVector("_LightDir", (sun.transform.position-transform.position).normalized);
		}
		catch
		{
				
		}
		renderer.material.SetVector("_LightDir", (sun.transform.position-transform.position).normalized);
		//myFilter.mesh.vertices = planet.mesh.vertices;
		//myFilter.mesh.RecalculateBounds();
		//myFilter.mesh.RecalculateNormals();
	}
}
