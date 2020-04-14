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
        EditorGUILayout.LabelField("Velocity : " + t.FinalVelocity.ToString());
        EditorGUILayout.LabelField("Jump Left : " + t.JumpLeft.ToString());
        EditorGUILayout.LabelField("Fall initialVelocity : " + t.FallInitVelocityY.ToString());
        EditorGUILayout.LabelField("WallClimb : " + t.WallClimbCharge.ToString());
        EditorGUILayout.LabelField("Cast Ledge : " + t.CastLedge().ToString());
        EditorGUILayout.LabelField("Cast Wall : " + t.CastWall().ToString());
        EditorGUILayout.LabelField("Was in flow state : " + t.WasInFlowState);
        EditorGUILayout.LabelField("Is in flow state : " + t.IsInFlowState);

        EditorGUILayout.EndVertical();

		serializedObject.ApplyModifiedProperties();
		if (EditorGUI.EndChangeCheck())
		{
			EditorUtility.SetDirty(t);
		}
	}
}
