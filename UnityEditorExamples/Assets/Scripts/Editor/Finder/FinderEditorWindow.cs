using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.CodeDom.Compiler;
using System.Linq;
using System.ComponentModel;

public class FinderEditorWindow : EditorWindow
{
    [MenuItem("Tools/Open Finder")]
    public static void Open()
    {
        var window = GetWindow<FinderEditorWindow>();
        window.Show();
    }

    List<(Material m, bool isPersitent, string assetPath)> results;
    List<(Material m, bool isPersitent, string assetPath)> filters;

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Finder !", EditorStyles.whiteBoldLabel);

        if (GUILayout.Button("Find All Materials"))
        {
            results = new List<(Material m, bool isPersitent, string assetPath)>();
            foreach (Material m in Resources.FindObjectsOfTypeAll(typeof(Material)) as Material[])
            { 
                if (m != null)
                {
                    bool isPersistent = EditorUtility.IsPersistent(m);
                    string assetPath = "";
                    if (isPersistent)
                    {
                        assetPath = AssetDatabase.GetAssetPath(m);
                    }
                    results.Add((m, isPersistent, assetPath));
                }
            }
            filters = results.Where(Filter).ToList();
        }

        GUIResultOptions();
        GUIResult();
    }

    bool showPersistentOnly = false;
    bool showMatOnly = false;
    bool showAssetsOnly = false;
    bool showShaderOnly = false;
    string shaderName = "";

    void GUIResultOptions()
    {
        using (var h = new EditorGUILayout.HorizontalScope("Toolbar", GUILayout.ExpandWidth(true)))
        {
            EditorGUILayout.LabelField("-- Filter --", EditorStyles.boldLabel);

            if (GUILayout.Button(" Select All "))
            {
                Selection.objects = filters.Select(f => f.m).ToArray();
            }
            
            bool changed = GUI.changed;
            GUI.changed = false;

            GUILayout.Space(5);
            showPersistentOnly = GUILayout.Toggle(showPersistentOnly, "Persistent", "ToolbarButton");
            GUILayout.Space(5);
            showAssetsOnly = GUILayout.Toggle(showAssetsOnly, "Assets/", "ToolbarButton");
            GUILayout.Space(5);
            showMatOnly = GUILayout.Toggle(showMatOnly, ".mat", "ToolbarButton");
            GUILayout.Space(5);
            showShaderOnly = GUILayout.Toggle(showShaderOnly, $"shader:{shaderName}", "ToolbarButton");
            GUILayout.Space(5);

            GUILayout.FlexibleSpace();

            if (GUI.changed && results != null)
            {
                filters = results.Where(Filter).ToList();
            }

            GUI.changed |= changed;
        }
    }
    bool Filter((Material m, bool isPersitent, string assetPath) result)
    {
        return result.m != null &&
            (!showPersistentOnly || result.isPersitent) &&
            (!showAssetsOnly || result.assetPath.StartsWith("Assets/")) &&
            (!showMatOnly || result.assetPath.EndsWith(".mat")) &&
            (!showShaderOnly || result.m.shader.name == shaderName);
    }

    Vector2 posResult;
    private void GUIResult()
    {
        if (results == null || results.Count == 0)
        {
            EditorGUILayout.HelpBox("No result.", MessageType.Warning);
            return;
        }
        if (filters == null || filters.Count == 0)
        {
            EditorGUILayout.HelpBox($"No Filtered result. (0 / {results.Count})", MessageType.Warning);
            return;
        }
        EditorGUILayout.LabelField($"Total {results.Count} result(s). Filtered {filters.Count}");


        using (var s = new EditorGUILayout.ScrollViewScope(posResult))
        {
            posResult = s.scrollPosition;
            using (var v = new EditorGUILayout.VerticalScope())
            {
                foreach (var result in filters)
                {
                    using (var h = new EditorGUILayout.HorizontalScope())
                    {
                        var alignment = GUI.skin.button.alignment;
                        GUI.skin.button.alignment = TextAnchor.MiddleLeft;
                        if (GUILayout.Button(result.m.name, GUILayout.Width(400)))
                        {
                            EditorGUIUtility.PingObject(result.m);
                        }
                        GUI.skin.button.alignment = alignment;

                        if (result.isPersitent)
                        {
                            EditorGUILayout.LabelField("P", EditorStyles.miniButtonMid, GUILayout.Width(20));
                        }
                        else
                        {
                            GUILayout.Space(20);
                        }
                        if (GUILayout.Button(result.m.shader.name, EditorStyles.centeredGreyMiniLabel, GUILayout.Width(120)))
                        {
                            shaderName = result.m.shader.name;
                        }
                        EditorGUILayout.LabelField(result.assetPath, EditorStyles.miniLabel);
                        GUILayout.FlexibleSpace();
                    }
                }
            }
        }
        
    }
}
