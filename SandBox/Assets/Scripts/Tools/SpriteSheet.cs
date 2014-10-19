using System;
using UnityEngine;

public class SpriteSheet : MonoBehaviour
{
 	public int frames = 24;
	public bool isX = false;
	public int id = 0;
	
	float ix = 0f;
    void Update()
    {
		ix+=1f/frames;
		
		if(isX)
        renderer.materials[id].SetTextureOffset ("_MainTex", new Vector2(ix, 0f));
		else
		renderer.materials[id].SetTextureOffset ("_MainTex", new Vector2(0f, ix));
		
		/*try{
			if(isX)
	        renderer.materials[id].SetTextureOffset ("_BumpMap", new Vector2(ix, 0f));
			else
			renderer.materials[id].SetTextureOffset ("_BumpMap", new Vector2(0f, ix));
		}catch(Exception e){}*/
		if(ix>1)
			ix = 0f;
    }
}