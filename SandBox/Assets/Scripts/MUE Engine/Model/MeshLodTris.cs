using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct MeshMock
{
	public Vector3[] vertices;
	public int[] triangles;
	public Vector2[] uv;
}

public class MeshLodTris : MonoBehaviour {

	MeshFilter myFilter; 
	Hashtable indexedTriangles;
	Hashtable indexedVertices;
	[HideInInspector]
	public Camera mainCamera;
	public float activationDistance = 2f;
	public float area = 1f;
	public float refreshDistance = 1f;
	public float GLOBAL_STEP = 0.5f;
	//public float lodScale = 0.3f;

	public float fading = 0.45f;

	[HideInInspector]
	public int triangle = -1;

	[HideInInspector]
	public int level = 1;

	public int maxLevel = 3;
	
	public int childsLayer;
	public int initSubdivision = 2;
	public int subdivision=4;
	public bool enableDetails=false;

	public bool isIsland = false;
	
	MeshMock oldMesh;
	public GameObject waterTile;
	public GameObject cloudTile;
	
	// Use this for initialization
	void Start () {
		GlobalCore.loadingQueue++;
		myFilter = GetComponent<MeshFilter> ();
		mainCamera = GameObject.FindObjectOfType<PlanetCharacterController> ().mainCamera.camera;
	}

	void initPlanet()
	{

		if (level > 1) 
		{
			if(!planet.isIsland)
				StartCoroutine (planet.perlinPlanet (myFilter, this, true, waterTile, cloudTile));
			else
				StartCoroutine (planet.perlinIsland (myFilter, this, true));

			renderer.material.shader = Shader.Find("MrNothing's Shaders/Vertex Blended Terrain With Fog");
			renderer.material.SetTextureScale ("_Layer1", new Vector2 (planet.lodTilingScale.x * level * level, planet.lodTilingScale.y * level * level));
		}
		else 
		{
			//planet.Invoke ("initClouds", 2);
			if(!planet.isIsland)
				StartCoroutine (planet.perlinPlanet ());
			else
				planet.Invoke("perlinIsland", 0.2f);
		}	
		desiredAlpha= 1;
	}

	bool isLodCreated = false;

	List<MeshLodTris> lod = new List<MeshLodTris>();

	float distance;

	public Planet planet;
	public IslandsLayer Islands;
	// Update is called once per frame

	Vector3 lastHitPoint;

	bool generate = true;
	bool initalized=false;

	bool meshIndexed = false;
	public void OnMeshIndexed()
	{
		MeshHelper.Subdivide (myFilter.mesh, initSubdivision);
		meshIndexed = true;
	}

