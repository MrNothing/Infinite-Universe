using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshLod : MonoBehaviour {

	MeshFilter myFilter; 
	Hashtable indexedTriangles;
	Hashtable indexedVertices;
	public Camera mainCamera;
	public float activationDistance = 2f;
	float GLOBAL_STEP = 1f;
	public Planet planet;
	//public float lodScale = 0.3f;

	[HideInInspector]
	public int level = 1;

	public int maxLevel = 3;

	// Use this for initialization
	void Start () {
		myFilter = GetComponent<MeshFilter> ();
		//indexedTriangles = MeshHelper.IndexTriangles (myFilter.mesh, GLOBAL_STEP);
		//indexedVertices = MeshHelper.IndexVertices (myFilter.mesh, GLOBAL_STEP);
		//myFilter.mesh.RecalculateNormals ();
		
		Invoke("init", 1);
	}
	
	void init()
	{
		//planet.shapeAsPlanet(myFilter);
	}

	bool isLodCreated = false;

	public GameObject lodPlane;
	GameObject lod;

	float distance;
	// Update is called once per frame
	void Update () 
	{
		distance = Vector3.Distance(mainCamera.transform.position, transform.position);
		if (distance < activationDistance) 
		{
			if(!isLodCreated)
			{
				if(level<maxLevel)
				{
					GameObject go = new GameObject();
	
					MeshFilter meshToClone = lodPlane.GetComponent<MeshFilter>();
					Mesh mesh = go.AddComponent<MeshFilter>().mesh;
	
					mesh.vertices = meshToClone.mesh.vertices;
					mesh.uv = meshToClone.mesh.uv;
					mesh.triangles = meshToClone.mesh.triangles;
					mesh.normals = meshToClone.mesh.normals;
	
					//MeshHelper.Subdivide(mesh, 4);
					//mesh.RecalculateNormals();
	
					MeshRenderer myRenderer = go.AddComponent<MeshRenderer>();
					myRenderer.materials = renderer.materials;
					//myRenderer.enabled = false;
	
					Vector3 localCameraPos = mainCamera.transform.position-transform.position;
					localCameraPos.y=0;
	
					go.transform.position = planet.getFragment(mainCamera.transform.position).point;
					
					go.transform.rotation = Quaternion.LookRotation(transform.position);
					
					go.transform.localScale = new Vector3(1f/(float)(level+1), 1f/(float)(level+1), 1f/(float)(level+1));
					
					go.transform.parent = transform;
	
					go.name = "Lod"+level;
	
					/*testVerticesOnPlane myNoiseManager = GetComponent<testVerticesOnPlane>();
					testVerticesOnPlane newNoiseManager = go.AddComponent<testVerticesOnPlane>();
	
					newNoiseManager.height = myNoiseManager.height*(1f/go.transform.localScale.x);
					newNoiseManager.scale = myNoiseManager.scale;
					newNoiseManager.offset = myNoiseManager.offset;
					
					myNoiseManager.desiredAlpha = 0;
					*/
				
				
					MeshLod lodManager = go.AddComponent<MeshLod>();
					lodManager.level = level+1;
					lodManager.maxLevel = maxLevel;
					lodManager.lodPlane = lodPlane;
					lodManager.mainCamera = mainCamera;
					lodManager.activationDistance = activationDistance/2;
					lodManager.mainCamera = mainCamera;
					lodManager.planet = planet;
				

					lod = go;
	
					Debug.Log("LOD ON");
				}
				
				isLodCreated = true;

			}
		}
		else 
		{
			if(isLodCreated)
			{
				//lod.GetComponent<testVerticesOnPlane>().desiredAlpha = 0;
				//lod.GetComponent<testVerticesOnPlane>().destroyWhenInvisible = true;

				Debug.Log("LOD OFF");
				isLodCreated = false;

				//testVerticesOnPlane myNoiseManager = GetComponent<testVerticesOnPlane>();
				//myNoiseManager.desiredAlpha = 1;
			}
		}
	}

	int getNearestVertice()
	{
		float bestDistance = float.MaxValue;

		Vector3 localCameraPos = mainCamera.transform.position-transform.position;

		float distance;

		int chosenVertice = -1;

		for(int i=-1; i<=1; i++)
		{
			for(int j=-1; j<=1; j++)
			{
				for(int k=-1; k<=1; k++)
				{
					Vector3 flatPosition = MeshHelper.flatten(mainCamera.transform.position-transform.position+new Vector3(i*GLOBAL_STEP, j*GLOBAL_STEP, k), GLOBAL_STEP);
					//Debug.Log("Checking: "+flatPosition);

					if(indexedVertices[flatPosition.ToString()]!=null)
					{
						List<int> vertInBloc = (List<int>)indexedVertices[flatPosition.ToString()];

						foreach(int vertice in vertInBloc)
						{

							distance = Vector3.Distance(localCameraPos, myFilter.mesh.vertices[vertice]);

							if(bestDistance>distance)
							{
								chosenVertice = vertice;
								bestDistance = distance;
							}
						}
					}
					else
					{
						//no triangles here... 
					}
				}
			}
		}

		return chosenVertice;
	}
	
	public static Vector3[] recalculateNormalsSmooth(Mesh mesh)
	{
		Vector3[] vertices = mesh.vertices;
		Vector3[] normals = mesh.normals;
		var firstVertex = 0;
		for ( var i=1; i<vertices.Length-1; i+=2 ) {
			if ( vertices[i] == vertices[i+1] ) {
				Vector3 averageNormal = ( normals[i] + normals[i+1] )/2;
				normals[i] = averageNormal;
				normals[i+1] = averageNormal;
			}
			else {
				Vector3 averageNormal = ( normals[firstVertex] + normals[i] )/2;
				normals[firstVertex] = averageNormal;
				normals[i] = averageNormal;
				firstVertex = i+1;
			}
		}

		return normals;
	}
	
	 void CalculateNormals (Mesh mesh) {
 
		Vector3[] normals = mesh.normals;
		Vector3[] vertices = mesh.vertices;
        int[] trigs = mesh.triangles;
 
        for(int i = 0; i < trigs.Length; i+=3) {
 
			Vector3 avg = (vertices[trigs[i]] + vertices[trigs[i+1]] + vertices[trigs[i+2]])/3;
            normals[trigs[i]] = avg;
            normals[trigs[i+1]] = avg;
            normals[trigs[i+2]] = avg;
 
        }
 
    }
}
