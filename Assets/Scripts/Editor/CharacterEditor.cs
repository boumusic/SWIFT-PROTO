using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Character))]
public class CharacterEditor : Editor
{
	private Character t;

	private void OnEnable()
	{
		t = target as Character;
	}

	public override void OnInspectorGUI()
	{
		EditorGUI.BeginChangeCheck();
		base.OnInspectorGUI();

        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Current State : " + t.CurrentState.ToString());
        EditorGUILayout.LabelField("WallClimb : " + t.WallClimbCharge.ToString());
        EditorGUILayout.LabelField("Cast Ledge : " + t.CastLedge().ToString());
        EditorGUILayout.LabelField("Cast Wall : " + t.CastWall().ToString());
        

        EditorGUILayout.EndVertical();

		serializedObject.ApplyModifiedProperties();
		if (EditorGUI.EndChangeCheck())
		{
			EditorUtility.SetDirty(t);
		}
	}
}
