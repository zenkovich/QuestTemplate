using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(QuestAction))]
public class QuestActionEditor : Editor {

	public enum ActionType { PlayAnimation, IfElseCondition, DebugMessage, SetVariable, AddQuestItem, UseQuestItem }
	public string[] ActionTypeNames = new string[] { "PlayAnimation", "IfElseCondition", "DebugMessage", "SetVariable", "AddQuestItem", "UseQuestItem" };

	public enum ConditionType { QuestVarCheck, CheckHaveItem }
	public string[] ConditionTypeNames = new string[] { "QuestVarCheck", "CheckHaveItem" };

	public GameObject prefab;

	
	private QuestAction targetAction;
	private ActionType creationActionType;	
	private ConditionType creationConditionType;
	private int internalIndent;
	
	private Color lightRed = new Color(1f, 0.7f, 0.7f);
	private Color lightGreen = new Color(0.7f, 1f, 0.7f);
	private Color lightBlue = new Color(0.7f, 0.7f, 1f);

	private string[] locActionTypeNames;
	private string[] locConditionTypeNames;
	private string[] locConditionOperatorTypeNames;
	private string[] locVariableCondOperatorTypeNames;
	
	private string[] ConditionOperatorTypeNames = new string[] { "And", "Or" };
	private string[] VariableCondOperatorTypeNames = new string[] { "Equals", "Not equals", "Not equals", "Not equals", "Not equals" };

	public override void OnInspectorGUI() {
		targetAction = target as QuestAction;
		CheckLocalizations();
		//Debug.Log ("Editing " + targetAction);

		if (Application.isPlaying) {
			if (GUILayout.Button( QuestEditorsLocals.GetString("Action!") )) 
				targetAction.Action();
		}

		GUILayout.Space(3f);
		DrawActions(targetAction.actions, QuestEditorsLocals.GetString( "Actions sequence:" ), ref targetAction.visibleActions, Color.white);

		Event evt = Event.current;
		if (evt.type == EventType.ContextClick) {
			GenericMenu menu = new GenericMenu();
			menu.AddItem(new GUIContent("Test"), true, OnInspectorGUI);
			menu.AddItem(new GUIContent("Test"), true, OnInspectorGUI);
			menu.ShowAsContext();
			evt.Use ();
		}
	}

	void LocalizeArray(ref string[] locals, string[] keys) {
		if (locals == null) {
			locals = new string[keys.Count ()];
			for (int i = 0; i < keys.Count (); i++)
				locals[i] = QuestEditorsLocals.GetString(keys[i]);
		}
	}

	void CheckLocalizations() {
		LocalizeArray(ref locActionTypeNames, ActionTypeNames);
		LocalizeArray(ref locConditionTypeNames, ConditionTypeNames);
		LocalizeArray(ref locConditionOperatorTypeNames, ConditionOperatorTypeNames);
		LocalizeArray(ref locVariableCondOperatorTypeNames, VariableCondOperatorTypeNames);
	}

	public void DrawActions(List<IQuestAction> actions, string caption, ref bool opened, Color color) {

		opened = EditorsUtilities.DrawHeader(caption, opened, internalIndent);
		if (opened) {
			EditorsUtilities.BeginContentsColored(color, internalIndent);
			
			if (actions.Count == 0)
				GUILayout.Label( QuestEditorsLocals.GetString("No actions") );
			else {			
				internalIndent++;
				
				int idx = 0;
				int needActionIdx = -1;
				int needAction = 0;
				foreach(IQuestAction act in actions) {

					int actt = DrawActionUI(act, (idx%2 == 0) ? Color.white:Color.gray);
					if (actt > 0) {
						needActionIdx = idx;
						needAction = actt;
					}
					idx++;
				}
				
				if (needActionIdx >= 0) {
					if (needAction == 1)
						actions.RemoveAt(needActionIdx);
					if (needAction == 2) {
						actions.Insert(needActionIdx - 1, actions[needActionIdx]);
						actions.RemoveAt(needActionIdx + 1);
					}
					if (needAction == 3) {
						actions.Insert(needActionIdx + 2, actions[needActionIdx]);
						actions.RemoveAt(needActionIdx);
					}
				}
				
				internalIndent--;
			}			
			
			GUILayout.BeginHorizontal();
			if (GUILayout.Button( QuestEditorsLocals.GetString("Clear") )) 
				ClearActions(actions);
			
			if (GUILayout.Button( QuestEditorsLocals.GetString("Add action") )) 
				CreateNewAction(actions);
			
			creationActionType = (ActionType)EditorGUILayout.Popup((int)creationActionType, locActionTypeNames);
			GUILayout.Space(5f);
			GUILayout.EndHorizontal();
			
			EditorsUtilities.EndContents ();
		}
	}
	
