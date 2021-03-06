using UnityEngine;
using System.Collections;
using System;

public class skydomeScript2 : MonoBehaviour {
    
    
    public Light sunLight;
    public GameObject SkyDome;
	public GameObject stars;
	public GameObject[] clouds;
	
    Camera cam;
    public Sun sunlightScript;
    public Light moon;
	public Transform moonShaft;
	public GlobalFog globalFog;
	SunShafts sunShafts;
	//public SpecularLighting oceanSpecular;
	//public Bloom bloom;
	//public TiltShift tiltShift;
	
    public bool debug = false;
	public bool realTime = true;

    public float JULIANDATE = 150;
    public float LONGITUDE = 0.0f;
    public float LATITUDE = 0.0f;
    public float MERIDIAN = 0.0f;
    public float TIME = 8.0f;
	public float timeSpeed=0f;
    public float m_fTurbidity = 2.0f;

    public float cloudSpeed1 = 1.0f;
    public float cloudSpeed2 = 1.5f;
    public float cloudHeight1 = 12.0f;
    public float cloudHeight2 = 13.0f;
    public float cloudTint = 1.0f;
	
	public Color nightColor = Color.blue;

    Vector4 vSunColourIntensity = new Vector4(1f, 1f, 1f, 100);
    Vector4 vBetaRayleigh = new Vector4();
    Vector4 vBetaMie = new Vector4();
    Vector3 m_vBetaRayTheta = new Vector3();
    Vector3 m_vBetaMieTheta = new Vector3();
    
    public float m_fRayFactor = 1000.0f;
	public float m_fMieFactor =  0.7f;
    public float m_fDirectionalityFactor = 0.6f;
    public float m_fSunColorIntensity = 1.0f;
	
	public float ratio = 0;
	
	public Core connector;
	
