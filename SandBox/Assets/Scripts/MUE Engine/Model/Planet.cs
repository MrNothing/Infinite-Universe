using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum NoiseMethod
{
	Plains, Deserts, Moutains
}

/*
 * In a planet, positions are indexed using polar coordinates. Other than that it is the same as a planar universe 
 * */

public class NoiseInfos
{
	public Vector3 point;

	public List<float> noises = new List<float>();

	public float totalNoise=0;
	public float totalHeight=0;
	//public float noise2;
}

[System.Serializable]
public class PerlinConfig
{
	public string name;
	public float scale = 10;
	public Vector3 offset = Vector3.zero;
	public float height = 1;
	public float min = 0;
	public float max = 1;

	public float getNoise(Vector3 testPoint, SimplexNoiseGenerator perlin)
	{
		return perlin.coherentNoise (testPoint.x * scale + offset.x, testPoint.y * scale + offset.y, testPoint.z * scale + offset.z, 1, 25, 0.5f, 2f, 0.9f);
	}
}

public class Planet : MonoBehaviour {

	public static int isAvaliable=0;

	//planet parameters
	public Transform sun;
	
	public float radius;
	
	public NoiseMethod method;
	public PerlinConfig[] perlins;

	/*public Vector2 noiseOffset;
	public Vector2 noiseOffset2;
	public float perlinScale=1;
	public float perlinScale2=1;
	public float perlinHeight=1;
	public float perlinHeight2=1;*/
	
	
	public static Dictionary<string, List<MeshBlender>> squareBasedWorldIndex = new Dictionary<string, List<MeshBlender>>();
	public static Dictionary<string, MeshBlender> index = new Dictionary<string, MeshBlender>();
	
	public static Hashtable dugZones=new Hashtable();
	
	MeshFilter filter;
	Vector3[] initVertices;
	bool init=false;

	public bool hasFog = true;
	
	SimplexNoiseGenerator perlin;
	
	public float scale = 1;

	public Vector2 lodTilingScale = new Vector2(80,80);
	// Use this for initialization
	void Start () 
	{
		//renderer.material.shader = Shader.Find("MrNothing's Shaders/Vertex Layered Terrain No Z");

		testcamera = GameObject.FindObjectOfType<PlanetCharacterController> ().mainCamera.camera;

		filter = GetComponent<MeshFilter>();

		//scale mesh...
		Vector3[] vertices = new Vector3[filter.mesh.vertices.Length];
		filter.mesh.vertices.CopyTo(vertices, 0);
		
		for (int i=0; i<vertices.Length; i++) 
		{
			vertices[i]*=scale;
		}
		filter.mesh.vertices = vertices;

		radius = Vector3.Distance(transform.TransformPoint(filter.mesh.vertices[0]), transform.position);

		//Debug.Log (name+": " + transform.TransformPoint (filter.mesh.vertices [0]));

		initVertices = filter.mesh.vertices;
		//Invoke("perlinPlanet", 1);
		//Invoke("initClouds", 2);

		perlin = new SimplexNoiseGenerator("azedaz");
		
		lastTestPos =transform.position;

		try
		{
			skyDome.GetComponent<AthmosphereScript>().sun = sun;
			Color skyColor = skyDome.renderer.material.GetColor("_Color");
			if(skyColor.r+skyColor.g+skyColor.b>=3)
			{
				skyColor.r = 0;
				skyColor.g = 0.5f;
				skyColor.b = 1;
				skyColor.a = 0.35f;
			}
			else
			{
				if(skyColor.r+skyColor.g+skyColor.b<=1.5f)
				{
					skyColor.r*=1.5f;
					skyColor.g*=1.5f;
					skyColor.b*=1.5f;
				}
				skyColor.a = 0.35f;
			}
			sea.renderer.material.SetColor("_Tint", skyColor);
		}
		catch
		{
				
		}

		try
		{
			skyDome.transform.localScale = Vector3.one*(radius+90);
			Color atmosphereColor = skyDome.renderer.material.GetColor("_Color");
			atmosphereColor.a = 0.5f;
			skyDome.renderer.material.SetColor("_Color", atmosphereColor);
			skyDome.GetComponent<AthmosphereScript>().sun = sun;
			clouds.transform.localScale = Vector3.one*(radius+85);	
			clouds.GetComponent<AthmosphereScript>().sun = sun;
			sea.transform.localScale = Vector3.one;
			sea.GetComponent<AthmosphereScript>().sun = sun;
		}
		catch
		{
				
		}

		filter.mesh.RecalculateNormals ();
		initClouds();
	}

	public bool isIsland = false;

	[HideInInspector]
	public bool initialized = false;

	//public Transform test;
	bool draw=false;
	bool enableGui=false;

	[HideInInspector]
	public Camera testcamera;
	//public Transform rotate;
	GameObject blocsContainer=null;
	
	public GameObject skyDome;
	public MeshFilter clouds;
	public MeshFilter sea;

	public bool enableSea = false;
	
	bool enableBlocs=false;
	Hashtable blocsByPolarCoords = new Hashtable();
	Vector3 lastTestPos = Vector3.zero;
	
	public float skyDistance = 80;
	
	
	// Update is called once per frame
	void Update () 
	{
		sea.renderer.material.SetVector("_LightDir", (sun.transform.position-transform.position).normalized);
		try
		{
			Vector3 point;
			if(!isIsland)
				point = getFragment(testcamera.transform.position).point;
			else
				point = getIslandFragment(testcamera.transform.position);
				

			float ratio = Vector3.Distance(point, testcamera.transform.position)/skyDistance;

			//Debug.DrawLine(transform.position, transform.position+point);

			if(ratio<1)
			{
				//skyDome.renderer.material.SetColor("_Color", new Color(1, 1, 1, 1-ratio/2));
				
				/*if(Vector3.Distance(lastTestPos, test.transform.position)>20 && ratio<0.3)
				{ 
					if(blocsContainer==null)
					{
						blocsContainer=new GameObject();
						blocsContainer.transform.position = transform.position;
						blocsContainer.name=name+"_container";
					}
					StartCoroutine(makeBlocGround());
					lastTestPos = test.transform.position;
				}*/
			}
			else
			{
				//skyDome.renderer.material.SetColor("_Color", new Color(1, 1, 1, 0.5f));
				if(blocsContainer!=null)
				{
					Destroy(blocsContainer);
					blocsContainer = null;
					MeshIndexer.Clear();
				}
			}
		}
		catch
		{
			
		}
	}
	
