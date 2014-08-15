using UnityEngine;
using System.Collections;

public class DebugQuestAction : IQuestAction {
	
	public string message;
	
	public DebugQuestAction(): base() {
		caption = "Debug message";
	}
	
	public override void Action() {		
		Debug.Log ("Quest debug message: " + message);
	}
}