	public int DrawActionUI(IQuestAction action, Color colr) {
		
		int procAction = 0;
		
		GUILayout.BeginHorizontal();

		if (action.opened) {
			GUILayout.BeginVertical();
			if (GUILayout.Button("\u25AC", "ButtonLeft", GUILayout.Width(20f), GUILayout.Height(16f)))
				procAction = 1;
			if (GUILayout.Button("\u25B2", "ButtonLeft", GUILayout.Width(20f), GUILayout.Height(16f)))
				procAction = 2;
			if (GUILayout.Button("\u25BC", "ButtonLeft", GUILayout.Width(20f), GUILayout.Height(16f)))
				procAction = 3;
			GUILayout.EndVertical();
		}
		else {
			if (GUILayout.Button("\u25AC", "ButtonLeft", GUILayout.Width(20f), GUILayout.Height(16f)))
				procAction = 1;
		}
		
		EditorGUILayout.BeginVertical();

		action.opened = EditorsUtilities.DrawHeader(action.caption, action.opened, internalIndent);
		if (action.opened) {
			EditorsUtilities.BeginContentsColored(colr, internalIndent);

			EditorGUILayout.BeginVertical();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField( QuestEditorsLocals.GetString("Caption:"), GUILayout.MaxWidth (100f));
			action.caption = EditorGUILayout.TextField(action.caption);
			EditorGUILayout.EndHorizontal ();
			
			if (action.GetType () == typeof(PlayAnimQuestAction))
				DrawQuestAction(action as PlayAnimQuestAction);
			else if (action.GetType() == typeof(IfElseQuestAction))
				DrawQuestAction(action as IfElseQuestAction);
			else if (action.GetType() == typeof(DebugQuestAction))
				DrawQuestAction(action as DebugQuestAction);
			else if (action.GetType() == typeof(SetVariableQuestAction))
				DrawQuestAction(action as SetVariableQuestAction);
			else if (action.GetType() == typeof(AddQuestItemAction))
				DrawQuestAction(action as AddQuestItemAction);
			else if (action.GetType() == typeof(UseItemQuestAction))
				DrawQuestAction(action as UseItemQuestAction);

			EditorGUILayout.EndVertical();

			
			EditorsUtilities.EndContents ();
		}

		
		EditorGUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.Space(5f);
		
		return procAction;
	}

	void CreateNewAction(List<IQuestAction> actions) {
		if (creationActionType == ActionType.IfElseCondition)
			actions.Add (ScriptableObject.CreateInstance<IfElseQuestAction>());
		else if (creationActionType == ActionType.PlayAnimation)
			actions.Add (ScriptableObject.CreateInstance<PlayAnimQuestAction>());
		else if (creationActionType == ActionType.DebugMessage)
			actions.Add (ScriptableObject.CreateInstance<DebugQuestAction>());
		else if (creationActionType == ActionType.SetVariable)
			actions.Add (ScriptableObject.CreateInstance<SetVariableQuestAction>());
		else if (creationActionType == ActionType.AddQuestItem)
			actions.Add (ScriptableObject.CreateInstance<AddQuestItemAction>());
		else if (creationActionType == ActionType.UseQuestItem)
			actions.Add (ScriptableObject.CreateInstance<UseItemQuestAction>());
	}

	void ClearActions(List<IQuestAction> actions) {
		actions.Clear ();
	}
	
	public void DrawQuestAction( PlayAnimQuestAction action) {		
		GUILayout.BeginHorizontal ();
		
		action.animation = (Animation)EditorGUILayout.ObjectField(action.animation, typeof(Animation), true);
		action.clip = (AnimationClip)EditorGUILayout.ObjectField(action.clip, typeof(AnimationClip), false);
		
		GUILayout.EndHorizontal ();
	}
	
	public void DrawQuestAction( DebugQuestAction action) {		
		GUILayout.BeginHorizontal ();
		
		EditorGUILayout.LabelField( QuestEditorsLocals.GetString("Message"), GUILayout.MaxWidth (80f));
		action.message = EditorGUILayout.TextField(action.message);
		
		GUILayout.EndHorizontal ();
	}
	
	public void DrawQuestAction( SetVariableQuestAction action) {		
		GUILayout.BeginHorizontal ();
		
		action.variable = (QuestVariable)EditorGUILayout.ObjectField(action.variable, typeof(QuestVariable), false);
		EditorGUILayout.LabelField( QuestEditorsLocals.GetString("Value"), GUILayout.MaxWidth (60f));
		action.value = EditorGUILayout.TextField(action.value);
		
		GUILayout.EndHorizontal ();
	}
	
	public void DrawQuestAction( AddQuestItemAction action) {		
		GUILayout.BeginHorizontal ();		
		action.item = (QuestItem)EditorGUILayout.ObjectField(action.item, typeof(QuestItem), false);		
		GUILayout.EndHorizontal ();
	}
	