	public Vector3 getPointFromTangentPlane(Vector3 origin, Vector2 coords)
	{
		Vector3 point = getFragment(origin).point;
		Vector3 direction = point-transform.position;

		Vector3 v = new Vector3(direction.y, -direction.x, 0);
		Vector3 k = Vector3.Cross(v,direction);
			
		v = v.normalized;
		k = k.normalized;
		
		return new Vector3(point.x + coords.x*v.x + coords.y*k.x, point.y + coords.x*v.y + coords.y*k.y, point.z + coords.x*v.z + coords.y*k.z);
	}
	
	public NoiseInfos getFragment(Vector3 point)
	{
		Vector3 testPoint = transform.position+(point-transform.position).normalized*radius;

		NoiseInfos infos = new NoiseInfos();
		infos.point = (point-transform.position).normalized*radius;
		
		if(method==NoiseMethod.Plains)
		{
			for (int i=0; i<perlins.Length; i++) 
			{
				
				//if(infos.noises[i]<-0.1)
				//	infos.noises[i] = -0.1f;
				float perlinres = perlins[i].getNoise(testPoint, perlin);

				if(perlinres<0f)
					perlinres*=10;

				if(perlinres<-0.2f)
					perlinres = -0.2f;

				if(perlinres>0.05f)
					perlinres+=(perlinres-0.05f)*Mathf.PerlinNoise(testPoint.x/100, testPoint.y/100)*50;

				if(perlinres>1f)
					perlinres-=(perlinres-1f);
				//if(i>0)
				{
					//float factor = (infos.noises[0])*10;
					
					//if(factor<0.1)
					//	factor=factor*0.5f;
					
					//perlinres = perlins[i].getNoise(testPoint, perlin)*Mathf.PerlinNoise(factor, factor);
				}
				
				infos.noises.Add(perlinres);
				infos.totalNoise+=infos.noises[i];
				infos.totalHeight+=perlins[i].height;
				infos.point+=(point-transform.position).normalized*infos.noises[i]*perlins[i].height;
			}
		}
		else if(method==NoiseMethod.Deserts)
		{
			Vector3 tmpPoint = infos.point+perlins[0].offset;
			
			PolarPoint p = PolarPoint.getPolarCoordinates(transform.InverseTransformPoint(tmpPoint), radius);
			
			tmpPoint.x=p.phi;
			tmpPoint.z=p.theta;
			
			float proceduralScale = Mathf.PerlinNoise(tmpPoint.x*perlins[0].scale, tmpPoint.z*perlins[0].scale);
			
			//Debug.Log("p: "+p.phi+" - "+p.theta);
			
			float largeProceduralScale = Mathf.PerlinNoise(tmpPoint.x*perlins[1].scale, tmpPoint.z*perlins[1].scale);
			float proceduralHeight = Mathf.PerlinNoise(tmpPoint.x*perlins[2].scale*largeProceduralScale, tmpPoint.z*perlins[2].scale*largeProceduralScale);
			proceduralScale-=0.5f;
			proceduralScale*=3;
			proceduralHeight-=0.5f;
			proceduralHeight*=3;
			
			float method1 = proceduralScale;
			float method2 = Mathf.PerlinNoise(Mathf.Sin(tmpPoint.x*proceduralScale*perlins[3].scale), Mathf.Sin(tmpPoint.z*proceduralScale*perlins[3].scale));
			method2-=0.5f;
			method2*=3;
		
			
			float method3 = Mathf.PerlinNoise(tmpPoint.x*proceduralScale*perlins[3].scale, tmpPoint.z*proceduralScale*perlins[3].scale);
			method3-=0.5f;
			method3*=3;
		
			float methodDefiner = Mathf.PerlinNoise(tmpPoint.x*perlins[4].scale, tmpPoint.z*perlins[4].scale);
			methodDefiner-=0.5f;
			methodDefiner*=3;
			
			if(methodDefiner>1)
				methodDefiner=1;
			if(methodDefiner<-1)
				methodDefiner=-1;
			
			infos.noises.Add((method1*Planet.rangeFactor(methodDefiner, -1.5f, 0.5f)+method2*Planet.rangeFactor(methodDefiner, -0.5f, 0.5f)+method3*Planet.rangeFactor(methodDefiner, 0.5f, 0.5f))*proceduralHeight);
			infos.point+=(point-transform.position).normalized*infos.noises[0]*perlins[0].height*proceduralHeight;
		}
		else
		{
			Vector3 tmpPoint = infos.point+perlins[0].offset;
			
			//tmpPoint.x=0;
			//tmpPoint.z=0;
			
			/*if(tmpPoint.y!=0)
				tmpPoint.x = tmpPoint.x/tmpPoint.y;
			
			if(tmpPoint.x!=0)
				tmpPoint.y = tmpPoint.z/tmpPoint.x;
			
			if(tmpPoint.z!=0)
				tmpPoint.z = tmpPoint.y/tmpPoint.z;*/
			
			float proceduralScale = perlins[0].getNoise(tmpPoint, perlin)*10;
			float largeProceduralScale = perlins[1].getNoise(tmpPoint, perlin)*10;
			float proceduralHeight = perlins[2].getNoise(tmpPoint*largeProceduralScale, perlin)*10;
			
			float method1 = proceduralScale;
			
			//Vector3 tmpPoint2 = new Vector3(Mathf.Sin(tmpPoint.x), Mathf.Sin(tmpPoint.y), Mathf.Sin(tmpPoint.z));
			
			float method2 = perlins[3].getNoise(tmpPoint*proceduralScale, perlin)*10;
	
			float method3 = perlins[3].getNoise(tmpPoint*proceduralScale, perlin)*10;
	
		
			float methodDefiner = perlins[4].getNoise(tmpPoint, perlin)*10;
			
			if(methodDefiner>1)
				methodDefiner=1;
			if(methodDefiner<-1)
				methodDefiner=-1;
			
			infos.noises.Add((method1*Planet.rangeFactor(methodDefiner, -0.5f, 0.5f)+method2*Planet.rangeFactor(methodDefiner, -0f, 0.5f)+method3*Planet.rangeFactor(methodDefiner, 0.5f, 0.5f))*proceduralHeight);
			infos.point+=(point-transform.position).normalized*infos.noises[0]*perlins[0].height*proceduralHeight;
		}
		/*float noise = perlin.coherentNoise(testPoint.x*perlinScale+noiseOffset.x, testPoint.y*perlinScale+noiseOffset.y, testPoint.z*perlinScale, 1, 25, 0.5f, 2f, 0.9f);
		float noise2 = perlin.coherentNoise(testPoint.x*perlinScale2+noiseOffset2.x, testPoint.y*perlinScale2+noiseOffset2.y, testPoint.z*perlinScale2, 1, 25, 0.5f, 2f, 0.9f);
		
			
		Vector3 localPlanetPoint = (point-transform.position).normalized*noise*perlinHeight;
		Vector3 localPlanetPoint2 = (point-transform.position).normalized*noise2*perlinHeight2;
		
	

		infos.noise1 = noise;
		infos.noise2 = noise2;
		
		if(infos.noise1<-0.2)
		{
			infos.noise1=-0.2f;
			infos.noise2=0;
		}
		
		infos.point =transform.position+(point-transform.position).normalized*radius+localPlanetPoint+localPlanetPoint2/2;*/
		infos.point *= transform.localScale.x;
	return infos;
	}
	
