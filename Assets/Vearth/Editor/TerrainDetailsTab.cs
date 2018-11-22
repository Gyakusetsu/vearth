#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Vearth3D {
    public class TerrainDetailsTab : VearthTab {
        
        public static string[] DetailRules = new string[10] {"Detail Rule 1","Detail Rule 2","Detail Rule 3","Detail Rule 4","Detail Rule 5","Detail Rule 6","Detail Rule 7","Detail Rule 8","Detail Rule 9","Detail Rule 10"};
        public static int DetailRuleIndex = 0;
        public static int[] DetailRulePrototypeID = new int[10];
        public static float[,] DetailRuleParams = new float[10,8];
        public static bool[] DetailSplatPrototypeEnable = new bool[10];
        public static int[] DetailSplatPrototypeMatch = new int[10];
        public static float[] DetailSplatPrototypeAmount = new float[10];
        
        public TerrainDetailsTab(string description) : base(description) {
            
            for (int i = 0; i < 10; i++)
            {
                DetailRuleParams[i, 1] = 1.0f;
                DetailRuleParams[i, 3] = 90.0f;
                DetailRuleParams[i, 6] = 0.5f;
            }
        }

        
        public override void ShowTabContent()
        {
            Vearth.BeginVearthBox("Terrain Details Modification"); {
                if (Vearth.SelectedTerrain != null) {     
                    EditorGUILayout.BeginVertical(); {
                        EditorGUILayout.BeginHorizontal(); {
                            DetailRuleIndex = EditorGUILayout.Popup(DetailRuleIndex, DetailRules);
                            DetailPrototype[] detaildata = Vearth.SelectedTerrain.GetComponent<Terrain>().terrainData.detailPrototypes;
                            string[] teDetailOptions = new string[detaildata.Length + 1];
                            teDetailOptions[0] = "(none)";

                            if (detaildata.Length > 0)
                            {
                                for (int i = 0; i < detaildata.Length; i++)
                                {
                                    
                                    if (detaildata[i].prototypeTexture)
                                    {
                                        teDetailOptions[i + 1] = detaildata[i].prototypeTexture.name;
                                    } else {
                                        teDetailOptions[i + 1] = detaildata[i].prototype.name;	
                                    }

                                }   
                            }
                                
                            GUILayout.Space(6);

                            DetailRulePrototypeID[DetailRuleIndex] = EditorGUILayout.Popup(DetailRulePrototypeID[DetailRuleIndex], teDetailOptions);
                        } EditorGUILayout.EndHorizontal();
                        
                        GUILayout.Space(3);
                
                        if (DetailRulePrototypeID[DetailRuleIndex] > 0)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Height", GUILayout.Width(140));
                            GUILayout.Label (DetailRuleParams[DetailRuleIndex,0].ToString("N2"), GUILayout.Width(40));
                            EditorGUILayout.MinMaxSlider(ref DetailRuleParams[DetailRuleIndex, 0], ref DetailRuleParams[DetailRuleIndex, 1], 0.0f, 1.0f);
                            GUILayout.Label(DetailRuleParams[DetailRuleIndex, 1].ToString("N2"), GUILayout.Width(40));
                            GUILayout.EndHorizontal();
                            GUILayout.Space(3);

                            GUILayout.BeginHorizontal();   
                            GUILayout.Label("Slope", GUILayout.Width(140));
                            GUILayout.Label(DetailRuleParams[DetailRuleIndex, 2].ToString("N2"), GUILayout.Width(40));
                            EditorGUILayout.MinMaxSlider(ref DetailRuleParams[DetailRuleIndex, 2], ref DetailRuleParams[DetailRuleIndex, 3], 0.0f, 90.0f);
                            GUILayout.Label(DetailRuleParams[DetailRuleIndex, 3].ToString("N2"), GUILayout.Width(40));
                            GUILayout.EndHorizontal();
                            GUILayout.Space(3);

                            DetailRuleParams[DetailRuleIndex,7] = EditorGUILayout.Slider("Coverage", DetailRuleParams[DetailRuleIndex,7],0.0f,1.0f);			

                            GUILayout.Space(3);
                            DetailRuleParams[DetailRuleIndex, 6] = EditorGUILayout.Slider("Strength", DetailRuleParams[DetailRuleIndex, 6], 1.0f, 15.0f);
                            
                            GUILayout.Space(3);
                            
                            DetailSplatPrototypeEnable[DetailRuleIndex] = EditorGUILayout.ToggleLeft("Filter By Splatmap", DetailSplatPrototypeEnable[DetailRuleIndex]);
                            if(DetailSplatPrototypeEnable[DetailRuleIndex]){
                                if(Vearth.SelectedTerrain.GetComponent<Terrain>().terrainData.splatPrototypes.Length>0){
                                    SplatPrototype[] splatdata = Vearth.SelectedTerrain.GetComponent<Terrain>().terrainData.splatPrototypes;
                                    string[] teSplatTextures = new string[splatdata.Length];
                                    if(splatdata.Length>0){
                                        for(int i=0;i<splatdata.Length;i++){
                                            teSplatTextures[i] = splatdata[i].texture.name;
                                        }
                                        GUILayout.Space(3);
                            
                                        DetailSplatPrototypeMatch[DetailRuleIndex] = EditorGUILayout.Popup("Prototype", DetailSplatPrototypeMatch[DetailRuleIndex],teSplatTextures);
                                    } 
                                } else {
                                    EditorGUILayout.LabelField ("No textures on this terrain!", EditorStyles.boldLabel);	
                                }
                                GUILayout.Space(3);
                            
                                DetailSplatPrototypeAmount[DetailRuleIndex] = EditorGUILayout.Slider("Threshold", DetailSplatPrototypeAmount[DetailRuleIndex],0.0f,1.0f);
                            }
                        } else {     
                            EditorGUILayout.LabelField ("Select a Detail for this Rule!", EditorStyles.boldLabel);	   
                        }
                        
                        EditorGUILayout.Space();
                        if (GUILayout.Button("Apply Detail Rules", GUILayout.MinHeight(30))) {
                            if (Vearth.SelectedTerrain != null) {
                                Undo.RegisterCompleteObjectUndo(Vearth.SelectedTerrain.GetComponent<Terrain>().terrainData,
                                    "vearth:Apply detail to selected terrain.");

                                EditorUtility.DisplayProgressBar("Generating Details", "Generating "
                                                    + Vearth.SelectedTerrain.name, 1f);

                                TerrainModder.ApplyDetails(Vearth.SelectedTerrain.GetComponent<Terrain>(),
                                                            DetailRulePrototypeID, DetailRuleParams,
                                                            DetailSplatPrototypeEnable, DetailSplatPrototypeMatch,
                                                            DetailSplatPrototypeAmount);

                                EditorUtility.ClearProgressBar();
                            } else {
                                EditorUtility.DisplayDialog("Vearth Error", "Please select a Terrain GameObject first!", "OK", "CANCEL");
                            }
                        }
                    } EditorGUILayout.EndVertical();
                } else {
                    EditorGUILayout.LabelField("Please select a Terrain GameObject first!", EditorStyles.boldLabel);  
                }
            } Vearth.EndVearthBox();
        }
    }
}
#endif