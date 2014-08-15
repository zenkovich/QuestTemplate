using UnityEngine;
using System.Collections;

public class CheckItemCondition : IConditionState {
	
	public enum OperatorType { Have, NotHave }
	
	public QuestItem item;
	public OperatorType oprType;
	
	public override bool Condition() {
		if (oprType == OperatorType.Have)
			return QuestInventory.instance.IsHaveItem(item);
		else if (oprType == OperatorType.NotHave)
			return !QuestInventory.instance.IsHaveItem(item);
		
		return false;
	}
}
