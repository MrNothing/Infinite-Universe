using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PolarMeshIndexer
{
	
}

public class MeshIndexer
{

	public static Dictionary<string, List<MeshBlender>> squareBasedWorldIndex = new Dictionary<string, List<MeshBlender>>();
	public static Dictionary<string, MeshBlender> index = new Dictionary<string, MeshBlender>();
	
	public static Hashtable dugZones=new Hashtable();
	
	public static void Clear()
	{
		squareBasedWorldIndex.Clear();
		index.Clear();
		dugZones.Clear();
	}

	public static void RemoveOldIndex(MeshBlender mesh, Vector3 pos)
	{
		string oldIndex = Mathf.Floor (pos.x / mesh.baseStep) * mesh.baseStep + "_" + Mathf.Floor (pos.y / mesh.baseStep) * mesh.baseStep + "_" + Mathf.Floor (pos.z / mesh.baseStep) * mesh.baseStep;
		
		List<MeshBlender> blockElements = new List<MeshBlender>();
		try
		{
			blockElements = squareBasedWorldIndex[oldIndex];
			blockElements.Remove(mesh);
			index.Remove(oldIndex);
		}
		catch
		{
			
		}
		
		if(dugZones[oldIndex]==null)
			dugZones.Add(oldIndex, oldIndex);
	}

	public static void IndexMesh(MeshBlender mesh)
	{
		int range = 1;
		Vector3 indexedPos = GetIndexAsVector3 (mesh);
		
		for (int x=-range; x<=range; x++) 
		{
			for (int y=-range; y<=range; y++) 
			{
				for (int z=-range; z<=range; z++) 
				{
					string tmpIndex = (indexedPos.x+x*mesh.baseStep)+"_"+(indexedPos.y+y*mesh.baseStep)+"_"+(indexedPos.z+z*mesh.baseStep);
					AddMeshAtIndex(mesh, tmpIndex);
				}
			}
		}
		
		try
		{
			index.Add(GetIndex(mesh), mesh);
		}
		catch
		{
			
		}
		
		//Debug.Log ("Assigning index to: " + indexedPos);
	}
	
	static void AddMeshAtIndex(MeshBlender mesh,  string index)
	{
		List<MeshBlender> blockElements = new List<MeshBlender>();
		try
		{
			blockElements = squareBasedWorldIndex[index];
		}
		catch
		{

		}
		
		try
		{
			blockElements.Remove (mesh);
		}
		catch
		{}

		blockElements.Add (mesh);
		squareBasedWorldIndex[index] = blockElements;
	}

	public static string GetIndex(MeshBlender mesh)
	{
		Vector3 pos = mesh.transform.position;
		return Mathf.Floor (pos.x / mesh.baseStep) * mesh.baseStep+"_"+Mathf.Floor (pos.y / mesh.baseStep) * mesh.baseStep+"_"+Mathf.Floor (pos.z / mesh.baseStep) * mesh.baseStep;
	}
	
	public static string GetIndexAt(Vector3 pos, float step)
	{
		return Mathf.Floor (pos.x / step) * step+"_"+Mathf.Floor (pos.y / step) * step+"_"+Mathf.Floor (pos.z / step) * step;
	}
	
	public static Vector3 GetIndexAsVector3At(Vector3 pos, float step)
	{
		return new Vector3( Mathf.Floor (pos.x / step) * step, Mathf.Floor (pos.y / step) * step, Mathf.Floor (pos.z / step) * step);
	}

	public static Vector3 GetIndexAsVector3(MeshBlender mesh)
	{
		Vector3 pos = mesh.transform.position;
		return new Vector3( Mathf.Floor (pos.x / mesh.baseStep) * mesh.baseStep, Mathf.Floor (pos.y / mesh.baseStep) * mesh.baseStep, Mathf.Floor (pos.z / mesh.baseStep) * mesh.baseStep);
	}

	public static List<MeshBlender> FindSurroundingMeshes(MeshBlender mesh)
	{
		string index = GetIndex (mesh);

		List<MeshBlender> nearMeshes = new List<MeshBlender>();

		try
		{
			nearMeshes = squareBasedWorldIndex[index];
		}catch{}

		//Debug.Log ("Searching around: " +index+" found: "+nearMeshes.Count+" meshes.");

		return nearMeshes;
	}
	
	public static int CountSurroundingMeshes(Vector3 pos, float step)
	{
		string _index = GetIndexAt (pos, step);

		MeshBlender meshAtIndex;

		try
		{
			meshAtIndex = index[_index];
			return 1;
		}catch
		{
			return 0;
		}
	}
	
	public static MeshBlender GetBlocAt(Vector3 pos, float step)
	{
		string _index = GetIndexAt (pos, step);

		try
		{
			return index[_index];
		}catch
		{
			return null;
		}
	}
}
