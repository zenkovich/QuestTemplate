using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class QuestDataController : MonoBehaviour {

	[Serializable]
	public class QuestVarContainer {
		public string name;
		public QuestVariable var;
		public string value;
	}

	public static QuestDataController instance;
	public List<QuestVarContainer> vars;

	void Awake() {
		instance = this;
	}

	public void SetValue(QuestVariable var, string value) {
		QuestVarContainer cont = FindContainer(var);

		if (cont == null) {
			cont = new QuestVarContainer(){ name = var.name, var = var, value = value };
			vars.Add (cont);
		}
		else cont.value = value;
	}

	public string GetValue(QuestVariable var) {
		QuestVarContainer cont = FindContainer(var);
		
		if (cont == null) {
			cont = new QuestVarContainer(){ name = var.name, var = var, value = var.value };
			vars.Add (cont);
		}

		return cont.value;
	}

	QuestVarContainer FindContainer(QuestVariable var) {
		return vars.Find (x => x.var == var);
	}
}
