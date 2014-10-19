using UnityEngine;
using System.Collections;

[System.Serializable]
public class BlocInfos
{
	public string name;
	public string prefab;
	public Texture2D texture;
}

public class InGame : MonoBehaviour {
	
	public Texture2D skillBg;
	public Texture2D skillSelector;
	public BlocInfos[] infos;
	public int selection=0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void OnGUI () {
		for(int i=0; i<infos.Length; i++)
		{
			Rect rect = new Rect(10+(skillBg.width+5)*i, Screen.height-skillBg.height-10,skillBg.width, skillBg.height);
			GUI.DrawTexture(rect, infos[i].texture);
			if(rect.Contains(new Vector3(Input.mousePosition.x, Screen.height-Input.mousePosition.y)))
			{
				if(Input.GetMouseButton(0))
				{
					selection = i;
				}
			}
		}
		
		GUI.DrawTexture(new Rect(10+(skillBg.width+5)*selection, Screen.height-skillBg.height-10,skillBg.width, skillBg.height), skillSelector);
	}
}
