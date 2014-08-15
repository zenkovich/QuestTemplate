using UnityEngine;
using System.Collections;

public class QuestVariable : ScriptableObject {
	public string value;

	public string Value {
		set {
			QuestDataController.instance.SetValue(this, value);
		}
		get {
			return QuestDataController.instance.GetValue(this);
		}
	}
}
