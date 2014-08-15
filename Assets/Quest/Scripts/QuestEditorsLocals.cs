using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class QuestEditorsLocals : MonoBehaviour {

	public enum Language { Rus, Eng };

	[Serializable]
	public class Key {
		public string key;
		public string value; 
	}

	public static QuestEditorsLocals instance;
	public List<Key> rusStrings;
	public List<Key> engStrings;
	public Language language;

	public static string GetString(string key) {
		if (instance == null)
			instance = FindObjectOfType<QuestEditorsLocals>();

		List<Key> searchArr = null;

		if (instance.language == Language.Eng) 
			searchArr = instance.engStrings;
		if (instance.language == Language.Rus) 
			searchArr = instance.rusStrings;

		Key ks = searchArr.Find (x => x.key == key);
		if (ks == null) {
			//Debug.LogWarning ("Can't find quest localize string: " + key);
			return key;
		}

		return ks.value;
	}
}