	public string perlinDebug = "";

	int asyncInit=0;
	void initClouds()
	{
		//MeshHelper.Subdivide(clouds.mesh, 8);

		cloudVertices = new Vector3[clouds.mesh.vertices.Length];
		cloudColors = new Color[clouds.mesh.vertices.Length];

		perlinClouds (0, clouds.mesh.vertices.Length);
	}

	Vector3[] cloudVertices;
	Color[] cloudColors;
	void perlinClouds(int start, int end)
	{
		if (start < 0)
			start = 0;

		if (end > clouds.mesh.vertices.Length)
			end = clouds.mesh.vertices.Length;

		float totalHeight = 0;
		foreach(PerlinConfig p in perlins)
		totalHeight+=p.height;

		for (int i=start; i<end; i++) 
		{
		
			NoiseInfos noise = getFragment (transform.TransformPoint (clouds.mesh.vertices [i]));
			float perlinEffects = 0;
			
			float range = 0.02f;
			
			for(int k=0; k<perlins.Length; k++)
			{
				perlinEffects+=perlins[k].height/totalHeight*noise.noises[k];
			}

			perlinEffects+=0.1f;
			perlinEffects*=2;
			
			range = 0.1f;

			float rFact = Planet.rangeFactor(perlinEffects, 0, range);
			
			if(perlinEffects>range && perlinEffects<0.1f-range)
				rFact = 1;
			
			float gFact = Planet.rangeFactor(perlinEffects, 0.1f, range);
			
			if(perlinEffects>0.1f+range && perlinEffects<0.2f-range)
				gFact = 1;
			
			float bFact = Planet.rangeFactor(perlinEffects, 0.2f, range);
			
			if(perlinEffects>0.2f+range && perlinEffects<0.3f-range)
				bFact = 1;
			
			float aFact = Planet.rangeFactor(perlinEffects, 0.3f, range);
			
			if(perlinEffects>0.3f+range)
				aFact = 1;
			
			if(bFact<0)
				bFact = 0;
			
			if(aFact<0)
				aFact = 0;
			
			if(rFact<0)
				rFact = 0;
			
			if(gFact<0)
				gFact = 0;
			
			cloudColors[i] = new Color(rFact, gFact, bFact, aFact);	
		}

		asyncInit--;
		clouds.mesh.colors = cloudColors;
		clouds.mesh.RecalculateBounds ();
		clouds.mesh.RecalculateNormals ();
	}

