using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Core : MonoBehaviour {
	public float perlinScale = 0;
	public Transform player;
	
	public GameObject perlinWorldBloc;
	public GameObject[] spheres;
	
	public Dictionary<string, GameObject> spheresByName = new Dictionary<string, GameObject>();
	
	Hashtable index = new Hashtable();
	
	public bool makePlanarWorld = false;
	
	// Use this for initialization
	void Awake () {
		
		foreach(GameObject o in spheres)
		{
			spheresByName.Add(o.name, o);
		}
		
		if(!makePlanarWorld)
			return;
		
		for(int i = -2; i<=2; i++)
		{
			for(int j = -2; j<=2; j++)
			{
				GameObject worldBloc = (GameObject)Instantiate(perlinWorldBloc, new Vector3(i*10, 0, j*10), Quaternion.identity);
				WorldBloc block = worldBloc.GetComponent<WorldBloc>();
				block.perlinOrigin = new Vector3(i*perlinScale, j*perlinScale, 0);
				block.perlinScale = perlinScale;
				block.optimizationLvl = 0;
				block.clamp = false;
				index.Add(worldBloc.transform.position.x+"_"+worldBloc.transform.position.z, block);
			}
		}
		
		lastPlayerPos = player.transform.position;
	}
	
	// Update is called once per frame
	Vector3 lastPlayerPos;
	void Update () 
	{
		if(!makePlanarWorld)
			return;
		
		if(Vector3.Distance(player.transform.position, lastPlayerPos)>5)
		{
			Vector3 pos = MeshIndexer.GetIndexAsVector3At(player.transform.position, 10);
			pos.y = 0;
				
			for(int i = -2; i<=2; i++)
			{
				for(int j = -2; j<=2; j++)
				{
					string indexStr = (Mathf.Floor(player.transform.position.x/10)*10+i*10)+"_"+(Mathf.Floor(player.transform.position.z/10)*10+j*10);
					
					if(index[indexStr]==null)
					{
						GameObject worldBloc = (GameObject)Instantiate(perlinWorldBloc, pos+new Vector3(i*10, 0, j*10), Quaternion.identity);
						WorldBloc block = worldBloc.GetComponent<WorldBloc>();
						block.perlinOrigin = new Vector3(pos.x/10*perlinScale+i*perlinScale, pos.z/10*perlinScale+j*perlinScale, 0);
						block.perlinScale = perlinScale;
						block.optimizationLvl = 1;
						block.clamp = true;
						index.Add(indexStr, block);
					}
				}
			}
			
			//Debug.Log("Updating map...");
			
			lastPlayerPos = player.transform.position;
			
		}
	}
	
	public float getHeightAt(Vector3 pos)
	{
		float xIndex = Mathf.Floor(pos.x/10)*10;
		float zIndex = Mathf.Floor(pos.z/10)*10;
		
		WorldBloc block = (WorldBloc)index[xIndex+"_"+zIndex];
		
		try
		{
			return block.getHeightAt(pos);
		}
		catch
		{
			return 0;
		}		
	}
}
