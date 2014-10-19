using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshBlender : MonoBehaviour {

	public List<MeshBlender> objects = new List<MeshBlender>();
	public float influenceRange = 2;
	public float baseStep = 1;

	MeshFilter myFilter;
	
	Vector3[] initialVertices=null;
	Color32[] colorsBuffer;
	
	bool hasToUpdateColors=false;
	public bool isWater = false;
	public bool useShader = false;
	public bool deleted = false;
	// Use this for initialization
	void Start () {
		myFilter = gameObject.GetComponent<MeshFilter> ();
		initialVertices = myFilter.mesh.vertices;

		/*MeshBlender[] tmpObjects = (MeshBlender[])GameObject.FindObjectsOfType (typeof(MeshBlender));

		foreach (MeshBlender b in tmpObjects) 
		{
			if(Vector3.Distance(transform.position, b.transform.position)<influenceRange*1.3f)
			{
				if(b.GetInstanceID()!=GetInstanceID())
					objects.Add(b);
			}
		}
		 */
		lastPosition = transform.position;
		//refreshShape();
	}
	
	public void activateWater()
	{
		if(!IsInvoking("spreadWater") && !deleted)
			InvokeRepeating("spreadWater", 0.5f, 1);
	}
	
	// Update is called once per frame
	Vector3 lastPosition = Vector3.zero;
	public float wait = 0;
	public bool forceNearbyRefresh = false;
	public bool isTerrainBloc = false;
	void Update () 
	{
		Vector3 result = Camera.mainCamera.WorldToViewportPoint(transform.position);
		if(isBetween(result.x, 0, 1) && isBetween(result.y, 0, 1) && result.z>0 && hasToUpdateColors)
		{
			if(!isWater && !useShader)
				myFilter.mesh.colors32 = colorsBuffer;
			
			renderer.enabled = true;
			hasToUpdateColors = false;
			enabled = false;
		}
		
		if(wait>0)
		{
			if(Vector3.Distance(Camera.mainCamera.transform.position, transform.position)<20)
				wait-=Time.deltaTime*60;
			else
				wait-=Time.deltaTime*20;
			if(wait<=0)
			{
				refreshShape();
				
				if(forceNearbyRefresh && !isWater)
					onAppear();
			}
			
		}
	
		/*if (Vector3.Distance(transform.position,lastPosition)>0) 
		{
			refreshShape();
			foreach(MeshBlender o in objects)
			{
				o.refreshShape();
			}
			lastPosition = transform.position;
		}*/
	}
	
	public int waterSpread = 4;
	void spreadWater()
	{
		if(deleted)
			return;
		
		if(transform.position.y<-50)
			return;
		
		MeshBlender blocUnder = MeshIndexer.GetBlocAt(transform.position+new Vector3(0, -1, 0), 1);
		
		if(blocUnder==null)
		{
			if(waterSpread==0)
				return;
			
			GameObject bloc = (GameObject)Instantiate(Resources.Load("SphereWater", typeof(GameObject)), transform.position+new Vector3(0, -1, 0), Quaternion.identity);
			bloc.GetComponent<MeshBlender>().wait = 10;
			MeshIndexer.IndexMesh(bloc.GetComponent<MeshBlender>());
			bloc.GetComponent<MeshBlender>().forceNearbyRefresh=true;
			wait = 10;
			bloc.GetComponent<MeshBlender>().waterSpread = 4;
			CancelInvoke("spreadWater");
		}
		else
		{
			if(blocUnder.isWater)
				return;
			
			if(waterSpread==0)
				return;
			
			for(int i=-1; i<=1; i++)
			{
				for(int j=-1; j<=1; j++)
				{
					if(MeshIndexer.CountSurroundingMeshes(transform.position+new Vector3(i, 0, j), 1)==0)
					{
						GameObject bloc = (GameObject)Instantiate(Resources.Load("SphereWater", typeof(GameObject)), transform.position+new Vector3(i, 0, j), Quaternion.identity);
						bloc.GetComponent<MeshBlender>().wait = 10;
						MeshIndexer.IndexMesh(bloc.GetComponent<MeshBlender>());
						bloc.GetComponent<MeshBlender>().forceNearbyRefresh=true;
						bloc.GetComponent<MeshBlender>().waterSpread = waterSpread-1;
						wait = 10;
						
						break;
					}
				}
			}
			
			CancelInvoke("spreadWater");
		}
	}
	
	bool isBetween(float val, float min, float max)
	{
		if(val<max && val>min)
			return true;
		else
			return false;
	}

	public float debugFloat = 0;
	
	public bool initialized = false;
	public void refreshShape()
	{
		if(isWater)
		{
			activateWater();
		}
		
		initialized=true;
		
		if(useShader)
		{
			renderer.material.SetFloat ("influenceRange", influenceRange);
			renderer.material.SetVector ("_worldPosition", transform.position);
			
			if(objects.Count<=0)
				objects = MeshIndexer.FindSurroundingMeshes(this);
			
			int i = 0;
			foreach (MeshBlender b in objects) 
			{
				if(b.GetInstanceID()!=GetInstanceID())
				{
					renderer.material.SetVector("_o"+(i+1), b.transform.position);
					i++;
				}
			}
			
			renderer.material.SetFloat("_surroundingSize", i);
		}
		else
		{
			try
			{
				int i = initialVertices.Length;
			}
			catch
			{
				initialVertices = myFilter.mesh.vertices;
			}
			
			Vector3[] vertices = new Vector3[initialVertices.Length];
			colorsBuffer = new Color32[initialVertices.Length];
			
			initialVertices.CopyTo(vertices, 0);
			
			for(int i=0; i<colorsBuffer.Length; i++)
			{
				colorsBuffer[i] = new Color32(255, 255, 255, 255);
			}
			
			if(objects.Count<=0)
				objects = MeshIndexer.FindSurroundingMeshes(this);
			
			for(int myVerticeIndex = 0; myVerticeIndex<vertices.Length; myVerticeIndex++)
			{
				Vector3 newVertice = Vector3.zero;
				
				Vector3 worldVerticePos = transform.TransformPoint(vertices[myVerticeIndex]);
	
				float colorRatio = 1;
				
				foreach(MeshBlender o in objects)
				{
					if(o.GetHashCode()!=GetHashCode())
					{
						try
						{
							float maxInfluenceRange = influenceRange;
							
							if(o.influenceRange>influenceRange)
								maxInfluenceRange = o.influenceRange;
							
							float ratio = Vector3.Distance(worldVerticePos, o.transform.position)/maxInfluenceRange;
							
							if(ratio<1)
							{
								if(ratio<0.7)
								{
									ratio = 0.7f;
								}
								
								if(Vector3.Distance(o.transform.position,worldVerticePos)>maxInfluenceRange/3)
									newVertice+=((o.transform.position-worldVerticePos)*(1-ratio));
		
								colorRatio *= ratio;
							}
						}catch{}
					}
				}	
	
				float channels = 255f*colorRatio;
				
				try
				{
					colorsBuffer[myVerticeIndex] = new Color32((byte)channels, (byte)channels, (byte)channels, (byte)255);
				}
				catch
				{}
				
				newVertice = MeshBlender.RotateY(newVertice, -transform.eulerAngles.y/180*Mathf.PI);
				newVertice = MeshBlender.RotateX(newVertice, -transform.eulerAngles.x/180*Mathf.PI);
				newVertice = MeshBlender.RotateZ(newVertice, -transform.eulerAngles.z/180*Mathf.PI);
				
				vertices[myVerticeIndex] = vertices[myVerticeIndex]+newVertice;
			}
			
			myFilter.mesh.vertices = vertices;
			myFilter.mesh.RecalculateBounds();
			myFilter.mesh.RecalculateNormals();
		}
		
		hasToUpdateColors = true;
		enabled = true;
	}

	public void onDestroy()
	{
		deleted = true;
		
		if(!isWater)
		{
			refreshShape();
			foreach(MeshBlender o in objects)
			{
				o.refreshShape();
			}
			lastPosition = transform.position;
		}
	}

	public void onAppear()
	{
		if(objects.Count<=0)
			objects = MeshIndexer.FindSurroundingMeshes(this);
		
		if(!isWater)
		{
			foreach (MeshBlender o in objects) 
			{
				if(o.GetHashCode()!=GetHashCode())
					o.refreshShape();
			}
		}
	}
	
	public static Vector3 RotateX(Vector3 v, float angle )
	{
		float sin = Mathf.Sin( angle );
		float cos = Mathf.Cos( angle );
		
		float ty = v.y;
		float tz = v.z;
		v.y = (cos * ty) - (sin * tz);
		v.z = (cos * tz) + (sin * ty);

		return v;
	}
	
	public static Vector3 RotateY(Vector3 v, float angle )
	{
		float sin = Mathf.Sin( angle );
		float cos = Mathf.Cos( angle );
		
		float tx = v.x;
		float tz = v.z;
		v.x = (cos * tx) + (sin * tz);
		v.z = (cos * tz) - (sin * tx);

		return v;
	}
	
	public static Vector3 RotateZ(Vector3 v, float angle )
	{
		float sin = Mathf.Sin( angle );
		float cos = Mathf.Cos( angle );
		
		float tx = v.x;
		float ty = v.y;
		v.x = (cos * tx) - (sin * ty);
		v.y = (cos * ty) + (sin * tx);

		return v;
	}
}	
