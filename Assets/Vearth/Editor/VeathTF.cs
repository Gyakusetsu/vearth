#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;

namespace Vearth {

    public class VearthTF : EditorWindow
    {
        Vector2 scrollPos;

        static TreeFlowerTab treeFlowerTab;

        void OnEnable() {
            treeFlowerTab = new TreeFlowerTab("Generate a Procedural Tree or Flower");
        }

        // Add menu named "My Window" to the Window menu
        [MenuItem("Vearth/Vearth TF")]
        static void Init()
        {
            // Get existing open window or if none, make a new one:
            VearthTF window = (VearthTF)EditorWindow.GetWindow(typeof(VearthTF), false, "Vearth TF");
            window.minSize = new Vector2(450, 300);
            window.Show();
        }

        void OnGUI()
        {

            GUIStyle tabStyle = new GUIStyle(GUI.skin.scrollView);
            tabStyle.margin = new RectOffset(7, 7, 7, 7);

            EditorGUILayout.BeginVertical(tabStyle); {
                // Current Tab Checker
                EditorGUILayout.Space();

                scrollPos = EditorGUILayout.BeginScrollView(scrollPos); {
                    
                    // Show Description
                    EditorGUILayout.BeginVertical(EditorStyles.textField);
                    GUILayout.Label(treeFlowerTab.Description, EditorStyles.wordWrappedLabel);
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Space();

                    treeFlowerTab.ShowTabContent();

                } EditorGUILayout.EndScrollView();
                
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Vearth : Unique-Terrain Scene Generating System Using Noise and Fractals for Unity3D", 
                        EditorStyles.toolbarButton);

            } EditorGUILayout.EndVertical();
        }
    }
}
#endif