using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlocRayDestroyer : MonoBehaviour {
	
	InGame inGame;
	Core core;
	// Use this for initialization
	void Start () 
	{
		inGame = GetComponent<InGame>();
		core = ((GameObject)GameObject.Find("Core")).GetComponent<Core>();
	}

	int wait = 0;
	// Update is called once per frame
	void Update()
	{
		Ray ray = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
		
		RaycastHit hitFloor2;
		if (Physics.Raycast (ray.origin, ray.direction, out hitFloor2, 10f)) 
		{
			
			if(hitFloor2.collider.gameObject.GetComponent<MeshBlender>())
			{
				if(Input.GetMouseButtonUp(1) && wait<=0)
				{
					if(hitFloor2.collider.gameObject.transform.position.y<-50)
						return;
					
					if(hitFloor2.collider.gameObject.GetComponent<MeshBlender>().isTerrainBloc)
					{
						string prefab = "SphereDirt";
						
						if(hitFloor2.point.y<20)
						{
							prefab = "SphereStone";
						}
						
						GameObject o = core.spheresByName[prefab];
						
						List<MeshBlender> objectsAround = new List<MeshBlender>(MeshIndexer.FindSurroundingMeshes(hitFloor2.collider.gameObject.GetComponent<MeshBlender>()));
						
						foreach(MeshBlender b in objectsAround)
						{
							for(int i=-1; i<=1; i++)
							{
								Vector3 pos=b.transform.position-hitFloor2.normal+new Vector3(0, i, 0);
								
								if(MeshIndexer.CountSurroundingMeshes(pos, 1)==0 && MeshIndexer.dugZones[MeshIndexer.GetIndexAt(pos, 1)]==null && pos.y<=core.getHeightAt(pos))
								{
									GameObject bloc = (GameObject)Instantiate(o, pos, Quaternion.identity);
									bloc.GetComponent<MeshBlender>().wait = 10;
									MeshIndexer.IndexMesh(bloc.GetComponent<MeshBlender>());
									bloc.GetComponent<MeshBlender>().forceNearbyRefresh=true;
									bloc.GetComponent<MeshBlender>().isTerrainBloc = true;
								}
							}
						}
					}
					
					MeshIndexer.RemoveOldIndex(hitFloor2.collider.gameObject.GetComponent<MeshBlender>(), hitFloor2.collider.gameObject.transform.position);
					hitFloor2.collider.gameObject.transform.Translate(0, -1000, 0);
					//hitFloor2.collider.gameObject.GetComponent<MeshBlender>().refreshShape();
					hitFloor2.collider.gameObject.GetComponent<MeshBlender>().onDestroy();
					//Destroy(hit.collider.gameObject);
					wait = 10;
				}
				
				if(Input.GetMouseButtonUp(0) && Input.GetAxis("Fire1")>0 && wait<=0)
				{
					if(hitFloor2.collider.gameObject.GetComponent<MeshBlender>())
					{
						if(hitFloor2.collider.gameObject.GetComponent<MeshBlender>().initialized)
						{
							if(MeshIndexer.CountSurroundingMeshes(hitFloor2.collider.transform.position+hitFloor2.normal, 1)==0)
							{
								GameObject bloc = (GameObject)Instantiate(core.spheresByName[inGame.infos[inGame.selection].prefab], hitFloor2.collider.transform.position+hitFloor2.normal, Quaternion.identity);
								bloc.GetComponent<MeshBlender>().wait = 10;
								MeshIndexer.IndexMesh(bloc.GetComponent<MeshBlender>());
								bloc.GetComponent<MeshBlender>().forceNearbyRefresh=true;
								wait = 10;
							}
						}
					}
				}
				
			}
		}
	}
	
	void OnGUI () {
		Ray ray = Camera.mainCamera.ScreenPointToRay(Input.mousePosition);
		
		RaycastHit hitFloor2;
		if (Physics.Raycast (ray.origin, ray.direction, out hitFloor2, 10f)) 
		{
			
			if(hitFloor2.collider.gameObject.GetComponent<MeshBlender>())
			{
				GUI.Label(new Rect(0, 0, 300, 20), hitFloor2.collider.name);
			}
		}
		
		if(wait>0)
		wait--;
	}
}
