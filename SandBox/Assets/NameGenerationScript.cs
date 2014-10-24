using UnityEngine;
using System.Collections;

public class NameGenerationScript : MonoBehaviour {
	char[] vowels = {'a', 'e', 'y', 'i', 'o', 'u'};
	char[] consonants = {'z', 'r', 't', 'p', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'w', 'x', 'c', 'v', 'b', 'n'};
	char[] doubleConsonants = {'r', 's', 'h', 'l', 'm', 'v', 'n'};

	// Use this for initialization
	void Start () {
		StartCoroutine ("GenerateName");
	}

	IEnumerator GenerateName ()
	{
		float seed;
		int nbSyllables;
		string name;
		int nbConsonants;
		while(true){
			seed = Random.Range (0f, 1f);
			name = "";
			if((int)(seed*100)%5 == 0) nbConsonants = 2;
			else if((int)(seed*100)%5 <=2) nbConsonants = 1;
			else nbConsonants = 0;
			for(int i = 1; i <= nbConsonants; i++){
				if(i == 2) name += doubleConsonants[(int)(seed*Random.Range (20,100))%doubleConsonants.Length];
				else name += consonants[(int)(seed*Random.Range (20,100))%consonants.Length];
			}
			if((int)(seed*10)%8 < 2) nbSyllables = 1;
			else if((int)(seed*10)%8 < 5) nbSyllables = 2;
			else if((int)(seed*10)%8 < 8) nbSyllables = 3;
			else nbSyllables = 4;
			for(int i = 1; i <= nbSyllables; i++){
				if(i == 4) name += BuildSyllable(seed*(i)*10);
				else name += BuildSyllable(seed*(i)*10);
			}
			name = char.ToUpper(name[0]) + name.Substring(1);
			if (name.Length >= 9){
				string sub1 = name;
				string sub2 = name;
				name = sub1.Substring(0,name.Length-3) + "'" + sub2.Substring(name.Length-3,3);
			}
			if(name.Length <=3) name += "'" + BuildSyllable(seed*Random.Range (20,100));
			Debug.Log (name);
			yield return null;
		}
	}

	string BuildSyllable(float seed)
	{
		int nbVowels;
		int nbConsonants;
		string syllable = "";
		if((seed*10)%6 < 4)
		{
			nbVowels = 1;
		}
		else
		{
			nbVowels = 2;
		}
		if((seed*100)%4 < 3)
		{
			nbConsonants = 1;
		}
		else
		{
			nbConsonants = 2;
		}
		for(int i = 1; i <= nbVowels; i++)
		{
			syllable += vowels[(int)(seed*Random.Range(20,100))%vowels.Length];
		}
		for(int i = 1; i <= nbConsonants; i++)
		{
			if(i == 2) syllable += doubleConsonants[(int)(seed*Random.Range (20,100))%doubleConsonants.Length];
			else syllable += consonants[(int)(seed*Random.Range (20,100))%consonants.Length];
		}
		return syllable;
	}
}
