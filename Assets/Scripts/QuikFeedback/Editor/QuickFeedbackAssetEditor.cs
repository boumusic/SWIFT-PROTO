using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Pataya.QuikFeedback;
using System.Reflection;

[CustomEditor(typeof(QuikFeedbackAsset))]
public class QuickFeedbackAssetEditor : Editor
{
    private QuikFeedbackAsset t;

    private void OnEnable()
    {
        t = target as QuikFeedbackAsset;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("Add"))
        {
            
        }

        EditorGUILayout.Popup(0, new string[] { "Test", "Sip" });
        serializedObject.ApplyModifiedProperties();
    }
}
