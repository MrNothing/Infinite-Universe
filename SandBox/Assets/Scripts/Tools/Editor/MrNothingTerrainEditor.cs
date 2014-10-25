using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(MrNothingTerrain))]
public class MrNothingTerrainEditor : Editor {
	private MrNothingTerrain origin;

	MeshFilter myFilter;
	MeshCollider myCollider;

	GameObject grassGO;

	void Awake()
	{
		origin = (MrNothingTerrain)target;
		if(origin.GetComponent<MeshFilter> ())
			myFilter = origin.GetComponent<MeshFilter> ();
		else
			message = "No MeshFilter was found!";
			
		if(origin.GetComponent<MeshCollider> ())
			myCollider = origin.GetComponent<MeshCollider> ();
		else
			message = "No MeshCollider was found!";

		if(origin.initVertices==null)
			origin.initVertices = myFilter.mesh.vertices;

		grassGO = (GameObject)Resources.Load ("Grass", typeof(GameObject));
	}

	string message = "";

	string[] brushes = new string[]{"Levels", "Textures", "Details"};

	Material myMat=null;

	void OnSceneGUI()
	{
		if (Event.current.type == EventType.MouseDown) 
		{
			if (Event.current.button == 0) 
			{
				Event e = Event.current;

				Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

				RaycastHit hit;
				if (Physics.Raycast (ray.origin, ray.direction, out hit, 150)) 
				{
					if(origin.brushType==0)
					{
						//Debug.DrawLine (hit.point, hit.point + hit.normal);
						//Debug.DrawLine (ray.origin, ray.origin+ray.direction, Color.green);

						Vector3[] vertices = new Vector3[myFilter.mesh.vertices.Length];
						myFilter.mesh.vertices.CopyTo(vertices, 0);

						for(int i=0; i<vertices.Length; i++)
						{
							float ratio = Vector3.Distance(origin.transform.TransformPoint(vertices[i]), hit.point)/origin.brushSize;

							if(ratio<1)
							{
								vertices[i] += myFilter.mesh.normals[i]*(1-ratio)*origin.brushIntensity;
							}
						}

						myFilter.mesh.vertices = vertices;
						myFilter.mesh.RecalculateBounds();
						myFilter.mesh.RecalculateNormals();

						myCollider.sharedMesh = myFilter.mesh;
					}
					else if(origin.brushType == 1)
					{
						//Debug.DrawLine (hit.point, hit.point + hit.normal);
						//Debug.DrawLine (ray.origin, ray.origin+ray.direction, Color.green);
						
						Color[] colors = new Color[myFilter.mesh.colors.Length];
						myFilter.mesh.colors.CopyTo(colors, 0);
						
						for(int i=0; i<colors.Length; i++)
						{
							float ratio = Vector3.Distance(origin.transform.TransformPoint(myFilter.mesh.vertices[i]), hit.point)/origin.brushSize;
							
							if(ratio<1)
							{
								if(origin.channel==0)
								{
									colors[i].r += Mathf.Abs(origin.brushIntensity);
									colors[i].g -= Mathf.Abs(origin.brushIntensity);
									colors[i].b -= Mathf.Abs(origin.brushIntensity);
									colors[i].a -= Mathf.Abs(origin.brushIntensity);
								}
								if(origin.channel==1)
								{
									colors[i].r -= Mathf.Abs(origin.brushIntensity);
									colors[i].g += Mathf.Abs(origin.brushIntensity);
									colors[i].b -= Mathf.Abs(origin.brushIntensity);
									colors[i].a -= Mathf.Abs(origin.brushIntensity);
								}
								if(origin.channel==2)
								{
									colors[i].r -= Mathf.Abs(origin.brushIntensity);
									colors[i].g -= Mathf.Abs(origin.brushIntensity);
									colors[i].b += Mathf.Abs(origin.brushIntensity);
									colors[i].a -= Mathf.Abs(origin.brushIntensity);
								}
								if(origin.channel==3)
								{
									colors[i].r -= Mathf.Abs(origin.brushIntensity);
									colors[i].g -= Mathf.Abs(origin.brushIntensity);
									colors[i].b -= Mathf.Abs(origin.brushIntensity);
									colors[i].a += Mathf.Abs(origin.brushIntensity);
								}
								if(origin.channel==4)
								{
									colors[i].r -= Mathf.Abs(origin.brushIntensity);
									colors[i].g -= Mathf.Abs(origin.brushIntensity);
									colors[i].b -= Mathf.Abs(origin.brushIntensity);
									colors[i].a -= Mathf.Abs(origin.brushIntensity);
								}
							}
						}
						
						myFilter.mesh.colors = colors;
						myFilter.mesh.RecalculateBounds();
						myFilter.mesh.RecalculateNormals();
					}
					else
					{
						Color[] colors = new Color[myFilter.mesh.colors.Length];
						myFilter.mesh.colors.CopyTo(colors, 0);
						
						for(int i=0; i<colors.Length; i++)
						{
							float ratio = Vector3.Distance(origin.transform.TransformPoint(myFilter.mesh.vertices[i]), hit.point)/origin.brushSize;
							
							if(ratio<1)
							{
								GameObject detail = (GameObject)GameObject.Instantiate(grassGO);
								detail.transform.position = origin.transform.TransformPoint(myFilter.mesh.vertices[i]);
								detail.renderer.material = origin.details[origin.selectedDetail];

								if(origin.detailsIndex[i]!=null)
									Destroy((GameObject)origin.detailsIndex[i]);

								origin.detailsIndex[i] = detail;
							}
						}
					}
				}
				Event.current.Use();
			}
		}
	}

