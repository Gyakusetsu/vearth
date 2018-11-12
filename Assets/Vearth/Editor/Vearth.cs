using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;

namespace Vearth3D {

    [System.Serializable]
    public class Vearth : EditorWindow
    {
        int toolbarIndex = 0;

        string[] toolbarTitles = {"Terrain", "Trees", "Flowers", "Generate"};

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
        [MenuItem("Window/Vearth")]
        static void Init()
        {

            vearthTabs[0] = new TerrainTab("Configure Terrain GameObject from the Following Settings:");
            vearthTabs[1] = new TreesTab("Genarates a Tree GameObject that you could put to your terrain" );
            vearthTabs[2] = new FlowersTab("Genarates a Flower GameObject that you could put to your terrain");
            vearthTabs[3] = new TerrainTab("");

            // Get existing open window or if none, make a new one:
            Vearth window = (Vearth)EditorWindow.GetWindow(typeof(Vearth), false, "Vearth");
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
            tabStyle.margin = new RectOffset(13, 13, 13, 13);

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
    }

}
