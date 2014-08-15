using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class AddQuestItemAction : IQuestAction {
	
	public QuestItem item;
	
	public AddQuestItemAction(): base() {
		caption = "Add quest item";
	}
	
	public override void Action() {		
		QuestInventory.instance.AddItem(item);
	}
}