	public override void OnInspectorGUI()
	{
		GUILayout.Label ("Brush Type:");
		origin.brushType = GUILayout.SelectionGrid(origin.brushType, brushes, 3);

		EditorGUILayout.Separator ();

		if (origin.brushType == 0) 
		{
			if(GUILayout.Button("Reset"))
			{
				Vector3[] vertices = new Vector3[myFilter.mesh.vertices.Length];
				myFilter.mesh.vertices.CopyTo(vertices, 0);

				
				myFilter.mesh.vertices = origin.initVertices;
				myFilter.mesh.RecalculateBounds();
				myFilter.mesh.RecalculateNormals();
				
				myCollider.sharedMesh = myFilter.mesh;
			}
		} 
		else if (origin.brushType == 1) 
		{
			GUILayout.Label ("Selected Texture:");

			Texture[] textures = new Texture[5];
			for (int i=1; i<6; i++) {
				textures [i - 1] = origin.renderer.sharedMaterial.GetTexture ("_Layer" + i);
			}

			GUILayoutOption[] options = new GUILayoutOption[]{GUILayout.Width(250), GUILayout.Height(50)};

			origin.channel = GUILayout.SelectionGrid (origin.channel, textures, 5, options);
		} 
		else 
		{
			GUILayout.Label ("Details:");

			GUILayoutOption[] options = new GUILayoutOption[]{GUILayout.Width(380), GUILayout.Height(50)};

			Texture[] textures = new Texture[origin.details.Count];
			for (int i=0; i<origin.details.Count; i++) {
				textures [i] = origin.details[i].mainTexture;
			}

			origin.selectedDetail = GUILayout.SelectionGrid (origin.selectedDetail, textures, 9, options);

			myMat = (Material)EditorGUILayout.ObjectField("Add Detail", myMat, typeof(Material), false);

			if(myMat!=null)
			{
				if(origin.details.Count<9)
					origin.details.Add(myMat);

				myMat=null;
			}

			if(origin.details.Count==0)
				EditorGUILayout.HelpBox ("No detail texture was specified!", MessageType.Warning);
			else
			{
				if(origin.selectedDetail>origin.details.Count)
					origin.selectedDetail=0;

				GUILayout.BeginHorizontal();
				GUILayout.Label("Selected Detail: "+origin.details[origin.selectedDetail].name);
				if(GUILayout.Button("Remove"))
				{
					origin.details.Remove(origin.details[origin.selectedDetail]);
				}
				GUILayout.EndHorizontal();
			}
		}

		EditorGUILayout.Separator ();

		GUILayout.Label("Brush Size: ");
		origin.brushSize = EditorGUILayout.Slider(origin.brushSize, 0, 15);
		
		GUILayout.Label("Brush Intensity: ");
		origin.brushIntensity = EditorGUILayout.Slider(origin.brushIntensity, -1, 1);
		

		//test = EditorGUILayout.IntSlider (test, 0, 10);
		if(message.Length>0)
			EditorGUILayout.HelpBox (message, MessageType.Error);
		//es.speed = EditorGUILayout.Slider(“Enemy Ship Speed:”, es.speed, 1, 100);
	}
}