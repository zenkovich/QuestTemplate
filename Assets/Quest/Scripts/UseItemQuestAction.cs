using UnityEngine;
using System.Collections;

public class UseItemQuestAction : IQuestAction {
	
	public QuestItem item;
	
	public UseItemQuestAction(): base() {
		caption = "Using quest item";
	}
	
	public override void Action() {		
		QuestInventory.instance.RemoveItem(item);
	}
}
