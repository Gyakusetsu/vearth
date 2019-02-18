#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using mattatz.ProceduralFlower;
using ProceduralModeling;
using UnityEngine.Rendering;

namespace Vearth {
    
    public class TreeFlowerTab : VearthTab
    {
        Material TreeMaterial;
		int Seed = 0;
		float LengthAttenuation = 0.8f, RadiusAttenuation = 0.5f;
		int BranchesMin = 1, BranchesMax = 3;
        float GrowthAngleMin = -15f;
        float GrowthAngleMax = 15f;
        float GrowthAngleScale = 4f;
        float BranchingAngle = 15f;
		int HeightSegments = 10, RadialSegments = 8;
		float BendDegree = 0.1f;
		int Generations = 5;
		float Length = 1f;
		float Radius = 0.15f;
		PFShape LeafShape;
		Material LeafMaterial = null;
		Mesh LeafMesh;
		ShadowCastingMode LeafShadowCastingMode = ShadowCastingMode.On;
		bool LeafReceiveShadows = true;
		int LeafSeed = 0;



        float PetalDistance = 0.01f;
        int PetalsCount = 70;
        int BudsCount = 8;
        float Scale = 0.328f;
        float Minimum = 0.1f;
        float Angle = 87f;
        float AngleScale = 0.92f;
        float Offset = 0f;


        float Height = 2f;
        int LeavesCount = 6;
        float ScaleRangeMin = 0.2f;
        float ScaleRangeMax = 0.825f;
        float SegmentRangeMin = 0.2f;
        float SegmentRangeMax = 0.92f;



		ProceduralFlower ProcFlower;


        public TreeFlowerTab(string description) : base(description) {}

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

                            TreeMaterial = (Material)EditorGUILayout.ObjectField("Tree Material", TreeMaterial, typeof(Material), true);

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
                            
                            Generations = EditorGUILayout.IntSlider("Generations", Generations, 2, 8);
                                                    
                            Length = EditorGUILayout.Slider("Length", Length, 0.5f, 5f);
                                                    
                            Radius = EditorGUILayout.Slider("Radius", Radius, 0.1f, 2f);
                                                    
                            LeafShape = (PFShape)EditorGUILayout.ObjectField("Leaf Shape", LeafShape, typeof(PFShape), true);
                          
                            LeafMaterial = (Material)EditorGUILayout.ObjectField("Leaf Material", LeafMaterial, typeof(Material), true);
                     
                            LeafShadowCastingMode = (ShadowCastingMode)EditorGUILayout.EnumPopup("Shadow Casting Mode", LeafShadowCastingMode);       

                            LeafReceiveShadows = EditorGUILayout.Toggle("Receive Shadows", LeafReceiveShadows);

                            LeafSeed = EditorGUILayout.IntField("Leaf Seed", LeafSeed);

                            EditorGUILayout.Space();
                            if (GUILayout.Button("Generate", GUILayout.MinHeight(20f))) {
                                TreeData dataTemp = new TreeData();
                                dataTemp.randomSeed = Seed;
                                dataTemp.lengthAttenuation = LengthAttenuation;
                                dataTemp.radiusAttenuation = RadiusAttenuation;
                                dataTemp.branchesMin = BranchesMin;
                                dataTemp.branchesMax = BranchesMax;
                                dataTemp.growthAngleMin = GrowthAngleMin;
                                dataTemp.growthAngleMax = GrowthAngleMax;
                                dataTemp.growthAngleScale = GrowthAngleScale;
                                dataTemp.branchingAngle = BranchingAngle;
                                dataTemp.heightSegments = HeightSegments;
                                dataTemp.radialSegments = RadialSegments;
                                dataTemp.bendDegree = BendDegree;

                                GameObject objTemp = new GameObject();
                                ProceduralTree tempTree = objTemp.AddComponent(typeof(ProceduralTree)) as ProceduralTree;
                                tempTree.leafData = new ShapeData();

                                tempTree.data = dataTemp;
                                tempTree.generations = Generations;
                                tempTree.length = Length;
                                tempTree.radius = Radius;

                                tempTree.height = Radius;
                                tempTree.leafCount = LeavesCount;
                                tempTree.leafScaleRange.x = ScaleRangeMin;
                                tempTree.leafScaleRange.y = ScaleRangeMax;
                                tempTree.leafSegmentRange.x = SegmentRangeMin;
                                tempTree.leafSegmentRange.y = SegmentRangeMax;
                                tempTree.leafData.shape = LeafShape;
                                tempTree.leafData.material = LeafMaterial;
                                tempTree.leafData.shadowCastingMode = LeafShadowCastingMode;
                                tempTree.leafData.receiveShadows = LeafReceiveShadows;

                                tempTree.transform.position = Vector3.zero;
                                objTemp.GetComponent<Renderer>().material = TreeMaterial;
                                tempTree.name = "Procedural Tree";
                            }
                        } EditorGUILayout.EndVertical();
                        
                    } EditorGUILayout.EndVertical();

                } EditorGUILayout.EndVertical();

                
                // procedural tree generator
                EditorGUILayout.BeginVertical(); {

                    EditorGUILayout.LabelField("Procedural Flower Generator", EditorStyles.toolbarButton);

                    EditorGUILayout.BeginVertical(EditorStyles.textField); {
                        
                        GUIStyle SectionStyle = new GUIStyle();
                        SectionStyle.padding = new RectOffset(7, 7, 7, 7);
                        
                        EditorGUILayout.BeginVertical(SectionStyle); {
                                
                            EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
                            EditorGUILayout.Space();

                            ProcFlower = (ProceduralFlower)EditorGUILayout.ObjectField("Flower Data", ProcFlower, typeof(ProceduralFlower), true);
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
                                ProcFlower.c = PetalDistance;
                                ProcFlower.n = PetalsCount;
                                ProcFlower.m = BudsCount;
                                ProcFlower.scale = Scale;
                                ProcFlower.min = Minimum;
                                ProcFlower.angle = Angle;		
                                ProcFlower.angleScale = AngleScale;
		                        ProcFlower.offset = Offset;
			                    ProcFlower.height = Height;
			                    ProcFlower.leafCount = LeavesCount;
			                    ProcFlower.leafScaleRange.x = ScaleRangeMin;
                                ProcFlower.leafScaleRange.y = ScaleRangeMax;
                                ProcFlower.leafSegmentRange.x = SegmentRangeMin;
                                ProcFlower.leafSegmentRange.y = SegmentRangeMax;
		
                                var root = ProcFlower.Build(true);
                                root.transform.position = Vector3.zero;
                                root.GetComponent<PFPart>().Animate();
                                root.name = "Procedural Flower";
                            }
                        } EditorGUILayout.EndVertical();
                        
                    } EditorGUILayout.EndVertical();

                } EditorGUILayout.EndVertical();


            } EditorGUILayout.EndHorizontal();
        }
    }
}
#endif