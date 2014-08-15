using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

[Serializable]
public class IQuestAction: ScriptableObject {

	public string caption = "Action";
	public bool opened = true;

	public virtual void Action() {}
}

[Serializable]
public class QuestAction : MonoBehaviour {

	public List<IQuestAction> actions = new List<IQuestAction>();
	public bool visibleActions = false;

	public void Clone() {
		Debug.Log ("COPY!");
	}

	public void Action() {
		Debug.Log ("Run actions on " + gameObject);
		actions.ForEach (x => x.Action());
	}
}
