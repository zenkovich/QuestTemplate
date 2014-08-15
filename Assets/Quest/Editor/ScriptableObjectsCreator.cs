using UnityEngine;
using System.Collections;
using UnityEditor;

public class ScriptableObjectsCreator : MonoBehaviour {
	[MenuItem("Assets/Create Quest Variable")]
	public static void CreateQuestVar() {
		ScriptableObjectUtility.CreateAsset("QuestVariable");
	}
	
	[MenuItem("Assets/Create Quest Item")]
	public static void CreateQuestItem() {
		ScriptableObjectUtility.CreateAsset("QuestItem");
	}
}