	public IEnumerator perlinPlanet()
	{
		GlobalCore.loadingQueue += filter.mesh.vertices.Length;
		//Planet.isAvaliable += 1;
		if(radius<=1)
			radius = 1;
		
		float step=0.1f;
		
		Vector3[] vertices = new Vector3[filter.mesh.vertices.Length];
		Vector3[] seaVertices = new Vector3[filter.mesh.vertices.Length];
		Color[] colors = new Color[filter.mesh.vertices.Length];
		Color[] seaColors = new Color[filter.mesh.vertices.Length];
		
		filter.mesh.vertices.CopyTo(vertices, 0);
		
		float minPerlin=10;
		float maxPerlin=0;

		float totalHeight = 0;

		foreach(PerlinConfig p in perlins)
			totalHeight+=p.height;

		for(int i=0; i<vertices.Length; i++)
		{
	
			NoiseInfos noise = getFragment(transform.TransformPoint(vertices[i]));

			if((vertices[i].normalized*radius).magnitude+seaLevel>noise.point.magnitude)
				seaColors[i] = new Color(0, 0, 0, 1-Mathf.Abs(((vertices[i].normalized*radius).magnitude+seaLevel)-noise.point.magnitude)/totalHeight*foam);	
			else
				seaColors[i] = new Color(0, 0, 0, 1);	

			seaVertices[i] = noise.point.normalized*radius+noise.point.normalized*(seaLevel);

			vertices[i] = noise.point;

			float perlinOffset=perlin.coherentNoise(noise.point.x*1.5f, noise.point.y*1.5f, noise.point.z*1.5f);
			
			colors[i] = new Color((((vertices[i].normalized*radius).magnitude)-noise.point.magnitude)/totalHeight+0.5f+perlinOffset/10, 0, 0, 0);	
			
			//colors[i] = new Color(noise.noise1*color1+noise.noise2*color2, noise.noise1*color1+noise.noise2*color2, noise.noise1*color1+noise.noise2*color2, 1);	

			GlobalCore.loadingQueue--;

			if(i%1000==0)
				yield return new WaitForEndOfFrame();
		}
		
		//perlinDebug = "min: "+minPerlin+" max: "+maxPerlin;
		
		filter.mesh.colors = colors;
		filter.mesh.vertices = vertices;
		filter.mesh.RecalculateBounds();
		filter.mesh.RecalculateNormals();

		/*
		for(polarCoordinates.x=0; polarCoordinates.x<Mathf.PI; polarCoordinates.x+=step/2)
		{	
			for(polarCoordinates.y=0; polarCoordinates.y<Mathf.PI*2; polarCoordinates.y+=step/2)
			{
				PolarPoint tmpPolarCoordinates = new PolarPoint(polarCoordinates.x, polarCoordinates.y, radius);
				Vector3 worldPosition = getPlanetPointFromPolarCoords(tmpPolarCoordinates); 
				Vector3 localPoint = getLocalPlanetPointFromPolarCoords(tmpPolarCoordinates);
				Debug.DrawLine(worldPosition*0.999f, worldPosition, Color.white);
			}
		}
		*/

		if (enableSea)
		{
			sea.mesh.vertices = seaVertices;
			sea.mesh.triangles = filter.mesh.triangles;
			sea.mesh.uv = filter.mesh.uv;
			sea.mesh.normals = filter.mesh.normals;
			sea.mesh.colors = seaColors;
		}
		initialized = true;
	}
	
	public IEnumerator perlinPlanet(MeshFilter origin, MeshLodTris script, bool enableDetails)
	{
		if(radius<=1)
			radius = 1;
		
		float step=0.1f;
		
		Vector3[] vertices = new Vector3[origin.mesh.vertices.Length];
		Color[] colors = new Color[origin.mesh.vertices.Length];
		
		origin.mesh.vertices.CopyTo(vertices, 0);

		float currentTime = Time.time;

		int treeCounter = 0;
		
		for(int i=0; i<vertices.Length; i++)
		{
	
			NoiseInfos noise = getFragment(transform.TransformPoint(vertices[i]));

			vertices[i] = noise.point;
			
			float perlinEffects = 0;
			float totalHeight = 0;
			
			float range=0.02f;
			
			if(method==NoiseMethod.Plains)
			{
				foreach(PerlinConfig p in perlins)
					totalHeight+=p.height;
				
				for(int k=0; k<perlins.Length; k++)
				{
					perlinEffects+=perlins[k].height/totalHeight*noise.noises[k];
				}
				
				perlinEffects+=0.1f;
				perlinEffects*=2;
				
				range = 0.1f;
				
				if(perlinEffects>1)
					perlinEffects=1;
				if(perlinEffects<-1)
					perlinEffects=-1;
			}
			else
			{
				perlinEffects = noise.noises[0];
			}
			
			
			float rFact = Planet.rangeFactor(perlinEffects, 0, range);
			
			if(perlinEffects>range && perlinEffects<0.1f-range)
				rFact = 1;
			
			float gFact = Planet.rangeFactor(perlinEffects, 0.1f, range);
			
			if(perlinEffects>0.1f+range && perlinEffects<0.2f-range)
				gFact = 1;
			
			float bFact = Planet.rangeFactor(perlinEffects, 0.2f, range);
			
			if(perlinEffects>0.2f+range && perlinEffects<0.3f-range)
				bFact = 1;
			
			float aFact = Planet.rangeFactor(perlinEffects, 0.3f, range);
			
			if(perlinEffects>0.3f+range)
				aFact = 1;
			
			if(bFact<0)
				bFact = 0;
			
			if(aFact<0)
				aFact = 0;
			
			if(rFact<0)
				rFact = 0;

			if(gFact<0)
				gFact = 0;

			float perlinOffset=perlin.coherentNoise(noise.point.x*1.5f, noise.point.y*1.5f, noise.point.z*1.5f);
			
			colors[i] = new Color((((vertices[i].normalized*radius).magnitude)-noise.point.magnitude)/totalHeight+0.5f+perlinOffset/10, 0, 0, 0);	

			//if(i%2==0 && Planet.rangeFactor(rFact, 0.5f, 0.25f)>0 && script.level==script.maxLevel && details.Length>0 && enableDetails)
			//	addDetail(origin, i, vertices[i], (int)(Planet.rangeFactor(rFact, 0.5f, 0.25f)*details.Length-1));

			//place details
			for(int doodadIndex = 0; doodadIndex<doodads.Length; doodadIndex++)
			{
				if(noise.point.magnitude>(vertices[i].normalized*radius).magnitude+doodads[doodadIndex].minHeight && noise.point.magnitude<(vertices[i].normalized*radius).magnitude+doodads[doodadIndex].maxHeight)
				{
					if(doodads[doodadIndex].frequency>Mathf.Abs(Mathf.PerlinNoise(vertices[i].x*1000*(doodadIndex+1), vertices[i].y*1000*(doodadIndex+1))*2))
					{
						StartCoroutine(addDoodad(origin, i, vertices[i], doodadIndex, doodads[doodadIndex].offsetY));
						colors[i].g += doodads[doodadIndex].shadow;	
						//break;
					}
				}
			}
			treeCounter++;
			//colors[i] = new Color(noise.noise1*color1+noise.noise2*color2, noise.noise1*color1+noise.noise2*color2, noise.noise1*color1+noise.noise2*color2, 1);	

			if(i%50==0)
			{
				//currentTime = Time.time;
				yield return new WaitForEndOfFrame();
			}
		}
		
		origin.mesh.colors = colors;
		origin.mesh.vertices = vertices;
		origin.mesh.RecalculateBounds();
		origin.mesh.RecalculateNormals();
		
		//if(script.level>1)
		//{
			//print("adding mesh collider...");
			//MeshCollider collider = script.gameObject.AddComponent<MeshCollider>();
			//collider.sharedMesh = origin.mesh;
		//}

		script.desiredAlpha = 1;
		
		script.renderer.enabled = true;
		
		yield return new WaitForEndOfFrame();
	}

