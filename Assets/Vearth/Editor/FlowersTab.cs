using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Vearth3D {
    
    public class FlowersTab : VearthTab
    {

        float PetalDistance;
        int PetalsCount;
        int BudsCount;
        float Scale;
        float Minimum;
        float Angle;
        float AngleScale;
        float Offset;
        float Height;
        int LeavesCount;
        float ScaleRangeMin;
        float ScaleRangeMax;
        float SegmentRangeMin;
        float SegmentRangeMax;




        public FlowersTab(string description) : base(description) {}

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

                            PetalDistance = EditorGUILayout.Slider("Petal distance from center", PetalDistance, 0.001f, 0.05f);
                            PetalsCount = EditorGUILayout.IntSlider("# of petals", PetalsCount, 4, 200);
                            BudsCount = EditorGUILayout.IntSlider("# of buds", BudsCount, 0, PetalsCount);
                            Scale = EditorGUILayout.Slider("Scale", Scale, 0.1f, 0.6f);
                            Minimum = EditorGUILayout.Slider("Min", Minimum, 0.0f, 1f);
                            Angle = EditorGUILayout.Slider("Angle", Angle, 30f, 100f);
                            AngleScale = EditorGUILayout.Slider("Angle Scale", AngleScale, 0.1f, 1.5f);
                            Offset = EditorGUILayout.Slider("Offset", Offset, 0f, 1f);
                            Height = EditorGUILayout.Slider("Height", Height, 1f, 10f);
                            LeavesCount = EditorGUILayout.IntSlider("# of leafs", LeavesCount, 0, 10);
                            ScaleRangeMin = EditorGUILayout.Slider("Leaf scale range min", ScaleRangeMin, 0.1f, 0.9f);
                            ScaleRangeMax = EditorGUILayout.Slider("Leaf scale range max", ScaleRangeMax, ScaleRangeMin, 0.95f);
                            SegmentRangeMin = EditorGUILayout.Slider("Leaf segment range min",SegmentRangeMin, 0.1f, 0.9f);
                            SegmentRangeMax = EditorGUILayout.Slider("Leaf segment range max",SegmentRangeMax, SegmentRangeMin, 0.95f);
                    

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

                    EditorGUILayout.LabelField("Flowers to Spawn", EditorStyles.toolbarButton);

                    EditorGUILayout.BeginVertical(EditorStyles.textField); {
                        
                        GUIStyle SectionStyle = new GUIStyle();
                        SectionStyle.padding = new RectOffset(13, 13, 13, 13);
                        
                        EditorGUILayout.BeginVertical(SectionStyle); {
                            Vearth vearth = EditorWindow.GetWindow<Vearth>();

                            if(vearth.m_FlowersObjectsSO != null)
                            {
                                vearth.m_FlowersObjectsSO.Update();
                                vearth.m_FlowersReorderableList.DoLayoutList();
                                vearth.m_FlowersObjectsSO.ApplyModifiedProperties();
                            }
                        } EditorGUILayout.EndVertical();
                        
                    } EditorGUILayout.EndVertical();

                } EditorGUILayout.EndVertical();


            } EditorGUILayout.EndHorizontal();
        }
    }
}