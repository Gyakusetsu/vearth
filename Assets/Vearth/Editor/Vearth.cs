#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;

namespace Vearth3D {

    [System.Serializable]
    public class Vearth : EditorWindow
    {
        int toolbarIndex = 0;

        string[] toolbarTitles = {"Terrain", "Terrain Details", "Terrain Stitch", "Trees Details"};

        Vector2 scrollPos;

        static VearthTab[] vearthTabs = new VearthTab[4];
        
        public static GameObject SelectedTerrain;

        [SerializeField]
        public List<GameObject> TreesObjects = new List<GameObject>();
        
        [SerializeField]
        public List<GameObject> FlowersObjects = new List<GameObject>();

        public SerializedObject m_TreesObjectsSO = null;

        public ReorderableList m_TreesReorderableList = null;
        
        public SerializedObject m_FlowersObjectsSO = null;

        public ReorderableList m_FlowersReorderableList = null;

        void OnEnable() {
            vearthTabs[0] = new TerrainTab("Configure Terrain GameObject from the Following Settings:");
            vearthTabs[1] = new TerrainDetailsTab("Generate detail layers for your terrain(s) with a stack of 10 height/slope rules.");
            vearthTabs[2] = new TerrainStitchTab("Stitch Terrain with the same Height, Width and Resolution only! Please use only with terrains that contains small details.");
            vearthTabs[3] = new TreesDetailsTab("Genarates a Tree GameObject that you could put to your terrain" );
        //    vearthTabs[3] = new FlowersTab("Genarates a Flower GameObject that you could put to your terrain");
 
                
            Vearth target = this;
            int TOP_PADDING = 2;

            m_TreesObjectsSO = new SerializedObject(target);
            m_TreesReorderableList = new ReorderableList(m_TreesObjectsSO, m_TreesObjectsSO.FindProperty("TreesObjects"), true, true, true, true);
        
            m_TreesReorderableList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Trees");
            m_TreesReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.y += TOP_PADDING;
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, m_TreesReorderableList.serializedProperty.GetArrayElementAtIndex(index), GUIContent.none);
            };

            
            m_FlowersObjectsSO = new SerializedObject(target);
            m_FlowersReorderableList = new ReorderableList(m_FlowersObjectsSO, m_FlowersObjectsSO.FindProperty("FlowersObjects"), true, true, true, true);
        
            m_FlowersReorderableList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Flowers");
            m_FlowersReorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.y += TOP_PADDING;
                rect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(rect, m_FlowersReorderableList.serializedProperty.GetArrayElementAtIndex(index), GUIContent.none);
            };
        }

        // Add menu named "My Window" to the Window menu
        [MenuItem("Vearth/Vearth Editor")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            Vearth window = (Vearth)EditorWindow.GetWindow(typeof(Vearth), false, "Vearth 1.0");
            window.minSize = new Vector2(450, 300);
            window.Show();
        }

        void OnGUI()
        {

            if (Selection.activeGameObject != null) {  
                if(Selection.activeGameObject.GetComponent<Terrain>() != null) {
                    if (SelectedTerrain != Selection.activeGameObject) {
                        SelectedTerrain = Selection.activeGameObject;
                    }
                } else {
                    SelectedTerrain = null;
                }
            } else {
                SelectedTerrain = null;
            }

            GUIStyle tabStyle = new GUIStyle(GUI.skin.scrollView);
            tabStyle.margin = new RectOffset(7, 7, 7, 7);

            EditorGUILayout.BeginVertical(tabStyle); {
                // Current Tab Checker
                toolbarIndex = GUILayout.Toolbar(toolbarIndex, toolbarTitles);
                
                EditorGUILayout.Space();

                scrollPos = EditorGUILayout.BeginScrollView(scrollPos); {
                    
                    // Show Description
                    EditorGUILayout.BeginVertical(EditorStyles.textField);
                    GUILayout.Label(vearthTabs[toolbarIndex].Description, EditorStyles.wordWrappedLabel);
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space();

                    vearthTabs[toolbarIndex].ShowTabContent();

                } EditorGUILayout.EndScrollView();
                
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Vearth : Unique-Terrain Scene Generating System Using Noise and Fractals for Unity3D", 
                        EditorStyles.toolbarButton);

            } EditorGUILayout.EndVertical();
        }

        public static void BeginVearthBox(string label) {
            EditorGUILayout.LabelField(label, EditorStyles.toolbarButton);

            EditorGUILayout.BeginVertical(EditorStyles.textField); 
                    
            GUIStyle SectionStyle = new GUIStyle();
            SectionStyle.padding = new RectOffset(7, 7, 7, 7);

            EditorGUILayout.BeginVertical(SectionStyle); 
                    
        }

        public static void EndVearthBox() {
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }
    }

}
#endif