	void Update () 
	{
		if (!initalized) 
		{
			if(generate)
			{
				oldMesh.triangles=new int[myFilter.mesh.triangles.Length];
				myFilter.mesh.triangles.CopyTo(oldMesh.triangles, 0);
				oldMesh.vertices=new Vector3[myFilter.mesh.vertices.Length];
				myFilter.mesh.vertices.CopyTo(oldMesh.vertices, 0);
				oldMesh.uv=new Vector2[myFilter.mesh.uv.Length];
				myFilter.mesh.uv.CopyTo(oldMesh.uv, 0);
				
				if (level == 1) 
				{
					myFilter.mesh.RecalculateNormals ();
					StartCoroutine(IndexTriangles (myFilter.mesh, GLOBAL_STEP));
				}
				else
					meshIndexed = true;

				if(level>1)
				{
					renderer.material.SetFloat ("_Fading", 0);
					renderer.material.SetFloat ("_DisableSpec1", 0.75f);
					renderer.material.SetFloat ("_Disappear", activationDistance/4);
					try
					{
						waterTile.renderer.material.SetFloat ("_Fading", activationDistance/4);
					}
					catch
					{
					}
					//_Layer1Specular
					//renderer.material.renderQueue = 1;
				}
				//else
				//renderer.material.renderQueue = 2;
				initalized=true;
			}
			return;		
		}

		if(meshIndexed && initalized)
		{
			Invoke("initPlanet", 0.5f);
			GlobalCore.loadingQueue--;
			meshIndexed = false;
		}

		if (!isIsland) 
		{
			if (!planet.initialized)
				return;
		}
		else
		{
			if (!Islands.initialized)
				return;
		}

		Vector3 hitPoint;

		if (!planet.isIsland) 
			hitPoint = planet.transform.TransformPoint(planet.getFragment (mainCamera.transform.position).point);
		else
			hitPoint = planet.transform.TransformPoint(planet.getIslandFragment (mainCamera.transform.position));
			
		distance = Vector3.Distance(mainCamera.transform.position, hitPoint);
		if (distance < activationDistance) 
		{
			if(!isLodCreated)
			{
				isLodCreated = true;

				if(level<maxLevel)
				{
					StartCoroutine(makeLodWithIndex());
				}
				else
				{
					//StartCoroutine(createBlocks());
					//enabled = false;
				}
				
				if(level==1)
					Debug.Log("LOD ON");

				lastHitPoint = hitPoint;

				//if(level<maxLevel)
				//	desiredAlpha= 0;
			}
			else
			{
				distance = Vector3.Distance(lastHitPoint, hitPoint);
				if (distance > refreshDistance) 
				{
					StartCoroutine(clearLods(hitPoint));
					//StopCoroutine("makeLodWithIndex");

					isLodCreated = false;
				}
			}
		}
		else 
		{
			if(isLodCreated)
			{
				foreach(MeshLodTris m in lod)
				{
					try
					{
						Destroy(m.waterTile.renderer.GetComponent<MeshFilter>().mesh);
					}
					catch
					{}	
					try
					{
						Destroy(m.waterTile.renderer.material);
					}catch
					{}	
					try
					{
						Destroy(m.waterTile);
					}catch
					{}	

					try
					{
						Destroy(m.cloudTile.renderer.GetComponent<MeshFilter>().mesh);
					}
					catch
					{}	
					try
					{
						Destroy(m.cloudTile.renderer.material);
					}catch
					{}	
					try
					{
						Destroy(m.cloudTile);
					}catch
					{}	

					doublesChecker.Remove(m.triangle);
					try
					{
						Destroy(m.renderer.GetComponent<MeshFilter>().mesh);}catch
					{}	
					try
					{
						Destroy(m.renderer.material);}catch
					{}	
					try
					{Destroy(m.gameObject);}catch
					{}	
				}

				lod.Clear();


				isLodCreated = false;
				
				if(level==1)
					Debug.Log("LOD OFF");
			}
		}
		
		alpha += (desiredAlpha - alpha) / 10;
		
		if(alpha>0.95)
			alpha = 1;
		
		if (alpha == 1 && level==maxLevel) 
		{
			enabled = false;
		} 
		
		renderer.material.SetFloat ("_GlobalAlpha", alpha);
		try
		{
			waterTile.renderer.material.SetFloat ("_GlobalAlpha", alpha);
		}
		catch
		{
		}
	}
	
	IEnumerator clearLods(Vector3 camPoint)
	{
		List<MeshLodTris> lodCloned = new List<MeshLodTris>(lod);

		foreach(MeshLodTris m in lodCloned)
		{
			try
			{
				if(Vector3.Distance(m.transform.TransformPoint(m.GetComponent<MeshFilter>().mesh.vertices[0]), camPoint)> area * 2)
				{
					try
					{
						Destroy(m.waterTile.renderer.GetComponent<MeshFilter>().mesh);
					}
					catch
					{}	
					try
					{
						Destroy(m.waterTile.renderer.material);
					}catch
					{}	
					try
					{
						Destroy(m.waterTile);
					}catch
					{}	

					try
					{
						Destroy(m.cloudTile.renderer.GetComponent<MeshFilter>().mesh);
					}
					catch
					{}	
					try
					{
						Destroy(m.cloudTile.renderer.material);
					}catch
					{}	
					try
					{
						Destroy(m.cloudTile);
					}catch
					{}	
					doublesChecker.Remove(m.triangle);
					try
					{
						Destroy(m.renderer.GetComponent<MeshFilter>().mesh);}catch
					{}	
					try
					{
						Destroy(m.renderer.material);}catch
					{}	
					try
					{Destroy(m.gameObject);}catch
					{}	
				}
			
			}
			catch
			{
				
			}
			
			yield return new WaitForEndOfFrame();
			
		}
	}
	
	IEnumerator createBlocks()
	{
		MeshFilter subdividedFilter = myFilter;
		MeshHelper.Subdivide(subdividedFilter.mesh, 4);
		
		for(int i=0; i<subdividedFilter.mesh.vertices.Length; i++)
		{
			Vector3 point;
			if (!planet.isIsland) 
				point = planet.getFragment(transform.position+subdividedFilter.mesh.vertices[i]).point;
			else
				point = planet.getIslandFragment(transform.position+subdividedFilter.mesh.vertices[i]);
				
			if(MeshIndexer.CountSurroundingMeshes(point, 1)==0 && Vector3.Distance(mainCamera.transform.position, point)<10)
			{
				
				GameObject bloc = (GameObject)Instantiate(Resources.Load("SphereGrass", typeof(GameObject)), point, Quaternion.identity);
				bloc.transform.parent = transform;
				bloc.transform.LookAt(transform.position);
				bloc.transform.Rotate(-90, 0, 0);
				bloc.GetComponent<MeshBlender>().wait = Vector3.Distance(bloc.transform.position, mainCamera.transform.position)*5;
				bloc.GetComponent<MeshBlender>().isTerrainBloc = true;
				MeshIndexer.IndexMesh(bloc.GetComponent<MeshBlender>());
			}
		}
		yield return new WaitForEndOfFrame();
	}

