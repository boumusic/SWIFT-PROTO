using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InstantiateAction))]
public class InstantiateActionEditor : Editor
{
	private InstantiateAction t;

	private void OnEnable()
	{
		t = target as InstantiateAction;
	}

	public override void OnInspectorGUI()
	{
		EditorGUI.BeginChangeCheck();

        CustomEditorUtility.DrawTitle("Instantiate " + EntityNames[t.indexInPool]);


        t.indexInPool = EditorGUILayout.Popup(t.indexInPool, EntityNames.ToArray());
        
        CustomEditorUtility.QuickSerializeObject(EntityNames[t.indexInPool], serializedObject);

		serializedObject.ApplyModifiedProperties();
		if (EditorGUI.EndChangeCheck())
		{
			EditorUtility.SetDirty(t);
		}
	}

    private List<string> EntityNames
    {
        get
        {
            List<string> entities = new List<string>();
            PoolManager pm = PoolManager.Instance;
            for (int i = 0; i < pm.pools.Count; i++)
            {
                if (pm.pools[i].Entity is InstantiableEntity)
                {
                    entities.Add(pm.pools[i].Entity.gameObject.name);
                }
            }

            return entities;
        }
        
    }
}
