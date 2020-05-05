using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PoolManager))]
public class PoolManagerEditor : Editor
{
    private PoolManager m;

    private void OnEnable()
    {
        m = (PoolManager)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.Space();

        CustomEditorUtility.DrawTitle("Pool Manager");
        CustomEditorUtility.QuickSerializeObject("instantiateOnAwake", serializedObject);
        DrawPools();
        RefreshPoolsButton();
        AddPoolButton();
        DestroyPoolsButton();
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawPools()
    {
        SerializedProperty pools = serializedObject.FindProperty("pools");

        for (int i = 0; i < pools.arraySize; i++)
        {
            DrawSinglePool(i, pools);
        }
    }

    private void DrawSinglePool(int i, SerializedProperty pools)
    {
        if (i < m.pools.Count)
        {
            Color defaultCol = GUI.color;
            GameObject prefab = m.pools[i].prefab;
            string name = "Pool " + i;
            bool hasPrefab = prefab != null;
            bool hasEntity = true;

            EditorGUILayout.BeginVertical("box");

            if (hasPrefab)
            {
                hasEntity = prefab.GetComponentInChildren<Entity>();
                name += ": " + prefab.name;
                if (!hasEntity)
                    GUI.color = Color.red;

                //if (elem) name += ": Level element";
            }

            else
            {
                GUI.color = Color.yellow;
                name += ": No prefab ";
            }

            EditorGUILayout.BeginHorizontal();

            EditorGUI.indentLevel += 1;
            m.pools[i].foldout = EditorGUILayout.Foldout(m.pools[i].foldout, name, EditorStyles.foldoutHeader);

            GUI.color = defaultCol;
            EditorGUI.indentLevel -= 1;
            if (GUILayout.Button("x", GUILayout.MaxWidth(50)))
            {
                if (m.pools[i].parent)
                {
                    Undo.DestroyObjectImmediate(m.pools[i].parent.gameObject);
                }
                Undo.RecordObject(m, "Remove Pool");
                m.pools.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
            if (i < m.pools.Count)
            {
                if (m.pools[i].foldout)
                {
                    EditorGUILayout.Space();

                    SerializedProperty arrayElement = pools.GetArrayElementAtIndex(i);
                    CustomEditorUtility.QuickSerializeRelative("prefab", arrayElement);


                    if (hasPrefab)
                    {
                        if (!hasEntity)
                        {
                            EditorGUILayout.BeginHorizontal();

                            EditorGUILayout.HelpBox("Error : The specified prefab is not an entity. Please add an entity script on it.", MessageType.Error);
                            if (GUILayout.Button("Fix"))
                            {
                                Undo.AddComponent(prefab, typeof(Entity));
                                //prefab.AddComponent<Entity>();
                            }

                            EditorGUILayout.EndHorizontal();
                        }

                        else
                        {
                            CustomEditorUtility.QuickSerializeRelative("amount", arrayElement);
                            GUI.enabled = false;
                            CustomEditorUtility.QuickSerializeRelative("parent", arrayElement);
                            CustomEditorUtility.QuickSerializeRelative("entities", arrayElement);
                            GUI.enabled = true;

                            EditorGUILayout.Space();

                            int currentlyActive = m.UpdateCurrentlyActive(i);

                            Rect r = EditorGUILayout.BeginVertical();
                            int usage = Mathf.CeilToInt(m.pools[i].usage * 100);
                            EditorGUI.ProgressBar(r, m.pools[i].usage, "Current usage of the pool: " + usage.ToString("F2") + "% (" + currentlyActive + " out of " + m.pools[i].entities.Count + ")");
                            GUILayout.Space(18);
                            EditorGUILayout.EndVertical();
                        }
                    }

                    else
                    {
                        EditorGUILayout.HelpBox("Please assign a prefab.", MessageType.Warning);
                    }
                }
            }

            EditorGUILayout.EndVertical();
        }
    }

    private void AddPoolButton()
    {
        if (GUILayout.Button("Add Pool"))
        {
            Undo.RecordObject(m, "Add pool");
            m.pools.Add(new Pool());
        }
    }

    private void RefreshPoolsButton()
    {
        Color defaultCol = GUI.color;
        GUI.color = new Color(1, 0.3f, 0.3f, 1f);
        if (m.pools.Count > 0)
        {
            if (m.ShouldRefresh())
            {
                if (GUILayout.Button("Refresh pools", GUILayout.MinHeight(50)))
                {
                    EditorUtility.SetDirty(m);
                    RefreshPools();
                }
            }
        }

        GUI.color = defaultCol;
    }

    private void DestroyPoolsButton()
    {
        if (m.pools.Count > 0)
        {
            if (GUILayout.Button("Destroy pools"))
            {
                EditorUtility.SetDirty(m);
                DestroyPools();
            }
        }
    }

    private void RefreshPools()
    {
        DestroyPools();
        if (m.pools.Count > 0)
        {
            for (int i = 0; i < m.pools.Count; i++)
            {
                Pool p = m.pools[i];
                if (p.prefab)
                {
                    if (!p.parent)
                    {
                        GameObject newParentGo = new GameObject(p.prefab.name + "_Pool");
                        Undo.RegisterCreatedObjectUndo(newParentGo, "New Parent");
                        newParentGo.transform.parent = m.transform;
                        p.parent = newParentGo.transform;
                    }

                    for (int amnt = 0; amnt < p.amount; amnt++)
                    {
                        GameObject newGo = PrefabUtility.InstantiatePrefab(p.prefab, p.parent) as GameObject;
                        Undo.RegisterCreatedObjectUndo(newGo, "New Prefab");
                        newGo.name = p.prefab.name + "_" + amnt;
                        newGo.SetActive(false);
                        Entity e = newGo.GetComponentInChildren<Entity>();
                        p.entities.Add(e);
                    }
                }
            }
        }
    }

    private void DestroyPools()
    {
        if (m.pools.Count > 0)
        {
            for (int i = 0; i < m.pools.Count; i++)
            {
                if (m.pools[i].entities.Count > 0)
                {
                    for (int g = 0; g < m.pools[i].entities.Count; g++)
                    {
                        if (m.pools[i].entities[g])
                        {
                            GameObject go = m.pools[i].entities[g].gameObject;
                            if (go)
                            {
                                DestroyImmediate(go);
                            }
                        }
                    }

                    m.pools[i].entities.Clear();
                }
            }
        }
    }
}