	public float seaLevel = 0;
	public float foam = 100;
	public IEnumerator perlinPlanet(MeshFilter origin, int isWater)
	{
		if(radius<=1)
			radius = 1;
		
		float step=0.1f;
		
		Vector3[] vertices = new Vector3[origin.mesh.vertices.Length];
		Color[] colors = new Color[origin.mesh.vertices.Length];
		
		origin.mesh.vertices.CopyTo(vertices, 0);
		
		float currentTime = Time.time;
		
		float totalHeight = 0;
		float avgHeight = 0;
		foreach (PerlinConfig p in perlins) 
		{
			totalHeight += p.height;
		}

		for(int i=0; i<vertices.Length; i++)
		{
			
			NoiseInfos noise = getFragment(transform.TransformPoint(vertices[i]));

			if((vertices[i].normalized*radius).magnitude+seaLevel>noise.point.magnitude)
			{
				colors[i] = new Color(0, 0, 0, 1-Mathf.Abs(((vertices[i].normalized*radius).magnitude+seaLevel)-noise.point.magnitude)/totalHeight*foam);	

				if(colors[i].a<0.1f)
					colors[i].a = 0.1f;

				vertices[i] = noise.point.normalized*radius+noise.point.normalized*(seaLevel);
			}
			else
			{
				colors[i] = new Color(0, 0, 0, 1);	
				vertices[i] = noise.point.normalized*radius+noise.point.normalized*(seaLevel);
			}

			if(i%50==0)
			{
				//currentTime = Time.time;
				yield return new WaitForEndOfFrame();
			}
		}

		try
		{
			origin.mesh.colors = colors;
			origin.mesh.vertices = vertices;
			origin.mesh.RecalculateBounds();
			origin.mesh.RecalculateNormals();
		}
		catch
		{
		}

		yield return new WaitForEndOfFrame();
	}

	public Vector3 IslandAxis = Vector3.zero;
	public float IslandFloorLevel = 0;
	public void perlinIsland()
	{
		//Planet.isAvaliable += 1;
		if(radius<=1)
			radius = 1;
		
		float step=0.1f;
		
		Vector3[] vertices = new Vector3[filter.mesh.vertices.Length];

		Color[] colors = new Color[filter.mesh.vertices.Length];
		Color[] seaColors = new Color[filter.mesh.vertices.Length];
		
		filter.mesh.vertices.CopyTo(vertices, 0);
		
		float minPerlin=10;
		float maxPerlin=0;
		
		for(int i=0; i<vertices.Length; i++)
		{
			NoiseInfos noise = getFragment(transform.TransformPoint(vertices[i]));
			float perlinEffects = 0;
			float totalHeight = 0;
			
			float range=0.02f;
			
			if(method==NoiseMethod.Plains)
			{
				foreach(PerlinConfig p in perlins)
					totalHeight+=p.height;
				
				for(int k=0; k<perlins.Length; k++)
				{
					perlinEffects+=perlins[k].height/totalHeight*noise.noises[k];
				}
				
				if(perlinEffects<minPerlin)
					minPerlin=perlinEffects;
				
				if(perlinEffects>maxPerlin)
					maxPerlin=perlinEffects;
				
				perlinEffects+=0.1f;
				perlinEffects*=2;
				
				range = 0.1f;
				
			}
			else
			{
				perlinEffects = noise.noises[0];
			}
			
			
			
			float rFact = Planet.rangeFactor(perlinEffects, 0, range);
			
			if(perlinEffects>range && perlinEffects<0.1f-range)
				rFact = 1;
			
			float gFact = Planet.rangeFactor(perlinEffects, 0.1f, range);
			
			if(perlinEffects>0.1f+range && perlinEffects<0.2f-range)
				gFact = 1;
			
			float bFact = Planet.rangeFactor(perlinEffects, 0.2f, range);
			
			if(perlinEffects>0.2f+range && perlinEffects<0.3f-range)
				bFact = 1;
			
			float aFact = Planet.rangeFactor(perlinEffects, 0.3f, range);
			
			if(perlinEffects>0.3f+range)
				aFact = 1;
			
			if(bFact<0)
				bFact = 0;
			
			if(aFact<0)
				aFact = 0;
			
			if(rFact<0)
				rFact = 0;
			
			if(gFact<0)
				gFact = 0;
			
			if(perlinEffects<seaLevel)
				seaColors[i] = new Color(0, 0, 0, 1-Mathf.Abs(seaLevel-perlinEffects)*10);	
			else
				seaColors[i] = new Color(0, 0, 0, 1);	
			
			
			Vector3 projection = Planet.projectPointToPlane(vertices[i], IslandAxis.normalized*IslandFloorLevel, IslandAxis);

			Vector3 noiseVector = noise.point - vertices[i];
			Vector3 noiseAxisVec = IslandAxis.normalized*(noise.point.magnitude-radius);

			float factor = (Vector3.Distance(projection, Vector3.zero)/(radius/2))*(Vector3.Distance(projection, Vector3.zero)/(radius/2));
			float distFact = (2-Vector3.Distance(projection, Vector3.zero)/(radius/2));
			if(distFact<0f)
				distFact = 0f;

			float smoothFact = distFact*distFact;

			float fastFact = smoothFact*2;

			if(fastFact>1)
				fastFact = 1;

			if(smoothFact>1)
				smoothFact = 1;

			if(Vector3.Dot(IslandAxis, vertices[i])<IslandFloorLevel)
			{
				vertices[i] = projection*fastFact-IslandAxis.normalized*fastFact*radius-noiseAxisVec*10*(1-fastFact)+noiseVector*10*(fastFact)+noise.point*(1-fastFact);
				colors[i] = new Color(fastFact, gFact*(1-fastFact), bFact*(1-fastFact), aFact*(1-fastFact));	
				//colors[i] = new Color(noise.noise1*color1+noise.noise2*color2, noise.noise1*color1+noise.noise2*color2, noise.noise1*color1+noise.noise2*color2, 1);	
			}
			else
			{
				//projection de vertices[i] dans le plan ayant pour normale IslandAxis et pour origine IslandAxis.normalized()*IslandFloorLevel
				vertices[i] = projection*fastFact+IslandAxis.normalized*fastFact*(radius*0.6f+(noise.point.magnitude-radius)*2)+noise.point*(1-fastFact);
				colors[i] = new Color(rFact, gFact, bFact, fastFact);	
			}
		}
		
		//perlinDebug = "min: "+minPerlin+" max: "+maxPerlin;
		
		filter.mesh.colors = colors;
		filter.mesh.vertices = vertices;
		filter.mesh.RecalculateBounds();
		filter.mesh.RecalculateNormals();
		
		/*
		for(polarCoordinates.x=0; polarCoordinates.x<Mathf.PI; polarCoordinates.x+=step/2)
		{	
			for(polarCoordinates.y=0; polarCoordinates.y<Mathf.PI*2; polarCoordinates.y+=step/2)
			{
				PolarPoint tmpPolarCoordinates = new PolarPoint(polarCoordinates.x, polarCoordinates.y, radius);
				Vector3 worldPosition = getPlanetPointFromPolarCoords(tmpPolarCoordinates); 
				Vector3 localPoint = getLocalPlanetPointFromPolarCoords(tmpPolarCoordinates);
				Debug.DrawLine(worldPosition*0.999f, worldPosition, Color.white);
			}
		}
		*/
		
		if (enableSea)
		{
			sea.mesh.vertices = filter.mesh.vertices;
			sea.mesh.triangles = filter.mesh.triangles;
			sea.mesh.uv = filter.mesh.uv;
			sea.mesh.normals = filter.mesh.normals;
			sea.mesh.colors = seaColors;
		}
		initialized = true;

		//MeshCollider collider = this.gameObject.AddComponent<MeshCollider>();
		//collider.sharedMesh = filter.mesh;
	}

