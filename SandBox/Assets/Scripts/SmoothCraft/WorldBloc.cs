using UnityEngine;
using System.Collections;

public enum WorldBlockType
{
	sphere, cube, perlin, planet, none
}

public class WorldBloc : MonoBehaviour 
{
	public WorldBlockType type;
	
	public float height = 4;
	public int size = 3;
	
	public Transform player;
	
	public int optimizationLvl=0;
	
	public bool clamp = false;
	
	Core core;
	// Use this for initialization
	void Start () 
	{
		player = ((GameObject)GameObject.Find("Player")).transform;
		core = ((GameObject)GameObject.Find("Core")).GetComponent<Core>();
		/*for (int i = -size; i<size; i++) 
		{
			for (int j = 0; j>-height; j--) 
			{
				for (int k = -size; k<size; k++) 
				{
					if(j==0)
						Instantiate(Resources.Load("SphereGrass", typeof(GameObject)), new Vector3(i, j, k), Quaternion.identity);
					else
						Instantiate(Resources.Load("SphereDirt", typeof(GameObject)), new Vector3(i, j, k), Quaternion.identity);
				}
			}
		}*/
		
		if(type==WorldBlockType.sphere)
			StartCoroutine(createPlanet(height));
		
		if(type==WorldBlockType.cube)
			StartCoroutine(createCube());
		
		if(type==WorldBlockType.perlin)
		{
			noiseTex = new Texture2D(size, size);
			pix = new Color[noiseTex.width * noiseTex.height];
			
			generatePerlin();
			
			StartCoroutine(CreateWorldWithPerlin());
		}
		
		if(type==WorldBlockType.planet)
		{
			
		}
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(type!=WorldBlockType.perlin)
			optimizationLvl = (int)(Vector3.Distance(transform.position, player.position)/5f);
		
		if(type==WorldBlockType.planet)
			drawPlanetChunk(player.transform.position);
	}
	
	public float perlinScale;
	public Vector3 perlinOrigin;
	
	Texture2D noiseTex;
	Color[] pix;
	
	void generatePerlin()
	{
		// For each pixel in the texture...
		for (int y = 0; y < noiseTex.height; y++) {
			for (int x = 0; x < noiseTex.width; x++) {
				// Get a sample from the corresponding position in the noise plane
				// and create a greyscale pixel from it.
				float xCoord = perlinOrigin.x + ((float)(x)) / noiseTex.width * perlinScale;
				float yCoord = perlinOrigin.y + ((float)(y)) / noiseTex.height * perlinScale;
				float sample = Mathf.PerlinNoise(xCoord, yCoord);
				pix[y * noiseTex.width + x] = new Color(sample, sample, sample);
			}
		}
		
		// Copy the pixel data to the texture and load it into the GPU.
		noiseTex.SetPixels(pix);
		noiseTex.Apply();
	}
	
	public float getHeightAt(Vector3 pos)
	{
		Vector3 localPos = pos - transform.position;
		
		int xIndex = (int)Mathf.Floor(localPos.x);
		int zIndex = (int)Mathf.Floor(localPos.z);
		
		float heightRatio = noiseTex.GetPixel(xIndex, zIndex).r;
		
		if(heightRatio<0.3f)
			heightRatio = 0.3f;
		
		return heightRatio*height;
	}
	
	IEnumerator CreateWorldWithPerlin()
	{
		for (int y = 0; y < noiseTex.height; y++) 
		{
			if(optimizationLvl>=1)
				yield return new WaitForEndOfFrame();
			
			for (int x = 0; x < noiseTex.width; x++) 
			{
				
				float heightRatio = noiseTex.GetPixel(x, y).r;
				
				if(heightRatio<0.3f)
					heightRatio = 0.3f;
				
				string prefab = "";
					
				bool addTree = false;
				prefab = "SphereGrass";
				
				if(Random.Range(0, size*2)<=1 && heightRatio>0.5f)
				{
					addTree = true;
				}
				
				if(heightRatio>0.6f)
				{
					prefab = "SphereSnow";
				}
				
				
				if(MeshIndexer.CountSurroundingMeshes(transform.position+new Vector3(x, heightRatio*height, y), 1)==0)
				{
					GameObject bloc = (GameObject)Instantiate(core.spheresByName[prefab], transform.position+new Vector3(x, heightRatio*height, y), Quaternion.identity);
					bloc.transform.parent = transform;
					bloc.GetComponent<MeshBlender>().wait = Vector3.Distance(bloc.transform.position, player.position)*5;
					bloc.GetComponent<MeshBlender>().isTerrainBloc = true;
					MeshIndexer.IndexMesh(bloc.GetComponent<MeshBlender>());
					
					if(clamp)
					{
						//if(x==0 || x==1 || x==noiseTex.width-1 || x==noiseTex.width || y==0 || y==1 || y==noiseTex.height-1 || y==noiseTex.height)
						//	bloc.GetComponent<MeshBlender>().forceNearbyRefresh = true;
					}
					if(addTree)
					{
						GameObject tree = (GameObject)Instantiate(core.spheresByName["Tree1"], transform.position+new Vector3(x, heightRatio*height+0.5f, y), Quaternion.identity);
						tree.transform.parent = transform;
						tree.transform.Rotate(Random.Range(-15, 15), Random.Range(0, 360), 0);
						tree.transform.localScale*=Random.Range(0.7f, 1.3f);
						tree.GetComponent<MeshBlender>().wait = Vector3.Distance(tree.transform.position, player.position)*6;
						MeshIndexer.IndexMesh(tree.GetComponent<MeshBlender>());
					}
						
				}
			}
		}
		yield return new WaitForEndOfFrame();
	}
	
