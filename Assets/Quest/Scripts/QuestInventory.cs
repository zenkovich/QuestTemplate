using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class QuestInventory : MonoBehaviour {

	[Serializable]
	public class ItemInfo {
		public string name;
		public QuestItem item;
		public int count;
		public QuestItemSlot slot;
	}

	public static QuestInventory instance;
	public QuestItemSlot slotPrefab;
	public UIScrollView scrollView;
	public UIGrid slotsContainerGrid;

	public List<ItemInfo> items = new List<ItemInfo>();


	void Awake() {
		instance = this;
	}

	public void AddItem(QuestItem item) {
		ItemInfo info = FindItemInfo(item);
		/*if (info != null) {
			info.count++;
			info.slot.Count = info.count;
			return;
		}*/
		
		GameObject slotObj = Instantiate(slotPrefab.gameObject) as GameObject;
		QuestItemSlot slot = slotObj.GetComponent<QuestItemSlot>();
		slot.Item = item;
		slot.Count = 1;
		slot.transform.Reparent(slotsContainerGrid.transform);
		slotsContainerGrid.Reposition();

		info = new ItemInfo(){ name = item.itemName, item = item, count = 1, slot = slot };

		items.Add(info);
	}

	public void RemoveItem(QuestItem item) {
		ItemInfo info = FindItemInfo (item);

		if (info == null) {
			Debug.LogError ("Trying to remove item from inventory: " + item.itemName + ", no item");
			return;
		}

		if (info.count > 1) {
			info.count--;
			info.slot.Count = info.count;
			return;
		}

		QuestItemSlot slot = info.slot;
		items.Remove(info);
		Destroy(slot.gameObject);

		slotsContainerGrid.Reposition();
	}

	public bool IsHaveItem(QuestItem item) {
		return GetItemsCount(item) > 0;
	}

	public int GetItemsCount(QuestItem item) {
		ItemInfo info = FindItemInfo(item);

		if (info == null)
			return 0;

		return info.count;
	}

	public void OnItemSlotPressed(QuestItemSlot slot) {
		Debug.Log ("Pressed item slot with item " + slot.item.itemName);
	}

	ItemInfo FindItemInfo(QuestItem item) {
		return items.Find(x => x.item == item);
	}
}
