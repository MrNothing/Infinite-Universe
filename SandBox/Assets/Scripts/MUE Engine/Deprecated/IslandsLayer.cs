using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IslandsLayer : MonoBehaviour {
	
	//planet parameters
	public Transform sun;
	
	public float radius;
	
	public NoiseMethod method;
	public PerlinConfig[] perlins;
	
	MeshFilter filter;
	Vector3[] initVertices;
	bool init=false;

	SimplexNoiseGenerator perlin;
	
	public float scale = 1;
	// Use this for initialization
	void Start () {
		
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

		initVertices = filter.mesh.vertices;
		//Invoke("perlinPlanet", 1);
		//Invoke("initClouds", 2);
		
		perlin = new SimplexNoiseGenerator("azedaz");
		
		lastTestPos =transform.position;
	}
	
	[HideInInspector]
	public bool initialized = false;
	
	//public Transform test;
	bool draw=false;
	bool enableGui=false;
	
	//public Transform rotate;
	GameObject blocsContainer=null;
	
	public MeshFilter sea;
	public Material waterMat;
	
	public bool enableSea = false;
	
	bool enableBlocs=false;
	Hashtable blocsByPolarCoords = new Hashtable();
	Vector3 lastTestPos = Vector3.zero;
	
	public float skyDistance = 80;
	
	
	// Update is called once per frame
	void Update () {

		try
		{
			waterMat.SetVector("_LightDir", (sun.transform.position-transform.position).normalized);
		}
		catch
		{
		}
	}

	public NoiseInfos getFragment(Vector3 point)
	{
		Vector3 testPoint = (point-transform.position).normalized*radius;
		
		NoiseInfos infos = new NoiseInfos();
		infos.point = (point-transform.position).normalized*radius;
		
		if(method==NoiseMethod.Plains)
		{
			for (int i=0; i<perlins.Length; i++) 
			{
				
				//if(infos.noises[i]<-0.1)
				//	infos.noises[i] = -0.1f;
				float perlinres = perlins[i].getNoise(point, perlin);
				
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

	public void perlinPlanet()
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
			
			
			colors[i] = new Color(rFact, gFact, bFact, aFact);	
			
			//colors[i] = new Color(noise.noise1*color1+noise.noise2*color2, noise.noise1*color1+noise.noise2*color2, noise.noise1*color1+noise.noise2*color2, 1);	
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
			
			colors[i] = new Color(rFact, gFact, bFact, aFact);	
			
			//if(i%2==0 && Planet.rangeFactor(rFact, 0.5f, 0.25f)>0 && script.level==script.maxLevel && details.Length>0 && enableDetails)
			//	addDetail(origin, i, vertices[i], (int)(Planet.rangeFactor(rFact, 0.5f, 0.25f)*details.Length-1));
			
			if(perlinEffects>seaLevel && i%2==0 && Planet.rangeFactor(bFact, 0f, 0.1f)>0 && script.level==script.maxLevel && details.Length>0 && enableDetails)
				addDetail(origin, i, vertices[i], (int)(Planet.rangeFactor(gFact, 0f, 0.1f)*details.Length-1));
			
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
	
	public float seaLevel = -0.2f;
	public float seaHeight = -40;
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
			
			float perlinEffects = 0;
			
			float range=0.02f;
			
			if(method==NoiseMethod.Plains)
			{
				
				for(int k=0; k<perlins.Length; k++)
				{
					perlinEffects+=(perlins[k].height/totalHeight*noise.noises[k]);
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
			
			
			if(perlinEffects<seaLevel)
			{
				colors[i] = new Color(0, 0, 0, 1-Mathf.Abs(seaLevel-perlinEffects)*10);	
				vertices[i] = noise.point.normalized*radius*(1+seaLevel/(5.9f+6f*(radius/641f-1)))+noise.point.normalized*(1-(perlinEffects-seaLevel)*seaHeight);
			}
			else
			{
				colors[i] = new Color(0, 0, 0, 1);	
				vertices[i] = noise.point.normalized*radius*(1+seaLevel/(5.9f+6f*(radius/641f-1)))+noise.point.normalized*(1+(perlinEffects-seaLevel)*seaHeight);
				
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
	
	public GameObject[] details;
	
	void addDetail(MeshFilter filter, int vertice, Vector3 verticePos, int detail)
	{
		if (detail < 0)
			detail = 0;
		if (detail > details.Length - 1)
			detail = details.Length - 1;
		
		GameObject obj = (GameObject)Instantiate(details[detail], getFragment(filter.transform.TransformPoint(verticePos)+new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f))).point, Quaternion.identity);
		obj.transform.localScale *= Random.Range(0.7f, 1.3f); 
		obj.transform.parent = filter.transform;
		
		obj.name = detail+"";
	}
}
