using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PowerSettings))]
public class PowerSettingsEditor : Editor
{
	private PowerSettings t;

	private void OnEnable()
	{
		t = target as PowerSettings;
	}

	public override void OnInspectorGUI()
	{
		EditorGUI.BeginChangeCheck();

        CustomEditorUtility.DrawTitle(t.name.Replace("POW_", ""));

        CustomEditorUtility.QuickSerializeObject("cooldown", serializedObject);

        EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);

        SerializedProperty actions = serializedObject.FindProperty("actions");
        for (int i = 0; i < actions.arraySize; i++)
        {
            EditorGUILayout.BeginVertical("box");
            SerializedProperty action = actions.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginHorizontal();
            if(i<t.actions.Count)
                t.actions[i].enabled = EditorGUILayout.Toggle(t.actions[i].enabled);
            CustomEditorUtility.QuickSerializeRelative("delay", action);
            if(CustomEditorUtility.RemoveButton())
            {
                Undo.RecordObject(t, "Remove");
                t.actions.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();

            CustomEditorUtility.QuickSerializeRelative("action", action);
            EditorGUILayout.EndVertical();
        }

        if(CustomEditorUtility.AddButton())
        {
            Undo.RecordObject(t, "Add");
            t.actions.Add(new PowerActionSettings());
        }

		if (EditorGUI.EndChangeCheck())
		{
			EditorUtility.SetDirty(t);
            Repaint();
		}
		serializedObject.ApplyModifiedProperties();
	}
}
