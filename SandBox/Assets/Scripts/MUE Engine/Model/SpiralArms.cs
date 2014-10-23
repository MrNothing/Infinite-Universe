using UnityEngine;
using System.Collections;

public class SpiralArms : MonoBehaviour 
{
    public GameObject particle;
    public float length = Mathf.PI/2;
    public float step = Mathf.PI/10f;
    public Vector3 scale;
    public float b = 1;
    public float c = 1;
    public float d = 0;
    public float e = 1;
	// Use this for initialization
    public bool debug = false;
    public int arms = 5;
    public float noiseScale = 1;

	public Material lineMaterial;

	void CreateLineMaterial() {
		if( !lineMaterial ) {
			lineMaterial = new Material( "Shader \"Lines/Colored Blended\" {" +
			                            "SubShader {Fog {Mode Off} Pass { " +
			                            "    Blend SrcAlpha OneMinusSrcAlpha " +
			                            "    ZWrite Off Cull Off Fog { Mode Off } " +
			                            "    BindChannels {" +
			                            "      Bind \"vertex\", vertex Bind \"color\", color }" +
			                            "} } }" );
			lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
		}
	}

    public int count = 0;
    public float particleSize = 1;
	public float spaceWarp = 2;
	public Vector3 universeOffset = Vector3.zero;
    void OnPostRender()
    {
		CreateLineMaterial ();
		lineMaterial.SetPass(0);
        GL.Begin(GL.TRIANGLES);
        count = 0;
        for (float i = 0; i < arms; i++)
        {
            //exp(t)*cos(t)
            //exp(t)*sin(t)
            for (float t = 0; t < length; t += step)
            {
                float x = Mathf.Exp(t * b + d) * Mathf.Cos(t * c + i * e) * scale.x;
                float y = Mathf.Exp(t * b + d) * Mathf.Sin(t * c + i * e) * scale.y;

                float noise2 = Mathf.PerlinNoise(x * noiseScale * t, y * noiseScale * t);
                float noise = Mathf.PerlinNoise(x * noise2 * noiseScale * t, y * noise2 * noiseScale * t);

                noise -= 0.5f;

                float fact = (Mathf.Abs(noise) / 0.5f);

				Vector3 p1 = new Vector3(x, noise * scale.z * ((1 - t / length) * fact / diminution), y)-Camera.main.transform.position/spaceWarp+universeOffset;
                
				float ratio = (Vector3.Distance(p1, Camera.main.transform.position)/scale.x-0.5f)*2;

				GL.Color(new Color(ratio, ratio*(1f+Mathf.Abs(Mathf.Sin(x/(scale.x))/2)), ratio*(1f)*(1f+Mathf.Abs(Mathf.Sin(x/(scale.x)))), 1 - t / length)*(1-ratio/2));
                GL.Vertex3(p1.x, p1.y, p1.z);
                GL.Vertex3(p1.x, p1.y + 0.01f * particleSize, p1.z);
                GL.Vertex3(p1.x, p1.y, p1.z - 0.01f * particleSize);

				GL.Vertex3(p1.x, p1.y, p1.z);
				GL.Vertex3(p1.x+ 0.01f * particleSize, p1.y, p1.z);
				GL.Vertex3(p1.x, p1.y, p1.z - 0.01f * particleSize);
				count++;
			}
		}

        GL.End();
    }

    public float diminution = 1;
	void Update () {
        if (debug)
        {
            for (float i = 0; i < arms; i++)
            {
                //exp(t)*cos(t)
                //exp(t)*sin(t)
                for (float t = 0; t < length; t += step)
                {
                    float x = Mathf.Exp(t * b + d) * Mathf.Cos(t * c + i*e) * 2;
                    float y = Mathf.Exp(t * b + d) * Mathf.Sin(t * c + i*e) * 2;

                    float noise2 = Mathf.PerlinNoise(x * noiseScale*t, y * noiseScale*t);
                    float noise = Mathf.PerlinNoise(x * noise2 * noiseScale * t, y * noise2 * noiseScale * t);

                    noise -= 0.5f;

                    float fact = (Mathf.Abs(noise) / 0.5f);

                    Vector3 p1 = new Vector3(x, noise * 10 * ((1 - t / length) * fact / diminution), y);
                    //GameObject p = (GameObject)Instantiate(particle, new Vector3(x, 0, y), Quaternion.Euler(0,0,0));
                    Debug.DrawLine(transform.position + p1, transform.position + p1 + new Vector3(0, 0.1f, 0), new Color(1, 1, 1, 1 - t / length));
                }
            }
            //draw = false;
        }
	}
}
