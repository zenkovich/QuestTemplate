using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class QuestInputController : MonoBehaviour {

	public static QuestInputController instance;
	public List<QuestTrigger> triggers;
	public string triggersLayer = "QuestTrigger";

	void Awake() {
		instance = this;
	}

	void Update () {
		if (Input.GetMouseButtonUp(0)) {

			int triggersLayerIdx = LayerMask.NameToLayer(triggersLayer);
			QuestTrigger[] hitTriggers = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.zero)
				                           .Where(x => x.transform.gameObject.layer == triggersLayerIdx).Select(x => x.transform.GetComponent<QuestTrigger>()).ToArray();

			if (hitTriggers.Count() == 0)
				return;

			QuestTrigger topTrigger = hitTriggers.First();
			foreach(var trigger in hitTriggers) {
				if (topTrigger.sortingOrder < trigger.sortingOrder)
					topTrigger = trigger;
			}

			topTrigger.Action();
		}
	}
}
