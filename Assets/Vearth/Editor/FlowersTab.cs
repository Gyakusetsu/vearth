#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Vearth3D {
    
    public class FlowersTab : VearthTab
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
                        SectionStyle.padding = new RectOffset(7, 7, 7, 7);
                        
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

                
                // procedural tree generator
                EditorGUILayout.BeginVertical(); {

                    EditorGUILayout.LabelField("Procedural Tree Generator", EditorStyles.toolbarButton);

                    EditorGUILayout.BeginVertical(EditorStyles.textField); {
                        
                        GUIStyle SectionStyle = new GUIStyle();
                        SectionStyle.padding = new RectOffset(7, 7, 7, 7);
                        
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


            } EditorGUILayout.EndHorizontal();
        }
    }
}
#endif