	Color[] initialCloudColors;
	void Start () {
        //sunlightScript = sunLight.GetComponent(typeof(Sun)) as Sun;
		connector = (Core)(GameObject.Find("Core")).GetComponent("Core");
		//oceanSpecular.specularLight = sunLight.transform;
		
		initialCloudColors = new Color[clouds.Length];
		
		for(int i=0; i<clouds.Length; i++)
		{
			Color cloudColor = clouds[i].renderer.material.GetColor("_TintColor");
			initialCloudColors[i] = new Color(cloudColor.r, cloudColor.g, cloudColor.b, cloudColor.a);
		}
	}
    void Update()
    {
		/*try
		{
			if((transform.position-connector.player.transform.position).magnitude>200)
			{
				Vector3 tmpPos = connector.player.transform.position;
				tmpPos.y = 0;
				transform.position = tmpPos;
			}
		}
		catch(Exception e)
		{
			stars.renderer.enabled = false;
			return;
		}*/
		
		transform.up = connector.player.transform.up;
		
		
		if(TIME<12)
		{
			ratio = TIME/12f;
		}
		else
		{
			ratio = (24-TIME)/12f;
		}
		
		if(ratio>1)
			ratio=1;
		
		
		float dayIntenseFactor = ratio*2;
		if(dayIntenseFactor>1)
			dayIntenseFactor = 1;
		
		//sunLight.intensity = dayIntenseFactor*0.70f;
		
		float nightFactor = (1-ratio*2);
		if(nightFactor<=0)
			nightFactor=0;
		
		if(nightFactor>1)
			nightFactor = 1;
		
		float eveningFator = (1-(ratio-0.05f)*2f);
		if(eveningFator<=0)
			eveningFator=0;
		
		if(eveningFator>1)
			eveningFator = 1;
		
		float tmpR = eveningFator*2;
		
		if(tmpR>0.5f)
			tmpR = 0.5f;
		
		float tmpG = eveningFator*4f;
		
		if(tmpG>0.5f)
			tmpG = 0.5f;
		
		float tmpB = eveningFator*10f;
		
		if(tmpB>0.5f)
			tmpB = 0.5f;
		
		try
		{
			if(nightFactor-0.1f>0)
			{
				stars.renderer.enabled = true;
				stars.renderer.material.SetColor("_Color",  new Color(nightFactor-0.1f, nightFactor-0.1f, nightFactor-0.1f));
			}
			else
			{
				stars.renderer.enabled = false;
			}
			
			for(int i=0;i<clouds.Length;i++)
			{
				Color initialColor = initialCloudColors[i];
				GameObject myCloud = clouds[i];
				myCloud.renderer.material.SetColor("_TintColor",  new Color(initialColor.r-tmpR, initialColor.g-tmpG, initialColor.b-tmpB, initialColor.a));
			}
		}
		catch(Exception e)
		{
			
		}
		
		m_fRayFactor = 250+(1f-ratio)*250;
		if(m_fRayFactor>300)
			m_fRayFactor = 300;
		
		m_fDirectionalityFactor = ratio;
		if(m_fDirectionalityFactor<0.6f)
			m_fDirectionalityFactor = 0.6f;
		
		
		m_fMieFactor = 0;
		
		if(TIME>17 && TIME<18)
			m_fMieFactor = (TIME-17f/1f)*0.2f;
		
		if(TIME>=18 && TIME<18.2f)
			m_fMieFactor = 18.2f-TIME;
		
		
		//globalFog.globalDensity = nightFactor;
		//globalFog.globalFogColor = RenderSettings.fogColor;
		
		//oceanSpecular.waterBase.sharedMaterial.SetColor("_SpecularColor", sunShafts.sunColor);
		//oceanSpecular.waterBase.sharedMaterial.SetColor("_ReflectionColor", RenderSettings.fogColor);
		
		
		DateTime currentTime = DateTime.Now;
		
		if(realTime)
		TIME = ((float)currentTime.Hour)+((float)currentTime.Minute)/60f+((float)(currentTime.Millisecond)/1000f)/(60f*60f)+timeSpeed;
		
		try
		{
        	calcAtmosphere();
		}
		catch
		{
			
		}
		
		if (Input.GetKeyDown(KeyCode.Backspace))
        {
            debug = !debug;
        }
		
		try
		{
			//day bloom: 0.49f;
			//night bloom: 0.49f;
			
			float extremeNightFactor = nightFactor*5;
			if(extremeNightFactor>1)
				extremeNightFactor = 1;
			
			//moon.intensity = 0.45f*extremeNightFactor;
			
			//bloom.bloomIntensity = 3f-nightFactor*1.5f;
			//tiltShift.smoothness = 9+nightFactor*1000;
			if(nightFactor<=0)
			{
				//RenderSettings.ambientLight = new Color(0.7f+ratio*0.15f, 0.7f+ratio*0.15f+nightFactor/8, 0.5f+ratio*0.15f+nightFactor/4);
				//RenderSettings.fogColor = new Color(0.05f+ratio/2, 0.05f+ratio/2, 0.05f+ratio/2);
				//RenderSettings.fogColor = new Color(RenderSettings.ambientLight.r-0.1f, RenderSettings.ambientLight.g-0.1f, RenderSettings.ambientLight.b-0.1f);
				
				//bloom.bloomThreshhold = 0.85f;
				sunLight.enabled = true;
				moon.enabled = false;
				//sunShafts.sunTransform = sunLight.transform;
				
				//sunShafts.sunColor = new Color(RenderSettings.ambientLight.r+0.2f, RenderSettings.ambientLight.g+0.2f, RenderSettings.ambientLight.b+0.2f);
				
			}
			else
			{
				//bloom.bloomThreshhold = 0.85f-0.40f*extremeNightFactor;
				sunLight.enabled = false;
				moon.enabled = true;
				//sunShafts.sunTransform = moonShaft;
				
				//RenderSettings.ambientLight = new Color((0.7f+ratio*0.15f)*(1-extremeNightFactor)+nightColor.r*extremeNightFactor, (0.7f+ratio*0.15f)*(1-extremeNightFactor)+nightColor.g*extremeNightFactor, (0.5f+ratio*0.15f)*(1-extremeNightFactor)+nightColor.b*extremeNightFactor);
				//RenderSettings.fogColor = new Color(0.05f+ratio/2, 0.05f+ratio/2, 0.05f+ratio/2);
				//RenderSettings.fogColor = new Color(RenderSettings.ambientLight.r-0.1f, RenderSettings.ambientLight.g-0.1f, RenderSettings.ambientLight.b-0.1f);
				
				//oceanSpecular.specularLight = moon.transform;
				//sunShafts.sunColor = moon.color;
			}
		}
		catch(Exception e1)
		{
			
		}
		
		
        Vector3 sunLightD = sunLight.transform.TransformDirection(-Vector3.forward);
		Vector3 pos;
		try
		{
        	pos = cam.transform.position;
		}
		catch(Exception e)
		{
			cam = ((GameObject) GameObject.Find("Main Camera")).camera;
			pos = cam.transform.position;
		}
		
		try
		{
	        SkyDome.renderer.material.SetVector("vBetaRayleigh", vBetaRayleigh);
	        SkyDome.renderer.material.SetVector("BetaRayTheta", m_vBetaRayTheta);
	        SkyDome.renderer.material.SetVector("vBetaMie", vBetaMie);                     
	        SkyDome.renderer.material.SetVector("BetaMieTheta", m_vBetaMieTheta);
	        SkyDome.renderer.material.SetVector("g_vEyePt",  pos);
	        SkyDome.renderer.material.SetVector("LightDir", sunLightD);
	        SkyDome.renderer.material.SetVector("g_vSunColor", sunlightScript.m_vColor);
	        SkyDome.renderer.material.SetFloat("DirectionalityFactor", m_fDirectionalityFactor);
	        SkyDome.renderer.material.SetFloat("SunColorIntensity", m_fSunColorIntensity);
	        SkyDome.renderer.material.SetFloat("tint", cloudTint);
	        SkyDome.renderer.material.SetFloat("cloudSpeed1", cloudSpeed1);
	        SkyDome.renderer.material.SetFloat("cloudSpeed2", cloudSpeed2);
	        SkyDome.renderer.material.SetFloat("plane_height1", cloudHeight1);
	        SkyDome.renderer.material.SetFloat("plane_height2", cloudHeight2);
		}
		catch(Exception e)
		{
			
		}
	}
    void calcAtmosphere()
    {
        calcRay();
        CalculateMieCoeff();
    }
    void calcRay()
    {
	    const float n  = 1.00029f;		//Refraction index for air
	    const float N  = 2.545e25f;		//Molecules per unit volume
	    const float pn = 0.035f;		//Depolarization factor

        float fRayleighFactor = m_fRayFactor * (Mathf.Pow(Mathf.PI, 2.0f) * Mathf.Pow(n * n - 1.0f, 2.0f) * (6 + 3 * pn)) / ( N * ( 6 - 7 * pn ) );
        
	    m_vBetaRayTheta.x = fRayleighFactor / ( 2.0f * Mathf.Pow( 650.0e-9f, 4.0f ) );
	    m_vBetaRayTheta.y = fRayleighFactor / ( 2.0f * Mathf.Pow( 570.0e-9f, 4.0f ) );
	    m_vBetaRayTheta.z = fRayleighFactor / ( 2.0f * Mathf.Pow( 475.0e-9f, 4.0f ) );

        vBetaRayleigh.x = 8.0f * fRayleighFactor / (3.0f * Mathf.Pow(650.0e-9f, 4.0f));
        vBetaRayleigh.y = 8.0f * fRayleighFactor / (3.0f * Mathf.Pow(570.0e-9f, 4.0f));
        vBetaRayleigh.z = 8.0f * fRayleighFactor / (3.0f * Mathf.Pow(475.0e-9f, 4.0f));
    }
    void CalculateMieCoeff()
    {
        float[] K =new float[3];
        K[0]=0.685f;  
        K[1]=0.679f;
        K[2]=0.670f;

	    float c = ( 0.6544f * m_fTurbidity - 0.6510f ) * 1e-16f;	//Concentration factor

	    float fMieFactor = m_fMieFactor * 0.434f * c * 4.0f * Mathf.PI * Mathf.PI;

	    m_vBetaMieTheta.x = fMieFactor / ( 2.0f * Mathf.Pow( 650e-9f, 2.0f ) );
	    m_vBetaMieTheta.y = fMieFactor / ( 2.0f * Mathf.Pow( 570e-9f, 2.0f ) );
	    m_vBetaMieTheta.z = fMieFactor / ( 2.0f * Mathf.Pow( 475e-9f, 2.0f ) );

        vBetaMie.x = K[0] * fMieFactor / Mathf.Pow(650e-9f, 2.0f);
        vBetaMie.y = K[1] * fMieFactor / Mathf.Pow(570e-9f, 2.0f);
        vBetaMie.z = K[2] * fMieFactor / Mathf.Pow(475e-9f, 2.0f);

        float fTemp3 = 0.434f * c * (float)Mathf.PI * (2.0f * (float)Mathf.PI) * (2.0f * (float)Mathf.PI);
        // not sure if above is correct, but it looks good.

        vBetaMie *= fTemp3;
    }
    void OnGUI()
    {
        if (debug)
        {
            GUILayout.BeginArea(new Rect(0, 10, 600, 400));
            GUILayout.BeginHorizontal();
            GUILayout.Label("Time : " + TIME.ToString(), GUILayout.Width(200));
            TIME = GUILayout.HorizontalSlider(TIME, 0f, 23f);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("SunColorIntensity : " + m_fSunColorIntensity.ToString(), GUILayout.Width(200));
            m_fSunColorIntensity = GUILayout.HorizontalSlider(m_fSunColorIntensity, 0, 2f);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("DirectionalityFactor : " + m_fDirectionalityFactor.ToString(), GUILayout.Width(200));
            m_fDirectionalityFactor = GUILayout.HorizontalSlider(m_fDirectionalityFactor, 0f, 1f);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Rayleigh multiplier : " + m_fRayFactor.ToString(), GUILayout.Width(200));
            m_fRayFactor = GUILayout.HorizontalSlider(m_fRayFactor, 0f, 10000f);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Mie multiplier : " + m_fMieFactor.ToString(), GUILayout.Width(200));
            m_fMieFactor = GUILayout.HorizontalSlider(m_fMieFactor, 0f, 5f);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("cloudTint : " + cloudTint.ToString(), GUILayout.Width(200));
            cloudTint = GUILayout.HorizontalSlider(cloudTint, 0f, 2f);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("cloudSpeed1 : " + cloudSpeed1.ToString(), GUILayout.Width(200));
            cloudSpeed1 = GUILayout.HorizontalSlider(cloudSpeed1, 0f, 6f);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("cloudSpeed2 : " + cloudSpeed2.ToString(), GUILayout.Width(200));
            cloudSpeed2 = GUILayout.HorizontalSlider(cloudSpeed2, 0f, 6f);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("cloudHeight1 : " + cloudHeight1.ToString(), GUILayout.Width(200));
            cloudHeight1 = GUILayout.HorizontalSlider(cloudHeight1, 10f, 20f);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("cloudHeight2 : " + cloudHeight2.ToString(), GUILayout.Width(200));
            cloudHeight2 = GUILayout.HorizontalSlider(cloudHeight2, 10f, 20f);
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }
        
    }
}
