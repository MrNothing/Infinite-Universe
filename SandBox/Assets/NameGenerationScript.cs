﻿using UnityEngine;
using System.Collections;



public class NameGenerationScript : MonoBehaviour {

	[SerializeField]
	public GUIText label;

	char[] vowels = {'a', 'e', 'y', 'i', 'o', 'u', 'a', 'e', 'o'};
	char[] doubleVowels = {'a', 'e', 'i', 'o', 'u'};
	char[] consonants = {'z', 'r', 't', 'p', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'x', 'c', 'v', 'b', 'n', 's', 't', 'r', 'k', 'r', 't', 'p', 's'};
	char[] doubleConsonants = {'r', 's', 'h', 'l', 'n'};

	// Use this for initialization
	void Start () {
	}

	void Update()
	{
		if(Input.GetButtonDown("Fire1"))
		{
			float seed;
			seed = Random.Range (0f, 1f);
			label.text = GenerateName (seed);
		}
	}

	string GenerateName (float seed)
	{
		int nbSyllables;
		string name;
		int nbConsonants;
		int secondLetter;
		name = "";
		if((int)(seed*100)%5 == 0) nbConsonants = 2;
		else if((int)(seed*100)%5 <=2) nbConsonants = 1;
		else nbConsonants = 0;
		for(int i = 1; i <= nbConsonants; i++)
		{
			if(i == 2)
			{
				secondLetter = (int)(seed*Random.Range (20,100))%doubleConsonants.Length;
				if(doubleConsonants[secondLetter] != name[0])
					name += doubleConsonants[secondLetter];
				else i = 1;
			}
			else name += consonants[(int)(seed*Random.Range (20,100))%consonants.Length];
		}
		if((int)(seed*10)%8 < 2) nbSyllables = 1;
		else if((int)(seed*10)%8 < 5) nbSyllables = 2;
		else if((int)(seed*10)%8 < 8) nbSyllables = 3;
		else nbSyllables = 4;
		for(int i = 1; i <= nbSyllables; i++){
			name += BuildSyllable(seed*Random.Range (20,100)*10);
		}
		name = char.ToUpper(name[0]) + name.Substring(1);
		if (name.Length >= 9){
			string sub1 = name;
			string sub2 = name;
			name = sub1.Substring(0,name.Length-3) + "'" + sub2.Substring(name.Length-3,3);
		}
		if(name.Length <=3) name += "'" + BuildSyllable(seed*Random.Range (20,100));
		for(int i = 1; i < name.Length; i++){
			if (name [i-1] == 'h' && name[i] == 'h'){
				name = name.Substring(0,i-1) + name.Substring(i, name.Length-i-1);
			}
		}
		if (name [name.Length - 1] == 'r' || name [name.Length - 1] == 'n' || name [name.Length - 1] == 'j' || name [name.Length - 1] == 'l')
						name = name.Substring (0, name.Length - 1);
		return name;
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
			if(i == 2) syllable += doubleVowels[(int)(seed*Random.Range(20,100))%doubleVowels.Length];
			else syllable += vowels[(int)(seed*Random.Range(20,100))%vowels.Length];
		}
		for(int i = 1; i <= nbConsonants; i++)
		{
			if(i == 2) syllable += doubleConsonants[(int)(seed*Random.Range (20,100))%doubleConsonants.Length];
			else syllable += consonants[(int)(seed*Random.Range (20,100))%consonants.Length];
		}
		return syllable;
	}
}
