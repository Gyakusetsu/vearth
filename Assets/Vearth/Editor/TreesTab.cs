using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Vearth3D {
    public class TreesTab : VearthTab
    {
		int Seed = 0;
		float LengthAttenuation = 0.8f, RadiusAttenuation = 0.5f;
		int BranchesMin = 1, BranchesMax = 3;
        float GrowthAngleMin = -15f;
        float GrowthAngleMax = 15f;
        float GrowthAngleScale = 4f;
        float BranchingAngle = 15f;
		int HeightSegments = 10, RadialSegments = 8;
		float BendDegree = 0.1f;


        public TreesTab(string description) : base(description) {}

        public override void ShowTabContent()
       { 
            EditorGUILayout.BeginHorizontal(); {
                
                // procedural tree generator
                EditorGUILayout.BeginVertical(); {

                    EditorGUILayout.LabelField("Procedural Tree Generator", EditorStyles.toolbarButton);

                    EditorGUILayout.BeginVertical(EditorStyles.textField); {
                        
                        GUIStyle SectionStyle = new GUIStyle();
                        SectionStyle.padding = new RectOffset(13, 13, 13, 13);
                        
                        EditorGUILayout.BeginVertical(SectionStyle); {
                                
                            EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
                            EditorGUILayout.Space();

                            Seed = EditorGUILayout.IntSlider("Seed", Seed, 0, 65535);

                            LengthAttenuation = EditorGUILayout.Slider("Length Attenuation", 
                                    LengthAttenuation, 0.25f, 0.95f);
                                    
                            RadiusAttenuation = EditorGUILayout.Slider("Radius Attenuation", 
                                    RadiusAttenuation, 0.25f, 0.95f);

                            BranchesMin = EditorGUILayout.IntSlider("Branches Min", BranchesMin, 1, 3);

                            BranchesMax = EditorGUILayout.IntSlider("Branches Max", BranchesMax, 1, 3);
                            
                            GrowthAngleMin = EditorGUILayout.Slider("Angle Min", GrowthAngleMin, -45f, 0f);
                            
                            GrowthAngleMax = EditorGUILayout.Slider("Angle Max", GrowthAngleMax, 0f, 45f);
                            
                            GrowthAngleScale = EditorGUILayout.Slider("Angle Scale", GrowthAngleScale, 1f, 10f);

                            BranchingAngle = EditorGUILayout.Slider("Branching Angle", BranchingAngle, 0f, 45f);
                            
                            HeightSegments = EditorGUILayout.IntSlider("Height Segments", HeightSegments, 4, 20);

                            RadialSegments = EditorGUILayout.IntSlider("Radial Segments", RadialSegments, 4, 20);

                            BendDegree = EditorGUILayout.Slider("Bend Degree", BendDegree, 0.0f, 0.35f);
                            
                            EditorGUILayout.Space();
                            if (GUILayout.Button("Generate", GUILayout.MinHeight(20f))) {
                                Debug.Log("Generate Me");
                            }
                        } EditorGUILayout.EndVertical();
                        
                    } EditorGUILayout.EndVertical();

                } EditorGUILayout.EndVertical();


                GUILayout.Space(13f);

                
                // Trees to Spawn
                EditorGUILayout.BeginVertical(); {

                    EditorGUILayout.LabelField("Trees to Spawn", EditorStyles.toolbarButton);

                    EditorGUILayout.BeginVertical(EditorStyles.textField); {
                        
                        GUIStyle SectionStyle = new GUIStyle();
                        SectionStyle.padding = new RectOffset(13, 13, 13, 13);
                        
                        EditorGUILayout.BeginVertical(SectionStyle); {
                            Vearth vearth = EditorWindow.GetWindow<Vearth>();

                            if(vearth.m_TreesObjectsSO != null)
                            {
                                vearth.m_TreesObjectsSO.Update();
                                vearth.m_ReorderableList.DoLayoutList();
                                vearth.m_TreesObjectsSO.ApplyModifiedProperties();
                            }
                        } EditorGUILayout.EndVertical();
                        
                    } EditorGUILayout.EndVertical();

                } EditorGUILayout.EndVertical();


            } EditorGUILayout.EndHorizontal();
        }

        /* 
        public void TreesDragAndDrop() {
            Rect myRect = GUILayoutUtility.GetRect(0,20,GUILayout.ExpandWidth(true));
            GUI.Box(myRect,"Drag and Drop Prefabs to this Box!");
            if (myRect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.DragUpdated)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    Debug.Log("Drag Updated!");
                    Event.current.Use ();
                }   
                else if (Event.current.type == EventType.DragPerform)
                {
                    Debug.Log("Drag Perform!");
                    Debug.Log(DragAndDrop.objectReferences.Length);
                    for(int i = 0; i<DragAndDrop.objectReferences.Length;i++)
                    {
                        if (!TreesObjects.Contains(DragAndDrop.objectReferences[i] as GameObject)) {
                            TreesObjects.Add(DragAndDrop.objectReferences[i] as GameObject);
                        }
                   }
                    Event.current.Use ();
                }
            }
        }
        */
    }
}