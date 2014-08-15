using UnityEngine;
using System.Collections;
using UnityEditor;

[RequireComponent(typeof(BoxCollider2D), typeof(QuestAction))]
public class QuestTrigger : MonoBehaviour {

	private QuestAction action;
	public int sortingOrder;

	void Awake() {
		action = GetComponent<QuestAction>();
		gameObject.layer = LayerMask.NameToLayer(QuestInputController.instance.triggersLayer);
		QuestInputController.instance.triggers.Add (this);

		SpriteRenderer sprRenderer = GetComponent<SpriteRenderer>();
		if (sprRenderer != null)
			sortingOrder = sprRenderer.sortingOrder;
	}

	public void Action() {
		action.Action();
	}

	[MenuItem("Quest/Add trigger %t")]
	public static void AddTrigger() {
		Selection.activeGameObject.AddComponent<QuestTrigger>();
	}
}
