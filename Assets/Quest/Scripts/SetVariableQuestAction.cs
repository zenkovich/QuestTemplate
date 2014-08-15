using UnityEngine;
using System.Collections;

public class SetVariableQuestAction : IQuestAction {

	public QuestVariable variable;
	public string value;
	
	public SetVariableQuestAction(): base() {
		caption = "Set quest variable";
	}
	
	public override void Action() {		
		variable.Value = value;
		Debug.Log ("Set value:" + value + " to " + variable);
	}
}
