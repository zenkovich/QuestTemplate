using UnityEngine;
using System.Collections;

public class QuestItemSlot : MonoBehaviour {

	public QuestItem item;
	public UISprite sprite;
	public UIWidget selection;
	public UILabel countLabel;

	private bool selected;
	private QuestInventory inventory;

	public bool Selected {
		set {
			selected = value;
			selection.gameObject.SetActive (value);
		}
		get { return selected; }
	}

	public int Count {
		set {
			countLabel.text = value.ToString ();
			countLabel.enabled = value > 1;
		}
	}

	public QuestItem Item {
		set {
			item = value;
			sprite.spriteName = item.sprite;
		}
		get { return item; }
	}

	public void OnPressed() {
		QuestInventory.instance.OnItemSlotPressed(this);
	}
}
