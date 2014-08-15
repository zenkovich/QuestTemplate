using UnityEngine;
using UnityEditor;
using System.Collections;

public static class EditorsUtilities {

	public static bool DrawHeader (string text, bool state, int indent = 0, bool forceOn = false) {		
		GUILayout.Space(3f);
		if (!forceOn && !state) 
			GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
		
		GUILayout.BeginHorizontal();
		GUILayout.Space(3f*indent);
		GUI.changed = false;
		
		text = "<b><size=11>" + text + "</size></b>";
		
		if (state) 
			text = "\u25BC " + text;
		else 
			text = "\u25BA " + text;
		
		if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) 
			state = !state;
		
		GUILayout.Space(2f);
		GUILayout.EndHorizontal();
		GUI.backgroundColor = Color.white;
		
		if (!forceOn && !state)
			GUILayout.Space(3f);
		
		return state;
	}
	
	public static void BeginContentsColored (Color color, int indent = 0) {
		GUILayout.BeginHorizontal();
		GUILayout.Space(3f*indent);
		GUI.backgroundColor = color;
		EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(30f));
		GUILayout.BeginVertical();
		GUILayout.Space(5f);
		GUI.backgroundColor = Color.white;
	}
	
	public static void BeginContents (int indent = 0) {
		BeginContentsColored(Color.white, indent);
	}
	
	public static void EndContents () {
		GUILayout.Space(3f);
		GUILayout.EndVertical();
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(3f);
		GUILayout.EndHorizontal();
		GUILayout.Space(3f);
		GUI.backgroundColor = Color.white;
	}
}