	public IEnumerator perlinIsland(MeshFilter origin, MeshLodTris script, bool enableDetails)
	{
		if(radius<=1)
			radius = 1;
		
		float step=0.1f;
		
		Vector3[] vertices = new Vector3[origin.mesh.vertices.Length];
		Color[] colors = new Color[origin.mesh.vertices.Length];
		
		origin.mesh.vertices.CopyTo(vertices, 0);
		
		float currentTime = Time.time;
		
		for(int i=0; i<vertices.Length; i++)
		{
			
			NoiseInfos noise = getFragment(transform.TransformPoint(vertices[i]));
			vertices[i] = noise.point;
			
			float perlinEffects = 0;
			float totalHeight = 0;
			
			float range=0.02f;
			
			if(method==NoiseMethod.Plains)
			{
				foreach(PerlinConfig p in perlins)
					totalHeight+=p.height;
				
				for(int k=0; k<perlins.Length; k++)
				{
					perlinEffects+=perlins[k].height/totalHeight*noise.noises[k];
				}
				
				perlinEffects+=0.1f;
				perlinEffects*=2;
				
				range = 0.1f;
				
				if(perlinEffects>1)
					perlinEffects=1;
				if(perlinEffects<-1)
					perlinEffects=-1;
			}
			else
			{
				perlinEffects = noise.noises[0];
			}
			
			
			float rFact = Planet.rangeFactor(perlinEffects, 0, range);
			
			if(perlinEffects>range && perlinEffects<0.1f-range)
				rFact = 1;
			
			float gFact = Planet.rangeFactor(perlinEffects, 0.1f, range);
			
			if(perlinEffects>0.1f+range && perlinEffects<0.2f-range)
				gFact = 1;
			
			float bFact = Planet.rangeFactor(perlinEffects, 0.2f, range);
			
			if(perlinEffects>0.2f+range && perlinEffects<0.3f-range)
				bFact = 1;
			
			float aFact = Planet.rangeFactor(perlinEffects, 0.3f, range);
			
			if(perlinEffects>0.3f+range)
				aFact = 1;
			
			if(bFact<0)
				bFact = 0;
			
			if(aFact<0)
				aFact = 0;
			
			if(rFact<0)
				rFact = 0;
			
			if(gFact<0)
				gFact = 0;
			
			Vector3 projection = Planet.projectPointToPlane(vertices[i], IslandAxis.normalized*IslandFloorLevel, IslandAxis);
			
			Vector3 noiseVector = noise.point - vertices[i];
			Vector3 noiseAxisVec = IslandAxis.normalized*(noise.point.magnitude-radius);
			
			float factor = (Vector3.Distance(projection, Vector3.zero)/(radius/2))*(Vector3.Distance(projection, Vector3.zero)/(radius/2));
			float distFact = (2-Vector3.Distance(projection, Vector3.zero)/(radius/2));
			if(distFact<0f)
				distFact = 0f;
			
			float smoothFact = distFact*distFact;
			
			float fastFact = smoothFact*2;
			
			if(fastFact>1)
				fastFact = 1;
			
			if(smoothFact>1)
				smoothFact = 1;
			
			if(Vector3.Dot(IslandAxis, vertices[i])<IslandFloorLevel)
			{
				vertices[i] = projection*fastFact-IslandAxis.normalized*fastFact*radius-noiseAxisVec*10*(1-fastFact)+noiseVector*10*(fastFact)+noise.point*(1-fastFact);
				colors[i] = new Color(fastFact, gFact*(1-fastFact), bFact*(1-fastFact), aFact*(1-fastFact));	
				//colors[i] = new Color(noise.noise1*color1+noise.noise2*color2, noise.noise1*color1+noise.noise2*color2, noise.noise1*color1+noise.noise2*color2, 1);	
			}
			else
			{
				//projection de vertices[i] dans le plan ayant pour normale IslandAxis et pour origine IslandAxis.normalized()*IslandFloorLevel
				vertices[i] = projection*fastFact+IslandAxis.normalized*fastFact*(radius*0.6f+(noise.point.magnitude-radius)*2)+noise.point*(1-fastFact);
				colors[i] = new Color(rFact, gFact, bFact, fastFact);	
			}
			
			//if(i%2==0 && Planet.rangeFactor(rFact, 0.5f, 0.25f)>0 && script.level==script.maxLevel && details.Length>0 && enableDetails)
			//	addDetail(origin, i, vertices[i], (int)(Planet.rangeFactor(rFact, 0.5f, 0.25f)*details.Length-1));
			
			//if(perlinEffects>seaLevel && i%2==0 && Planet.rangeFactor(bFact, 0f, 0.1f)>0 && script.level==script.maxLevel && details.Length>0 && enableDetails)
			//	addDetail(origin, i, vertices[i], (int)(Planet.rangeFactor(gFact, 0f, 0.1f)*details.Length-1));
			
			//colors[i] = new Color(noise.noise1*color1+noise.noise2*color2, noise.noise1*color1+noise.noise2*color2, noise.noise1*color1+noise.noise2*color2, 1);	
			
			if(i%50==0)
			{
				//currentTime = Time.time;
				yield return new WaitForEndOfFrame();
			}
		}
		
		origin.mesh.colors = colors;
		origin.mesh.vertices = vertices;
		origin.mesh.RecalculateBounds();
		origin.mesh.RecalculateNormals();

		
		script.desiredAlpha = 1;
		
		script.renderer.enabled = true;
		
		yield return new WaitForEndOfFrame();
	}

