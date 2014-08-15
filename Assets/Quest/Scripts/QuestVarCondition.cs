using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class QuestVarCondition : IConditionState {
	
	public enum OperatorType { Equals, NotEquals }
	
	public QuestVariable questVar;
	public string targetValue;
	public OperatorType oprType;
	
	public override bool Condition() {
		if (oprType == OperatorType.Equals)
			return questVar.Value == targetValue;
		else if (oprType == OperatorType.NotEquals)
			return questVar.Value != targetValue;
		
		return false;
	}
}