	public void DrawQuestAction( UseItemQuestAction action) {		
		GUILayout.BeginHorizontal ();		
		action.item = (QuestItem)EditorGUILayout.ObjectField(action.item, typeof(QuestItem), false);		
		GUILayout.EndHorizontal ();
	}
	
	public void DrawQuestAction( IfElseQuestAction action) {	
		
		internalIndent++;

		DrawIfElseCondition(action.conditions, ref action.visibleConditions);
		DrawActions(action.onTrueActions,  QuestEditorsLocals.GetString("TRUE actions sequence"), ref action.visibleTrueActions, lightGreen);
		DrawActions(action.onFalseActions,  QuestEditorsLocals.GetString("FALSE actions sequence"), ref action.visibleFalseActions, lightRed);
		
		internalIndent--;
	}

	void DrawIfElseCondition(List<IConditionState> states, ref bool opened) {

		opened = EditorsUtilities.DrawHeader( QuestEditorsLocals.GetString("IF:"), opened, internalIndent);
		if (opened) {
			EditorsUtilities.BeginContentsColored(lightBlue, internalIndent);
			
			if (states.Count == 0)
				GUILayout.Label( QuestEditorsLocals.GetString("ALWAYS TRUE (no conditions)") );
			else {			
				EditorGUI.indentLevel++;
				
				int idx = 0;
				int needRemoveAtIdx = -1;
				foreach(IConditionState state in states) {
					
					if (DrawConditionStateUI(state, (idx%2 == 0) ? Color.white:Color.gray, idx != states.Count - 1))
						needRemoveAtIdx = idx;
					idx++;
				}
				
				if (needRemoveAtIdx >= 0)
					states.RemoveAt(needRemoveAtIdx);
				
				EditorGUI.indentLevel--;
			}			
			
			GUILayout.BeginHorizontal();
			if (GUILayout.Button( QuestEditorsLocals.GetString("Clear") )) 
				states.Clear ();
			
			if (GUILayout.Button( QuestEditorsLocals.GetString("Add condition") )) 
				CreateNewCondition(states);
			
			creationConditionType = (ConditionType)EditorGUILayout.Popup((int)creationConditionType, locConditionTypeNames);
			GUILayout.Space(5f);
			GUILayout.EndHorizontal();
			
			EditorsUtilities.EndContents ();
		}
	}

	void CreateNewCondition(List<IConditionState> states) {
		if (creationConditionType == ConditionType.QuestVarCheck)
			states.Add( ScriptableObject.CreateInstance<QuestVarCondition>() );
		if (creationConditionType == ConditionType.CheckHaveItem)
			states.Add( ScriptableObject.CreateInstance<CheckItemCondition>() );
	}	
	
	public bool DrawConditionStateUI(IConditionState state, Color colr, bool showOperator) {
		
		bool needRemove = false;
		
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("", "ToggleMixed", GUILayout.Width(20f), GUILayout.Height(16f)))
			needRemove = true;
		
		EditorsUtilities.BeginContentsColored(colr, internalIndent);

		//GUILayout.Label (state.state.GetType().Name);
		if (state.GetType () == typeof(QuestVarCondition))
			DrawStateCondition(state as QuestVarCondition);
		if (state.GetType () == typeof(CheckItemCondition))
			DrawStateCondition(state as CheckItemCondition);
		
		EditorsUtilities.EndContents ();
		
		if (showOperator)
			state.opr = (IConditionState.OperatorType)EditorGUILayout.Popup((int)state.opr, locConditionOperatorTypeNames, GUILayout.MinWidth(40f));

		GUILayout.EndHorizontal();
		GUILayout.Space(5f);
		
		return needRemove;
	}
	
	public void DrawStateCondition( QuestVarCondition condition) {		
		GUILayout.BeginHorizontal ();
		
		condition.questVar = (QuestVariable)EditorGUILayout.ObjectField(condition.questVar, typeof(QuestVariable), false);
		condition.oprType = (QuestVarCondition.OperatorType)EditorGUILayout.Popup((int)condition.oprType, locVariableCondOperatorTypeNames);
		condition.targetValue = EditorGUILayout.TextField(condition.targetValue);
		
		GUILayout.EndHorizontal ();
	}
	
	public void DrawStateCondition( CheckItemCondition condition) {		
		GUILayout.BeginHorizontal ();
		
		condition.item = (QuestItem)EditorGUILayout.ObjectField(condition.item, typeof(QuestItem), false);
		condition.oprType = (CheckItemCondition.OperatorType)EditorGUILayout.EnumPopup(condition.oprType);
		
		GUILayout.EndHorizontal ();
	}
}