	Hashtable doublesChecker = new Hashtable();
	
	public int tmpRange = 4;
	
	IEnumerator makeLodWithIndex()
	{
		//Debug.Log("makeLodWithIndex: ");
		
		float currentTime = Time.time;

		Vector3 hitPoint;
		if (!planet.isIsland) 
			hitPoint = planet.getFragment (mainCamera.transform.position).point;
		else
			hitPoint = planet.getIslandFragment (mainCamera.transform.position);
			
		float distance = 0;

		int counter = 0;
		
		//Debug.Log("hitPoint: "+hitPoint);

		for (int _x=-tmpRange; _x<=tmpRange; _x++) 
		{
			for (int _y=-tmpRange; _y<=tmpRange; _y++) 
			{
				for (int _z=-tmpRange; _z<=tmpRange; _z++) 
				{
					string pos_id = MeshHelper.flatten(hitPoint+new Vector3(_x*GLOBAL_STEP, _y*GLOBAL_STEP, _z*GLOBAL_STEP), GLOBAL_STEP).ToString();

					//break;
					if(indexedTriangles[pos_id]!=null)
					{
						List<int> trisInBloc = (List<int>)indexedTriangles[pos_id];

						foreach(int triangle in trisInBloc)
						{
							if(isLodCreated)
							{
								distance = Vector3.Distance(oldMesh.vertices[oldMesh.triangles[0 + 3 * triangle]], hitPoint);

								if(distance<area*2)
								{
									createLodSubDivision (triangle, 0);
									
									if(counter>5)
									{
										yield return new WaitForEndOfFrame();
										counter=0;
									}
									
									counter++;
								}
							}
							else
							{
								break;
							}
						}

						//doublesChecker.Add(triangle, (byte)1);


						//if(!isLodCreated)
						//	break;
					}
				}
			}
		}
	}

	IEnumerator makeLod()
	{
		float currentTime = Time.time;
		
		Vector3 hitPoint = planet.getFragment (mainCamera.transform.position).point;
		
		float distance = 0;
		int counter = 0;
		
		for (int triangle=0; triangle<oldMesh.triangles.Length/3; triangle++) 
		{
			if(distance<area && doublesChecker[triangle]==null)
			{
				createLodSubDivision(triangle, 0);
				
				//doublesChecker.Add(triangle, (byte)1);
				
				if(isLodCreated)
				{
					yield return new WaitForEndOfFrame();
					counter=0;
				}
				
				counter++;
				
				//if(!isLodCreated)
				//	break;
			}
		}
	}

