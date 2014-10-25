using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public enum PlanetBlocType
{
	FullPlanet, Bloc, Water
}

public class DoodadQueue
{
	public int vertice;
	public Doodad doodad;
	public DoodadQueue(int _vertice, Doodad _doodad)
	{
		vertice = _vertice;
		doodad = _doodad;
	}
}

public class GeneratedLodBlocInfos 
{
	public MeshFilter origin;
	public MeshLodTris originScript;
	public Vector3[] vertices;
	public Color[] colors;
	public Color[] seaColors;
	public List<DoodadQueue> doodads;
	public PlanetBlocType type;
	public Matrix4x4 localToWorld;
	public Vector3 currentPos;
	public Thread T;

	public GeneratedLodBlocInfos(MeshFilter _origin, MeshLodTris _originScript, PlanetBlocType _type, Thread _T)
	{
		currentPos = _origin.transform.position;
		localToWorld = _origin.transform.localToWorldMatrix;
		vertices = _origin.mesh.vertices;
		originScript = _originScript;
		doodads = new List<DoodadQueue> ();
		origin = _origin;
		type = _type;
		T = _T;
	}
}