	public Vector3 getIslandFragment(Vector3 point)
	{
		NoiseInfos noise = getFragment(transform.TransformPoint(point));
        Vector3 pseudoVertice = (point-transform.position).normalized*radius;

		Vector3 projection = Planet.projectPointToPlane(pseudoVertice, IslandAxis.normalized*IslandFloorLevel, IslandAxis);
		
		Vector3 noiseVector = noise.point - pseudoVertice;
		Vector3 noiseAxisVec = IslandAxis.normalized*(noise.point.magnitude-radius);
		
		float factor = (Vector3.Distance(projection, Vector3.zero)/(radius/2))*(Vector3.Distance(projection, Vector3.zero)/(radius/2));
		float distFact = (2-Vector3.Distance(projection, Vector3.zero)/(radius/2));
		if(distFact<0f)
			distFact = 0f;
		
		float smoothFact = distFact*distFact;
		
		float fastFact = smoothFact*2;
		
		if(fastFact>1)
			fastFact = 1;
		
		if(smoothFact>1)
			smoothFact = 1;
		
		if(Vector3.Dot(IslandAxis, pseudoVertice)<IslandFloorLevel)
		{
			//return projection*fastFact-IslandAxis.normalized*fastFact*radius-noiseAxisVec*10*(1-fastFact)+noiseVector*10*(fastFact)+noise.point*(1-fastFact);
			return pseudoVertice;
		}
		else
		{
			//projection de vertices[i] dans le plan ayant pour normale IslandAxis et pour origine IslandAxis.normalized()*IslandFloorLevel
			return projection*fastFact+IslandAxis.normalized*fastFact*(radius*0.6f+(noise.point.magnitude-radius)*2)+noise.point*(1-fastFact);
			//return pseudoVertice;
		}
	}
	
	
	/*Vector3 getPlanetPointFromSpacePoint(Vector3 point)
	{
		return getPlanetPointFromPolarCoords(getPolarCoordinates(point));
	}
			
	Vector3 getPlanetPointFromPolarCoords(PolarPoint point)
	{
		Vector3 localPosition = getCartesianCoordinates(point); 
		Vector3 worldPosition = localPosition+transform.position; 
		
		float noise = Mathf.PerlinNoise(localPosition.x*perlinScale+noiseOffset.x, localPosition.y*perlinScale+noiseOffset.y);
		float noise2 = Mathf.PerlinNoise(localPosition.x*perlinScale2+noiseOffset2.x, localPosition.y*perlinScale2+noiseOffset2.y);
		
		noise = noise*noise2;		
		Vector3 localPlanetPoint = (localPosition-transform.position).normalized*noise*perlinHeight;
				
		return worldPosition - localPlanetPoint;
	}
	
	Vector3 getLocalPlanetPointFromPolarCoords(PolarPoint point)
	{
		Vector3 localPosition = getCartesianCoordinates(point); 
		Vector3 worldPosition = localPosition+transform.position; 
		
		float noise = Mathf.PerlinNoise(localPosition.x*perlinScale+noiseOffset.x, localPosition.y*perlinScale+noiseOffset.y);
		float noise2 = Mathf.PerlinNoise(localPosition.x*perlinScale2+noiseOffset2.x, localPosition.y*perlinScale2+noiseOffset2.y);
		
		noise = noise*noise2;		
		Vector3 localPlanetPoint = (localPosition-transform.position).normalized*noise*perlinHeight;
				
		return localPlanetPoint;
	}*/
	
