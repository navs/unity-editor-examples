using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine.Events;
using UnityEditor.EditorTools;
using System.Linq;

public class ExampleEditor : EditorWindow
{
    [MenuItem("Tools/Open ExampleEditor")]
    public static void open()
    {
        ExampleEditor exampleEditor = GetWindow<ExampleEditor>();
        exampleEditor.Show();
    }

    bool foldoutEditorGUILayout = false;
    bool foldoutEditorGUI = false;
    bool foldOutSerializedObjectExample = false;
    bool foldOutEditorStylesExample = false;


    public void OnGUI()
    {
        foldoutEditorGUILayout = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutEditorGUILayout, "EditorGUILayout Examples");
        if (foldoutEditorGUILayout)
        {
            EditorGUILayoutExamples();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        foldoutEditorGUI = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutEditorGUI, "EditorGUI Examples");
        if (foldoutEditorGUI)
        {
            EditorGUIExamples();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        foldOutSerializedObjectExample = EditorGUILayout.BeginFoldoutHeaderGroup(foldOutSerializedObjectExample, "SerializedObject Examples");
        if (foldOutSerializedObjectExample)
        {
            SerializedObjectExample();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        foldOutEditorStylesExample = EditorGUILayout.BeginFoldoutHeaderGroup(foldOutEditorStylesExample, "EditorStyles Examples");
        if (foldOutEditorStylesExample)
        {
            EditorStylesExample();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    Texture2D texIcon;
    Texture2D texInverted;

    float armor = 20;
    float damage = 80;

    public void EditorGUIExamples()
    {

        Color cc = GUI.contentColor;
        GUI.contentColor = (EditorGUI.actionKey) ? Color.red : GUI.contentColor;
        EditorGUILayout.LabelField($"EditorGUI.actionKey : {EditorGUI.actionKey}");
        GUI.contentColor = cc;

        Rect rect = new Rect(0, 0, 128, 128);
        if (texIcon == null)
        {
            texIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Resources/Textures/vertex_tool.png");
        }
        rect = EditorGUILayout.GetControlRect(false, 128);
        rect.width = 128;

        EditorGUI.DrawPreviewTexture(rect, texIcon, null, ScaleMode.ScaleToFit, 0, -1, UnityEngine.Rendering.ColorWriteMask.All);

        rect.x = rect.x + 140;
        EditorGUI.DrawTextureAlpha(rect, texIcon);

        if (texInverted != null)
        {
            rect.x = rect.x + 140;
            EditorGUI.DrawPreviewTexture(rect, texInverted, null, ScaleMode.ScaleToFit, 0, -1, UnityEngine.Rendering.ColorWriteMask.All);
        }

        rect.x = rect.x + 140;
        EditorGUI.DrawRect(rect, Color.green);

        if (GUILayout.Button("Invert!") && texIcon != null) 
        {
            texInverted = new Texture2D(texIcon.width, texIcon.height, TextureFormat.RGBA32 , (texIcon.mipmapCount != 0));
            for (int m = 0; m < texIcon.mipmapCount; m++)
            {
                texInverted.SetPixels(texIcon.GetPixels(m), m);
                Color[] c = texInverted.GetPixels(m);
                for (int i = 0; i < c.Length; i ++)
                {
                    c[i].r = 1 - c[i].r;
                    c[i].g = 1 - c[i].g;
                    c[i].b = 1 - c[i].b;
                }
                texInverted.SetPixels(c, m);
            }
            texInverted.Apply();
        }

        GUILayout.Button("Hey!");
        rect = EditorGUILayout.GetControlRect();
        EditorGUI.DropShadowLabel(rect, "This is just a shadow label.");

        rect = EditorGUILayout.GetControlRect();
        armor = EditorGUI.IntSlider(rect, "Armor", Mathf.RoundToInt(armor), 0, 10000);
        rect = EditorGUILayout.GetControlRect();
        damage = EditorGUI.IntSlider(rect, "Damage", Mathf.RoundToInt(damage), 0, 10000);
        rect = EditorGUILayout.GetControlRect();
        EditorGUI.ProgressBar(rect, armor / 10000, "Armor");
        rect = EditorGUILayout.GetControlRect();
        EditorGUI.ProgressBar(rect, damage / 10000, "Damage");

    }

    Gradient gradient = new Gradient();

    bool collapsed = false;
    bool clearOnPlay = false;

    Bounds bounds;
    double doubleValue;

    AnimBool m_ShowExtraFields;
    string m_String;
    Color m_Color = Color.white;
    int m_Number = 0;

    Vector2 scrollPos;
    string t = "This is a string inside a Scroll view!";

    bool posGroupEnabled = true;
    bool[] pos = new bool[3] { true, true, true };

    EditorTool vertexTool;

    void OnEnable()
    {
        m_ShowExtraFields = new AnimBool(true);
        m_ShowExtraFields.valueChanged.AddListener(new UnityAction(base.Repaint));

        if (vertexTool == null)
        {
            vertexTool = new VertexTool();
        }
    }


    public void EditorGUILayoutExamples()
    {
        EditorGUILayout.LabelField("Hello World!");

        Rect rect = EditorGUILayout.GetControlRect(false, 50);
        GUI.Button(rect, GUIContent.none);


        gradient = EditorGUILayout.GradientField("Gradient", gradient);
        //EditorGUILayout.HelpBox("Hello, World !", MessageType.Info);
        //EditorGUILayout.HelpBox("Hello, World !", MessageType.Error);
        EditorGUILayout.HelpBox("Hello, World !", MessageType.Warning);
        //EditorGUILayout.HelpBox("Hello, World !", MessageType.None);
        EditorGUILayout.SelectableLabel("This is a selectable label");

        EditorGUILayout.DropdownButton(GUIContent.none, FocusType.Keyboard);

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal("Toolbar", GUILayout.ExpandWidth(true));
        {
            if (GUILayout.Button("Clear", "ToolbarButton", GUILayout.Width(45f)))
            {
                Debug.Log("You click Clear button");
            }
            // Create space between Clear and Collapse button.
            GUILayout.Space(5f);
            // Create toggles button.
            collapsed = GUILayout.Toggle(collapsed, "Collapse", "ToolbarButton");
            clearOnPlay = GUILayout.Toggle(clearOnPlay, "Clear on Play", "ToolbarButton");
            // Push content to be what they should be. (ex. width)
            GUILayout.FlexibleSpace();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EditorToolbar(vertexTool);

        bounds = EditorGUILayout.BoundsField("Bounds", bounds);
        doubleValue = EditorGUILayout.DelayedDoubleField("Delayed Double", doubleValue);

        BuildTargetGroup buildTargetGroup = EditorGUILayout.BeginBuildTargetSelectionGrouping();
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField($"BuildTarget:{buildTargetGroup}");
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndBuildTargetSelectionGrouping();


        using (var v = new EditorGUILayout.VerticalScope("button"))
        {
            Rect r = new Rect(v.rect);
            r.height = r.height / 2;
            if (GUI.Button(r, GUIContent.none))
                Debug.Log("Go here");
            GUILayout.Label("I'm inside the button");
            GUILayout.Label("So am I");
            GUILayout.Label($"{v.rect.width} x {v.rect.height}");
        }

        //
        m_ShowExtraFields.target = EditorGUILayout.ToggleLeft("Show extra fields", m_ShowExtraFields.target);

        //Extra block that can be toggled on and off.
        using (var group = new EditorGUILayout.FadeGroupScope(m_ShowExtraFields.faded))
        {
            if (group.visible)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PrefixLabel("Color");
                m_Color = EditorGUILayout.ColorField(m_Color);
                EditorGUILayout.PrefixLabel("Text");
                m_String = EditorGUILayout.TextField(m_String);
                EditorGUILayout.PrefixLabel("Number");
                m_Number = EditorGUILayout.IntSlider(m_Number, 0, 10);
                EditorGUI.indentLevel--;
            }
        }


        using (var h = new EditorGUILayout.HorizontalScope())
        {
            using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPos, GUILayout.Width(100), GUILayout.Height(100)))
            {
                scrollPos = scrollView.scrollPosition;
                GUILayout.Label(t);
            }
            if (GUILayout.Button("Add More Text", GUILayout.Width(100), GUILayout.Height(100)))
                t += " \nAnd this is more text!";
        }
        if (GUILayout.Button("Clear"))
            t = "";

        using (var posGroup = new EditorGUILayout.ToggleGroupScope("Align position", posGroupEnabled))
        {
            EditorGUI.indentLevel++;
            posGroupEnabled = posGroup.enabled;
            pos[0] = EditorGUILayout.Toggle("x", pos[0]);
            pos[1] = EditorGUILayout.Toggle("y", pos[1]);
            pos[2] = EditorGUILayout.Toggle("z", pos[2]);
            EditorGUI.indentLevel--;
        }
    }


    public void SerializedObjectExample()
    {
        using (var v = new EditorGUILayout.VerticalScope("box"))
        {
            EditorGUILayout.LabelField("Example1 (no undo)");
            if (GUILayout.Button("Reset (Set local position of selected objects to origin"))
            {
                foreach (var go in Selection.gameObjects)
                {
                    go.transform.localPosition = Vector3.zero;
                }
            }
        }
        EditorGUILayout.Space();

        using (var v = new EditorGUILayout.VerticalScope("box"))
        {
            EditorGUILayout.LabelField("Example2 ");
            if (GUILayout.Button("Reset (Set local position of selected objects to origin", EditorStyles.miniButton))
            {
                var transforms = Selection.gameObjects.Select(go => go.transform).ToArray();
                var so = new SerializedObject(transforms);
                so.FindProperty("m_LocalPosition").vector3Value = new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3));
                so.ApplyModifiedProperties();
            }
        }
    }

    public void EditorStylesExample()
    {
        foreach(var p in typeof(EditorStyles).GetProperties())
        {
            if (p.PropertyType == typeof(GUIStyle))
            {
                EditorGUILayout.LabelField($"{p.Name}", (GUIStyle)p.GetValue(null));
            }
        }
    }
}