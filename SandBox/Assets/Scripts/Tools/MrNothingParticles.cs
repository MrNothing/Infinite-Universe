using UnityEngine;
using System.Collections;

public class MrNothingParticles : MonoBehaviour {

    public GameObject particle;
    public Vector3 grid = new Vector3(10, 5, 10);
    public float perlinScale = 2;
    public float particleScale = 1;
    public float shadowIntensity = 0.5f;
	// Use this for initialization
	IEnumerator Start () {
        float counter = 0;
        SimplexNoiseGenerator simplex = new SimplexNoiseGenerator("yolo");

		Vector3 sunDir = (transform.position - GlobalCore.sun.transform.position).normalized;
		Color cloudColor = GlobalCore.currentPlanet.getAtmosphereColor (transform.position);
		if(cloudColor.r<0)
			cloudColor.r = 0;
		if(cloudColor.g<0)
			cloudColor.g = 0;
		if(cloudColor.b<0)
			cloudColor.b = 0;
		

        for (float i = -grid.x; i < grid.x; i++)
        {
            for (float j = -grid.y; j < grid.y; j++)
            {
                for (float k = -grid.z; k < grid.z; k++)
                {
                    Vector3 pos = transform.position + new Vector3(i * particleScale, j * particleScale, k * particleScale);
                    float simplexV = simplex.coherentNoise(pos.x * perlinScale, pos.y * perlinScale, pos.z * perlinScale);
                    // Debug.Log(Mathf.PerlinNoise(pos.x * perlinScale, pos.y * perlinScale + pos.z * perlinScale));
                    if (simplexV > 0.04 * (grid.y - j) / grid.y && simplexV > 0.04 * (grid.x - (i)) / grid.x && simplexV > 0.04 * (grid.z - (k)) / grid.z)
                    {
                        GameObject go = (GameObject)Instantiate(particle);
                        go.renderer.material.SetFloat("_ShadowIntensity", Random.Range(shadowIntensity, shadowIntensity+0.05f));
                        go.renderer.material.SetFloat("_ParticleSize", Random.Range(0.15f / particleScale, 0.30f / particleScale));
                        go.transform.position = transform.position + new Vector3(i * particleScale, j * particleScale, k * particleScale);
                        //go.transform.localScale = Vector3.one*Random.Range(1,1.3f);
                        go.transform.parent = transform;
						go.renderer.material.SetVector ("_LightDir", new Vector3 (-sunDir.x, -sunDir.y, sunDir.z));
						go.renderer.material.SetColor ("_Tint", cloudColor*2+new Color(0.2f, 0.2f, 0.2f, 1));

						counter++;
                        if (counter > grid.x)
                        {
                            yield return new WaitForEndOfFrame();
                            counter = 0;
                        }
                    }
                }
            }
        }
	}
}
