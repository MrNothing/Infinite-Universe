using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MrNothingOptimizedParticles : MonoBehaviour {

	public GameObject particle;
    public Vector3 grid = new Vector3(10, 5, 10);
    public float perlinScale = 2;
	public Vector3 perlinOffset = Vector3.zero;
	public float initParticleScale = 1;
	float particleScale = 1;
    public float shadowIntensity = 0.5f;
	// Use this for initialization

	int status=0;

	IEnumerator Start () 
	{
		int counter = 0;
        SimplexNoiseGenerator simplex = new SimplexNoiseGenerator("yolo");
		
		List<MeshCombineUtility.MeshInstance> combine = new List<MeshCombineUtility.MeshInstance>();
		
		MeshFilter myFilter = particle.GetComponent<MeshFilter>();
		particle.renderer.material.SetVector("_Scale", grid);
		particle.transform.position = Vector3.zero;
		/*for (int u=0; u<myFilter.mesh.vertices.Length; u++) {
			myFilter.mesh.vertices[u]*=initParticleScale;
		}*/

		myFilter.mesh.RecalculateNormals ();
		myFilter.mesh.RecalculateBounds ();

		float gridScale = (grid.x + grid.y + grid.z);

        for (float i = -grid.x; i < grid.x; i++)
        {
            for (float j = -grid.y; j < grid.y; j++)
            {
                for (float k = -grid.z; k < grid.z; k++)
                {
					status++;
                    Vector3 pos = transform.position + new Vector3(i * particleScale, j * particleScale, k * particleScale);
					float simplexV = simplex.coherentNoise(pos.x * perlinScale+perlinOffset.x, pos.y * perlinScale+perlinOffset.y, pos.z * perlinScale+perlinOffset.z);
                    // Debug.Log(Mathf.PerlinNoise(pos.x * perlinScale, pos.y * perlinScale + pos.z * perlinScale));
                    if (simplexV > 0.04 * (grid.y - j) / grid.y && simplexV > 0.04 * (grid.x - (i)) / grid.x && simplexV > 0.04 * (grid.z - (k)) / grid.z)
                    {
                        
						//GameObject go = (GameObject)Instantiate(particle);
						//go.renderer.material.SetFloat("_ShadowIntensity", Random.Range(shadowIntensity, shadowIntensity+0.05f));
                        //go.renderer.material.SetFloat("_ParticleSize", Random.Range(0.15f / particleScale, 0.30f / particleScale));
                        //go.renderer.material.SetFloat("_Scale", grid.x);
                        //go.transform.position = transform.position+new Vector3(i * particleScale, j * particleScale, k * particleScale);
						
						//set vertex Colors for a Quad
						Color[] colors = new Color[myFilter.mesh.vertices.Length];
						Vector3[] vertices = new Vector3[myFilter.mesh.vertices.Length];
						
						for(int u=0; u<colors.Length; u++)
						{
							colors[u] = new Color(0.5f+i/grid.x*0.5f, 0.5f+j/grid.y*0.5f, 0.5f+k/grid.z*0.5f, Random.Range(shadowIntensity, shadowIntensity+0.05f));
							vertices[u] = new Vector3(i * particleScale, j * particleScale, k * particleScale)+myFilter.mesh.vertices[u]*Random.Range(initParticleScale*0.5f, initParticleScale);
						}
						//go.GetComponent<MeshFilter>().mesh.colors = colors;
						

						Mesh myMesh = new Mesh();
						myMesh.vertices = vertices;
						myMesh.triangles = myFilter.mesh.triangles;
						myMesh.colors = colors;
						myMesh.uv = myFilter.mesh.uv;
						
						myMesh.RecalculateNormals();
						myMesh.RecalculateBounds();
						
						//myFilter.transform.position = transform.position+new Vector3(i * particleScale, j * particleScale, k * particleScale);
						//myFilter.mesh.RecalculateNormals();
						//myFilter.mesh.RecalculateBounds();
						
                        //go.transform.localScale = Vector3.one*Random.Range(1,1.3f);
                        //go.transform.parent = transform;
						
						MeshCombineUtility.MeshInstance C = new MeshCombineUtility.MeshInstance();
						C.mesh = myMesh;
						C.transform  = myFilter.transform.localToWorldMatrix;
						
						combine.Add(C);
						
                        counter++;
                        if (counter >= 50)
                        {
                            yield return new WaitForEndOfFrame();
                            counter = 0;
							//break;
                        }
                    }
                }
            }
        }

		if (GlobalCore.currentPlanet != null) {
			particle.transform.up = (GlobalCore.currentPlanet.getNormalFromPoint(transform.position));
		}

		Vector3 sunDir = (transform.position - GlobalCore.sun.transform.position).normalized;
		particle.renderer.material.SetVector ("_LightDir", new Vector3 (-sunDir.x, -sunDir.y, sunDir.z));

		Color cloudColor = GlobalCore.currentPlanet.getAtmosphereColor (transform.position);
		if(cloudColor.r<0)
			cloudColor.r = 0;
		if(cloudColor.g<0)
			cloudColor.g = 0;
		if(cloudColor.b<0)
			cloudColor.b = 0;

		particle.renderer.material.SetColor ("_Tint", cloudColor+new Color(0.2f, 0.2f, 0.2f, 1));

		myFilter.mesh = MeshCombineUtility.Combine(combine.ToArray(), true);

		particle.transform.position = transform.position;
	}
	

}