	IEnumerator createCube ()
	{
		for (int i = -size; i<size; i++) 
		{
			for (int j = 0; j>-height; j--) 
			{
				for (int k = -size; k<size; k++) 
				{
					string prefab = "";
					
					bool addTree = false;
					if(j==0)
					{
						prefab = "SphereGrass";
						if(Random.Range(0, size*2)<=1)
						{
							addTree = true;
						}
					}
					else
						if(j<-height/2)
							prefab = "SphereStone";
						else
							prefab = "SphereDirt";
					
					if(MeshIndexer.CountSurroundingMeshes(transform.position+new Vector3(i, j, k), 1)==0)
					{
						GameObject bloc = (GameObject)Instantiate(core.spheresByName[prefab], transform.position+new Vector3(i, j, k), Quaternion.identity);
						bloc.transform.parent = transform;
						bloc.GetComponent<MeshBlender>().wait = Vector3.Distance(bloc.transform.position, player.position)*5;
						MeshIndexer.IndexMesh(bloc.GetComponent<MeshBlender>());
						
						if(addTree)
						{
							GameObject tree = (GameObject)Instantiate(core.spheresByName["Tree1"], transform.position+new Vector3(i, j+0.5f, k), Quaternion.identity);
							tree.transform.parent = transform;
							tree.transform.Rotate(Random.Range(-15, 15), Random.Range(0, 360), 0);
							tree.transform.localScale*=Random.Range(0.7f, 1.3f);
							tree.GetComponent<MeshBlender>().wait = Vector3.Distance(tree.transform.position, player.position)*6;
							MeshIndexer.IndexMesh(tree.GetComponent<MeshBlender>());
						}
					if(optimizationLvl>=3)
						yield return new WaitForFixedUpdate();
					}
					
					if(optimizationLvl==2)
						yield return new WaitForFixedUpdate();
				}
				
				if(optimizationLvl==1)
					yield return new WaitForFixedUpdate();
			}
			
		}
		
		enabled = false;
	}
	
	IEnumerator createPlanet(float radius)
	{
		for (float z=-radius; z<=radius; z++) 
		{
			for (float y=-radius; y<=0; y++)
			{
				for (float x=-radius; x<=radius; x++)
				{
					if (x * x + y * y + z * z < radius * radius) 
					{
						string prefab = "";
						
						bool addTree = false;
						if(y==0)
						{
							prefab = "SphereGrass";
							if(Random.Range(0, radius*2)<=1)
							{
								addTree = true;
							}
						}
						else
							if(y<-radius/2)
								prefab = "SphereStone";
							else
								prefab = "SphereDirt";
						
						if(MeshIndexer.CountSurroundingMeshes(transform.position+new Vector3(x, y, z), 1)==0)
						{
							GameObject bloc = (GameObject)Instantiate(core.spheresByName[prefab], transform.position+new Vector3(x, y, z), Quaternion.identity);
							bloc.transform.parent = transform;
							bloc.GetComponent<MeshBlender>().wait = Vector3.Distance(bloc.transform.position, player.position)*5;
							MeshIndexer.IndexMesh(bloc.GetComponent<MeshBlender>());
							
							if(addTree)
							{
								GameObject tree = (GameObject)Instantiate(core.spheresByName["Tree1"], transform.position+new Vector3(x, y+0.5f, z), Quaternion.identity);
								tree.transform.parent = transform;
								tree.transform.Rotate(Random.Range(-15, 15), Random.Range(0, 360), 0);
								tree.transform.localScale*=Random.Range(0.7f, 1.3f);
								tree.GetComponent<MeshBlender>().wait = Vector3.Distance(tree.transform.position, player.position)*6;
								MeshIndexer.IndexMesh(tree.GetComponent<MeshBlender>());
							}
						}
					}
					if(optimizationLvl>=3)
						yield return new WaitForFixedUpdate();
				}
				
				if(optimizationLvl==2)
					yield return new WaitForFixedUpdate();
			}
			
			if(optimizationLvl==1)
				yield return new WaitForFixedUpdate();
		}
		
		enabled = false;
	}
	
	public void drawPlanetChunk(Vector3 point)
	{
		Vector3 testPoint = (point-transform.position).normalized*960;
		
		Debug.DrawLine(transform.position, transform.position+testPoint);
	}
}