	void createLodSubDivision(int triangle, float timeToWait)
	{

		if (doublesChecker [triangle] != null)
			return;
		
		//terrain

		GameObject go = new GameObject();
		Mesh mesh = go.AddComponent<MeshFilter>().mesh;
		
		mesh.vertices = new Vector3[] {oldMesh.vertices[oldMesh.triangles[0 + 3 * triangle]], oldMesh.vertices[oldMesh.triangles[1 + 3 * triangle]], oldMesh.vertices[oldMesh.triangles[2 + 3 * triangle]]};
		mesh.uv = new Vector2[] {oldMesh.uv[oldMesh.triangles[0 + 3 * triangle]], oldMesh.uv[oldMesh.triangles[1 + 3 * triangle]], oldMesh.uv[oldMesh.triangles[2 + 3 * triangle]]};
		mesh.triangles = new int[] {0, 1, 2};
		mesh.normals = new Vector3[] {Vector3.zero, Vector3.zero, Vector3.zero};
		mesh.RecalculateNormals();

		//if(level<maxLevel)
			MeshHelper.Subdivide(mesh, subdivision);
		//else
		//	MeshHelper.Subdivide(mesh, 2);


		//water
		GameObject water=null;

		bool seaEnabled;
		if (!isIsland) 
			seaEnabled = planet.enableSea;
		else
			seaEnabled = Islands.enableSea;

		if (seaEnabled) {
			water = new GameObject ();
			MeshFilter waterMesh = water.AddComponent<MeshFilter> ();

			waterMesh.mesh.vertices = mesh.vertices;
			waterMesh.mesh.uv = mesh.uv;
			waterMesh.mesh.triangles = mesh.triangles;
			waterMesh.mesh.normals = mesh.normals;

			MeshRenderer myWaterRenderer = water.AddComponent<MeshRenderer> ();

			if (!isIsland) 
			{
				renderer.material.SetTextureScale ("_Foam", new Vector2 (80 * level * level, 80 * level * level));
				myWaterRenderer.material = planet.sea.renderer.material;
			}
			else
				myWaterRenderer.material = Islands.waterMat;

			myWaterRenderer.material.shader = Shader.Find("MrNothing's Shaders/Sea With Fog");
				
			water.transform.position = transform.position;
			water.transform.rotation = transform.rotation;
			water.transform.parent = transform;
			water.name = "WaterLod_" + triangle;
			water.layer = childsLayer;

			/*if (!isIsland) 
				StartCoroutine (planet.perlinPlanet (waterMesh, 0));
			else
				StartCoroutine (Islands.perlinPlanet (waterMesh, 0));*/
		}

		//clouds
		GameObject cloud = null;
		/*cloud = new GameObject ();
		MeshFilter cloudMesh = cloud.AddComponent<MeshFilter> ();
		
		cloudMesh.mesh.vertices = mesh.vertices;
		cloudMesh.mesh.uv = mesh.uv;
		cloudMesh.mesh.triangles = mesh.triangles;
		
		MeshRenderer myCloudRenderer = cloud.AddComponent<MeshRenderer> ();
		
		if (!isIsland) 
		{
			myCloudRenderer.material = planet.clouds.renderer.material;
		}


		cloud.transform.position = transform.position;
		cloud.transform.rotation = transform.rotation;
		cloud.transform.parent = transform;
		cloud.name = "CloudLod_" + triangle;
		cloud.layer = childsLayer;*/
		
		//lod	
		MeshRenderer myRenderer = go.AddComponent<MeshRenderer>();
		myRenderer.materials = renderer.materials;

		go.transform.position = transform.position;
		go.transform.rotation = transform.rotation;
		go.transform.parent = transform;
		go.name = "Lod"+childsLayer+"_"+triangle;
		go.layer = childsLayer;
		
		myRenderer.enabled = false;
		
		MeshLodTris lodManager = go.AddComponent<MeshLodTris>();
		lodManager.triangle = triangle;
		lodManager.level = level+1;
		lodManager.maxLevel = maxLevel;
		lodManager.mainCamera = mainCamera;
		lodManager.activationDistance = activationDistance;
		lodManager.area = area/6;
		lodManager.mainCamera = mainCamera;
		lodManager.planet = planet;
		lodManager.Islands = Islands;
		lodManager.isIsland = isIsland;
		lodManager.desiredAlpha = 0;
		lodManager.GLOBAL_STEP = GLOBAL_STEP/3;
		lodManager.fading = fading/2;

		lodManager.destroyWhenInvisible = true;
		lodManager.waterTile = water;
		lodManager.cloudTile = cloud;
		
		doublesChecker.Add (triangle, true);

		lod.Add(lodManager);

		//yield return new WaitForSeconds (timeToWait);
	}
	
	

	public float desiredAlpha = 1;
	float alpha = 0;

	[HideInInspector]
	public bool destroyWhenInvisible = false;


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

							distance = Vector3.Distance(localCameraPos, oldMesh.vertices[vertice]);

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

	IEnumerator IndexTriangles(Mesh mesh, float step)
	{
		GlobalCore.loadingQueue += mesh.triangles.Length/3;
		
		Hashtable doubleChecker = new Hashtable ();
		Hashtable indexedTris = new Hashtable ();
		for (int triangle=0; triangle<mesh.triangles.Length/3; triangle++) 
		{
			Vector3 t1_pos = mesh.vertices[mesh.triangles[0 + 3 * triangle]];
			Vector3 t2_pos = mesh.vertices[mesh.triangles[1 + 3 * triangle]];
			Vector3 t3_pos = mesh.vertices[mesh.triangles[2 + 3 * triangle]];
			
			Vector3 barycenter = (t1_pos+t2_pos+t3_pos)/3;
			
			if(doubleChecker[barycenter.ToString()]==null)
			{
				string tris_id = MeshHelper.flatten(barycenter, step).ToString();
				
				List<int> trisInBloc;
				if(indexedTris[tris_id]==null)
				{
					trisInBloc = new List<int>();
					trisInBloc.Add(triangle);
					indexedTris.Add(tris_id, trisInBloc);
				}
				else
				{
					trisInBloc = (List<int>) indexedTris[tris_id];
					trisInBloc.Add(triangle);
					indexedTris[tris_id] = trisInBloc;
				}
				
				//doubleChecker.Add(barycenter.ToString(), true);
			}

			GlobalCore.loadingQueue--;

			if(triangle%20==0)
				yield return new WaitForEndOfFrame();
		}
		
		indexedTriangles = indexedTris;
		OnMeshIndexed ();
	}

}