	Vector3 getCartesianCoordinates(PolarPoint polarCoords)
	{
        float a = polarCoords.r * Mathf.Cos(polarCoords.phi);
        return new Vector3(a * Mathf.Cos(polarCoords.theta), polarCoords.r * Mathf.Sin(polarCoords.phi), a * Mathf.Sin(polarCoords.theta));
    }
 
	PolarPoint getPolarCoordinates(Vector3 worldPoint)
	{
       Vector3 cartesianCoordinate = worldPoint-transform.position;
		 if( cartesianCoordinate.x == 0f )
            cartesianCoordinate.x = Mathf.Epsilon;
        float r = cartesianCoordinate.sqrMagnitude;
 
        float theta = Mathf.Atan(cartesianCoordinate.z / cartesianCoordinate.x);
 
        if( cartesianCoordinate.x < 0f )
            theta += Mathf.PI;
		
        float phi = Mathf.Asin(cartesianCoordinate.y / radius);
 
        return new PolarPoint(theta, phi, r);
    }
	
	float getDistanceFromAngle(float angle)
	{
		Vector2 origin = new Vector2(0, radius);
		Vector2 anglePoint = new Vector2(radius * Mathf.Cos(angle), radius * Mathf.Sin(angle));
		
		return Vector2.Distance(origin, anglePoint);
	}
	
	float getAngleFromDistance(float distance)
	{
		float K = (distance*distance)/(radius*radius)-2f;
		return Mathf.Acos(K/-2f)+Mathf.PI/2;
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
	
	public Doodad[] doodads;

	IEnumerator addDoodad(MeshFilter filter, int vertice, Vector3 verticePos, int detail, float offsetY)
	{
		yield return new WaitForEndOfFrame ();
		Vector3 point = getFragment (filter.transform.TransformPoint (verticePos)).point;

		try
		{
			GameObject obj = (GameObject)Instantiate(doodads[detail].gameObject, transform.position+point+point.normalized*offsetY, Quaternion.identity);
			obj.transform.localScale *= Random.Range(doodads[detail].minScaleFactor, doodads[detail].maxScaleFactor); 
			obj.transform.parent = filter.transform;

			obj.transform.up = filter.mesh.normals[vertice];

			try
			{
				float ambientLight = Vector3.Dot((sun.transform.position-transform.position).normalized.normalized, obj.transform.up.normalized);
				if(ambientLight<0)
					ambientLight = 0;
				obj.renderer.materials [0].SetFloat ("_AmbientShadow", ambientLight/2);
				obj.renderer.materials [0].SetFloat ("_AmbientLight", ambientLight+0.25f);
				obj.renderer.materials [1].SetVector ("_LightDir", (sun.transform.position-transform.position).normalized);
				obj.renderer.materials [1].SetFloat ("_AmbientShadow", ambientLight/2);
				obj.renderer.materials [1].SetFloat ("_AmbientLight", ambientLight+0.25f);
			}
			catch
			{

			}

			obj.transform.RotateAround (obj.transform.position, filter.mesh.normals[vertice].normalized, Random.Range (-doodads[detail].maxRotation, doodads[detail].maxRotation));

			//obj.transform.Translate (obj.transform.up*0.40f);

			//obj.transform.localEulerAngles += new Vector3 (0, Random.Range (-180, 180), 0);

			obj.name = "Doodad_"+detail+"";
		}
		catch
		{
			Debug.Log("Doodad failed: "+detail);		
		}
	}

	public Color getAtmosphereColor(Vector3 pos)
	{
		Vector3 vertex = getFragment (pos).point;
		Vector3 camPos =  getFragment (testcamera.transform.position).point;
		
		float sunspecular = Vector3.Dot(camPos, (sun.transform.position-transform.position).normalized);
		float sunIntensity =  Vector3.Dot((sun.transform.position-transform.position).normalized, vertex);
		float realDistance = Vector3.Distance(camPos, transform.position);
		float camVertexRatio = Vector3.Dot(camPos, transform.position);
		
		float sunSetFact = 0;
		
		//if(sunspecular<0)
		//	sunSetFact = sunspecular;
		
		//sunspecular = abs(sunspecular);
		
		float specularCanceler = 0;
		
		float sunspecularP = 0;
		
		if(sunspecular>0.5f)
		{
			sunspecular = 0.5f;
		}
		
		if(sunspecular<-0.5f)
			sunspecular = -0.5f;
		
		if(sunspecular<0)
		{
			sunIntensity*=1;
		}
		else
			sunspecularP = sunspecular;
		
		
		Color sunColor = new Color(sunIntensity, sunIntensity+sunspecular/2, sunIntensity+sunspecular, 1);

		sunColor.g *= 0.9f;
		sunColor.b *= 0.8f;

		return sunColor/radius;
	}

	public static Vector3 projectPointToPlane(Vector3 point, Vector3 planePoint, Vector3 planeNormal)
	{
		//plane's equation...
		float t = (planeNormal.x * planePoint.x - planeNormal.x * point.x + planeNormal.y * planePoint.y - planeNormal.y * point.y + planeNormal.z * planePoint.z - planeNormal.z * point.z) / (planeNormal.x * planeNormal.x + planeNormal.y * planeNormal.y + planeNormal.z * planeNormal.z);

		return new Vector3(point.x+t*planeNormal.x, point.y+t*planeNormal.y, point.z+t*planeNormal.z);
	}

	public bool isUnderWater()
	{
		Vector3 testPoint = testcamera.transform.position-transform.position;
		if(((testPoint).normalized*radius).magnitude+seaLevel>testPoint.magnitude)
			return true;
		else
			return false;
	}
}
