using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public abstract class IConditionState: ScriptableObject {
	public enum OperatorType { And, Or };
	
	public OperatorType opr;
	public abstract bool Condition();
}

[Serializable]
public class IfElseQuestAction : IQuestAction {

	public List<IConditionState> conditions = new List<IConditionState>();
	public List<IQuestAction> onTrueActions = new List<IQuestAction>();
	public List<IQuestAction> onFalseActions = new List<IQuestAction>();
	public bool visibleTrueActions = false;
	public bool visibleFalseActions = false;
	public bool visibleConditions = false;
	
	public IfElseQuestAction(): base() {
		caption = "IF <-> ELSE";
	}

	public override void Action() {
		bool res = true;
		
		bool lastCondition = true;
		IConditionState.OperatorType operation = IConditionState.OperatorType.And;
		foreach(IConditionState cond in conditions) {
			bool condValue = cond.Condition();
			if (operation == IConditionState.OperatorType.And) {
				if (!lastCondition || !condValue) {
					res = false;
					break;
				}
			}
			else if (operation == IConditionState.OperatorType.Or) {
				if (!lastCondition && !condValue) {
					res = false;
					break;
				}
			}
			
			lastCondition = operation == IConditionState.OperatorType.And ? (lastCondition && condValue):(lastCondition || condValue);
			operation = cond.opr;
		}

		if (res)
			onTrueActions.ForEach (x => x.Action());
		else
			onFalseActions.ForEach (x => x.Action ());
	}	
}
