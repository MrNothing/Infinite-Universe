using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum AsteroidFieldShape
{
	cube, circle
}

public class AsteroidField:MonoBehaviour{

	public Camera mainCamera;

	public Material AsteroidMaterial;

	public float range = 100;
	public int amount = 10;
	public float fieldSize;
	public float minHeight;
	public float maxHeight;

	public float perlinScale = 10;

	bool initialized = false;
	public List<GameObject> asteroids = new List<GameObject>();
		
	public AsteroidFieldShape shape = AsteroidFieldShape.circle;
	public float step;
	float distance;
	void Start()
	{
		step = (Mathf.PI * 2f) / ((float)amount);
	}
	void Update()
	{
		distance = Vector3.Distance(transform.position, mainCamera.transform.position);
		if(distance<range)
		{
			if(!initialized)
			{
				StartCoroutine(CreateField());

				initialized = true;
			}
		}
		/*else
		{
			if(initialized)
			{
				//destroy stuff here...
				foreach(GameObject m in asteroids)
				{
					try
					{
						Destroy(m.renderer.GetComponent<MeshFilter>().mesh);}catch
					{}	
					try
					{
						Destroy(m.renderer.material);}catch
					{}	
					try
					{
						Destroy(m.gameObject);}catch
					{}	
				}
				initialized = false;
			}
		}*/
	}

	IEnumerator CreateField()
	{
		if(shape==AsteroidFieldShape.cube)
		{
			//generate stuff here...
			for(int i=0; i<amount; i++)
			{
				PerlinConfig p = new PerlinConfig();
				p.height = Random.Range(minHeight, maxHeight);
				if(p.height>minHeight+(maxHeight-minHeight)/2)
					p.height*=5;
				p.scale = perlinScale/(p.height/10);
				GameObject g = generateAsteroid(p, 2, AsteroidMaterial);
				g.transform.parent = transform;
				g.transform.localPosition = new Vector3(Random.Range(-fieldSize, fieldSize), Random.Range(-fieldSize, fieldSize), Random.Range(-fieldSize, fieldSize));
				g.transform.Rotate(Random.Range(-180, 180), Random.Range(-180, 180), Random.Range(-180, 180));
				yield return new WaitForEndOfFrame();
			}
		}
		else
		{
			int counter = 0;
			for(float i=0; i<Mathf.PI*2; i+=step)
			{
				PerlinConfig p = new PerlinConfig();
				p.height = Random.Range(minHeight, maxHeight);
				if(p.height>minHeight+(maxHeight-minHeight)*0.9)
					p.height*=0.5f;
				p.scale = perlinScale/(p.height/10);
				GameObject g = generateAsteroid(p, 2, AsteroidMaterial);
				g.transform.parent = transform;
				g.transform.localPosition = new Vector3(fieldSize*Mathf.Sin(i)+Random.Range(-fieldSize/10, fieldSize/10), Random.Range(-fieldSize/100, fieldSize/100), fieldSize*Mathf.Cos(i)+Random.Range(-fieldSize/10, fieldSize/10));
				g.transform.Rotate(Random.Range(-180, 180), Random.Range(-180, 180), Random.Range(-180, 180));
			}
		}
		yield return new WaitForEndOfFrame();
	}

	public static GameObject generateAsteroid(PerlinConfig noiseData, int precision, Material mat)
	{
		GameObject asteroid = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		asteroid.renderer.material = mat;
		MeshFilter filter = asteroid.GetComponent<MeshFilter> ();
		if(precision>1)
		{
			MeshHelper.Subdivide(filter.mesh, precision);
		}

		SimplexNoiseGenerator simplex = new SimplexNoiseGenerator ();

		Vector3[] vertices = new Vector3[filter.mesh.vertices.Length];
		Color[] colors = new Color[filter.mesh.vertices.Length];

		filter.mesh.vertices.CopyTo(vertices, 0);

		for(int i=0; i<vertices.Length; i++)
		{
			float perlinEffects = noiseData.getNoise(filter.transform.TransformPoint(vertices[i]*noiseData.height), simplex);
			PerlinConfig noiseData2 = new PerlinConfig(); 
			noiseData2.scale*=noiseData.scale*1.1f;
			noiseData2.height*=noiseData.height;
			float perlinEffects2 = noiseData2.getNoise(filter.transform.TransformPoint(vertices[i]*noiseData.height), simplex);

			vertices[i] = (filter.transform.TransformPoint(vertices[i]*noiseData.height)-filter.transform.position).normalized*noiseData.height*2+(filter.transform.TransformPoint(vertices[i]*noiseData.height)-filter.transform.position).normalized*noiseData.height*(1+perlinEffects*10);
			vertices[i] *= 1-Mathf.Abs(perlinEffects2);

			colors[i] = new Color(Planet.rangeFactor(perlinEffects-perlinEffects2, 0, 0.01f), 1-Planet.rangeFactor(perlinEffects-perlinEffects2, 0, 0.01f), 0,0);
		}

		filter.mesh.vertices = vertices;
		filter.mesh.colors = colors;
		filter.mesh.RecalculateBounds();
		filter.mesh.RecalculateNormals();

		return asteroid;
	}
}
