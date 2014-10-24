using UnityEngine;
using System.Collections;

public class NameGenerationScript : MonoBehaviour {
	char[] vowels = {'a', 'e', 'y', 'i', 'o', 'u'};
	char[] consonants = {'z', 'r', 't', 'p', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'w', 'x', 'c', 'v', 'b', 'n'};
	char[] doubleConsonants = {'r', 's', 'h', 'j', 'l', 'm', 'v', 'n'};

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
			if((int)(seed*10)%4 == 0) nbSyllables = 2;
			else if((int)(seed*10)%4 == 1) nbSyllables = 3;
			else if((int)(seed*10)%4 == 2) nbSyllables = 4;
			else nbSyllables = 5;
			for(int i = 1; i < nbSyllables; i++) name += BuildSyllable(seed*(i)*10);
			name = char.ToUpper(name[0]) + name.Substring(1);
			Debug.Log (seed + " " + name);
			yield return new WaitForSeconds(0.1f);
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
