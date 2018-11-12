using UnityEditor;
using UnityEngine;
using LibNoise;
using LibNoise.Generator;


namespace Vearth3D {
    public class TerrainTab : VearthTab
    {
        int SelectedNoise = 0;

        string[] NoiseTypes = {"Perlin Noise", "Billow Noise", "Ridged Multifractal Noise"};

        int Seed = 1;

        float Frequency = 2.0f;

        float Lacunarity = 2.0f;
        
        float Persistance = 0.5f;

        int Octaves = 6;

        float Displacement = 1.0f;

        float Heights = 0.5f;

        float Alpha = 1.0f;

        int previewRedrawTime = 1;

        Texture2D previewTexture;

        Noise2D previewNoise;

        ModuleBase previewModule;

        int TempInt;

        float TempFloat;
            
        int resolution = 128;

        string [] SplatTextureNames;

        int [] SplatElevationTextureID = new int[12];

        float [] SplatElevationHeight = new float[12] { 0.01f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 0.99f, 1f };

        int [] SplatSlopeTextureID = new int[5];

        float [] SplatSlopeSteepness = new float[5] { 60.0f, 70f, 80f, 90f, 90f };

        bool UseFlowTexture = false;

        float [] FlowTextureParams = new float[5] {8f,0.2f,0.18f,1.0f,45f};

        int FlowTextureID = 0;

        public TerrainTab(string description) : base(description) {
            Generate2DNoise(SelectedNoise);
        }

        public override void ShowTabContent()
        {
            EditorGUILayout.BeginVertical(); {

                EditorGUILayout.LabelField("Noise Settings", EditorStyles.toolbarButton);

                EditorGUILayout.BeginVertical(EditorStyles.textField); {
                        
                    GUIStyle SectionStyle = new GUIStyle();
                    SectionStyle.padding = new RectOffset(7, 7, 7, 7);

                    EditorGUILayout.BeginVertical(SectionStyle); {
                        
                        EditorGUILayout.BeginHorizontal(); {

                            EditorGUILayout.BeginVertical(); {
                                
                                EditorGUI.BeginChangeCheck ();

                                EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
                                EditorGUILayout.Space();

                                SelectedNoise = EditorGUILayout.Popup("Type / Algorithm", SelectedNoise, NoiseTypes);

                                    GUILayout.Space(3);
                                Seed = EditorGUILayout.IntSlider("Seed", Seed, 0, 65535);

                                    GUILayout.Space(3);
                                Frequency = EditorGUILayout.Slider("Frequency", Frequency, 0.01f, 4f);

                                    GUILayout.Space(3);
                                Lacunarity = EditorGUILayout.Slider("Lacunarity", Lacunarity, 0.1f, 4f);

                                    GUILayout.Space(3);
                                if (!NoiseTypes[SelectedNoise].Equals("Ridged Multifractal Noise")) {
                                    Persistance = EditorGUILayout.Slider("Persistance", Persistance, 0.1f, 1.0f);
                                }

                                    GUILayout.Space(3);
                                Octaves = EditorGUILayout.IntSlider("Octaves", Octaves, 1, 10);

                                // check if changed
                                if (EditorGUI.EndChangeCheck ()) {
                                    Generate2DNoise(SelectedNoise);
                                }
                                
                                    GUILayout.Space(3);
                                Heights = EditorGUILayout.Slider("Heights", Heights, 0.1f, 1.0f);
                                
                                    GUILayout.Space(3);
                                Alpha = EditorGUILayout.Slider("Alpha", Alpha, 0.1f, 1.0f);
                            } EditorGUILayout.EndVertical();
                            
                            GUILayout.Space(13f);

                            EditorGUILayout.BeginVertical(GUILayout.MaxWidth(resolution + 3)); {
                                GUILayout.Box(previewTexture);
                                EditorGUILayout.Space();
                                if (GUILayout.Button("Apply Noise", GUILayout.MinHeight(30))) {
                                    if (Vearth.SelectedTerrain != null) {
                                        Undo.RegisterCompleteObjectUndo(Vearth.SelectedTerrain.GetComponent<Terrain>().terrainData,
                                            "vearth:Generate All Heightmaps");

                                        EditorUtility.DisplayProgressBar("Generating Terrain", "Generating "
                                                            + Vearth.SelectedTerrain.name, 1f);
                                        TerrainModder.ApplyHeightMap(Vearth.SelectedTerrain.GetComponent<Terrain>(),
                                                                    previewModule, Heights, Alpha);
                                        EditorUtility.ClearProgressBar();
                                    } else {
                                        EditorUtility.DisplayDialog("Vearth Error", "Please select a Terrain GameObject first!", "OK", "CANCEL");
                                    }
                                }
                            } EditorGUILayout.EndVertical(); 

                        } EditorGUILayout.EndHorizontal();
                    } EditorGUILayout.EndVertical();
                } EditorGUILayout.EndVertical();
                
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("SplatMap Texture Settings", EditorStyles.toolbarButton);

                EditorGUILayout.BeginVertical(EditorStyles.textField); {
                        
                    GUIStyle SectionStyle = new GUIStyle();
                    SectionStyle.padding = new RectOffset(7, 7, 7, 7);

                    EditorGUILayout.BeginVertical(SectionStyle); {
                        EditorGUILayout.BeginVertical(); {
                            
                            if (Vearth.SelectedTerrain != null) {
                                SplatPrototype[] splatdata = Vearth.SelectedTerrain.GetComponent<Terrain>().terrainData.splatPrototypes;
                                SplatTextureNames = new string[splatdata.Length];

                                if (splatdata.Length > 0) {
                   
                                    for (int i = 0; i < splatdata.Length; i++)
                                    {
                                        SplatTextureNames[i] = splatdata[i].texture.name;
                                    }


                                    EditorGUILayout.LabelField("Elevations per Scale: ", EditorStyles.boldLabel);
                                    EditorGUILayout.Space();


                                    EditorGUILayout.BeginHorizontal(); {  
                                        SplatElevationTextureID[0] = EditorGUILayout.Popup(SplatElevationTextureID[0], 
                                                                        SplatTextureNames, GUILayout.Width(120f));

                                        SplatElevationHeight[0] = EditorGUILayout.Slider(SplatElevationHeight[0], 0.0f, 1.0f);
                                        SplatElevationHeight[0] = Mathf.Clamp(SplatElevationHeight[0], 0.0f, 1.0f);
                                        SplatElevationHeight[1] = Mathf.Clamp(SplatElevationHeight[1], SplatElevationHeight[0], 1.0f);
                                    } EditorGUILayout.EndHorizontal();

                                    for (int i = 1; i < 9; i++) {
                                        GUILayout.Space(3);
                                        SplatSetting(i);
                                    }
                                    GUILayout.Space(3);

                                    EditorGUILayout.BeginHorizontal(); {
                                        SplatElevationTextureID[9] = EditorGUILayout.Popup(SplatElevationTextureID[9], 
                                                                        SplatTextureNames, GUILayout.Width(120f));
                                        SplatElevationHeight[9] = EditorGUILayout.Slider(SplatElevationHeight[9], 0.0f, 1.0f);
                                        SplatElevationHeight[9] = Mathf.Clamp(SplatElevationHeight[9], SplatElevationHeight[8], 1.0f);

                                        SplatElevationTextureID[10] = SplatElevationTextureID[9];
                                        SplatElevationHeight[10] = SplatElevationHeight[9];
                                    } EditorGUILayout.EndHorizontal();

                                    EditorGUILayout.Space();
                                    EditorGUILayout.LabelField("Slope per Scale: ", EditorStyles.boldLabel);
                                    EditorGUILayout.Space();

                                    EditorGUILayout.BeginHorizontal(); {
                                        GUILayout.Label("(Elevation Texture)", GUILayout.Width(120f));
                                        SplatSlopeSteepness[0] = EditorGUILayout.Slider(SplatSlopeSteepness[0], 0.0f, 90.0f);
                                        SplatSlopeSteepness[0] = Mathf.Clamp(SplatSlopeSteepness[0], 0.0f, 90.0f);
                                        SplatSlopeSteepness[1] = Mathf.Clamp(SplatSlopeSteepness[1], SplatSlopeSteepness[0], 90.0f);
                                    } EditorGUILayout.EndHorizontal();

                                        GUILayout.Space(3);
                                    EditorGUILayout.BeginHorizontal(); {
                                        SplatSlopeTextureID[1] = EditorGUILayout.Popup(SplatSlopeTextureID[1], 
                                                                            SplatTextureNames, GUILayout.Width(120f));
                                        SplatSlopeSteepness[1] = EditorGUILayout.Slider(SplatSlopeSteepness[1], 0.0f, 90.0f);
                                        SplatSlopeSteepness[1] = Mathf.Clamp(SplatSlopeSteepness[1], 0.0f, 90.0f);
                                        SplatSlopeSteepness[2] = Mathf.Clamp(SplatSlopeSteepness[2], SplatSlopeSteepness[1], 90.0f);
                                    } EditorGUILayout.EndHorizontal();

                                        GUILayout.Space(3);
                                    EditorGUILayout.BeginHorizontal(); {
                                        SplatSlopeTextureID[2] = EditorGUILayout.Popup(SplatSlopeTextureID[2], 
                                                                            SplatTextureNames, GUILayout.Width(120f));
                                        SplatSlopeSteepness[2] = EditorGUILayout.Slider(SplatSlopeSteepness[2], 0.0f, 90.0f);
                                        SplatSlopeSteepness[2] = Mathf.Clamp(SplatSlopeSteepness[2], 0.0f, 90.0f);
                                        SplatSlopeSteepness[3] = Mathf.Clamp(SplatSlopeSteepness[3], SplatSlopeSteepness[2], 90.0f);
                                    } EditorGUILayout.EndHorizontal();

                                        GUILayout.Space(3);
                                    EditorGUILayout.BeginHorizontal(); {
                                        SplatSlopeTextureID[3] = EditorGUILayout.Popup(SplatSlopeTextureID[3], 
                                                                            SplatTextureNames, GUILayout.Width(120f));
                                        SplatSlopeSteepness[3] = EditorGUILayout.Slider(SplatSlopeSteepness[3], 0.0f, 90.0f);
                                        SplatSlopeSteepness[3] = Mathf.Clamp(SplatSlopeSteepness[3], 0.0f, 90.0f);
                                    
                                        SplatSlopeTextureID[4] = SplatSlopeTextureID[3];
                                        SplatSlopeSteepness[4] = SplatSlopeSteepness[3];
                                    } EditorGUILayout.EndHorizontal();

                                    EditorGUILayout.Space();
                                    
                                    UseFlowTexture = EditorGUILayout.BeginToggleGroup("Flow-Map Texturing", UseFlowTexture); {
                                        
                                        EditorGUILayout.Space();
                                        FlowTextureID = EditorGUILayout.Popup(FlowTextureID, SplatTextureNames, GUILayout.Width(120)); 
                                        GUILayout.Space(3);
                                        FlowTextureParams[0] = EditorGUILayout.Slider("Iterations",FlowTextureParams[0],1f,100f);
                                        GUILayout.Space(3);
                                        FlowTextureParams[3] = EditorGUILayout.Slider("Initial",FlowTextureParams[3],0.0f,5.0f);
                                        GUILayout.Space(3);
                                        FlowTextureParams[1] = EditorGUILayout.Slider("Push Down",FlowTextureParams[1],0.05f,1.0f);
                                        GUILayout.Space(3);
                                        FlowTextureParams[2] = EditorGUILayout.Slider("Pull Down",FlowTextureParams[2],0.05f,1.0f);
                                        GUILayout.Space(3);
                                        FlowTextureParams[4] = EditorGUILayout.Slider("Min Slope",FlowTextureParams[4],0.05f,90.0f);
                                        GUILayout.Space(3);
                                    } EditorGUILayout.EndToggleGroup();

                                    EditorGUILayout.Space();
                                    if (GUILayout.Button("Apply Textures", GUILayout.MinHeight(30))) {
                                        if (Vearth.SelectedTerrain != null) {
                                            Undo.RegisterCompleteObjectUndo(Vearth.SelectedTerrain.GetComponent<Terrain>().terrainData,
                                                "vearth:Generate Splatmap For Selected Terrain");

                                            EditorUtility.DisplayProgressBar("Generating SplatMaps", "Generating "
                                                                + Vearth.SelectedTerrain.name, 1f);

                                            TerrainModder.ApplyTextures(Vearth.SelectedTerrain.GetComponent<Terrain>(),
                                                                        SplatElevationTextureID, SplatElevationHeight,
                                                                        SplatSlopeTextureID, SplatSlopeSteepness, UseFlowTexture,
                                                                        FlowTextureID, FlowTextureParams);
                                            EditorUtility.ClearProgressBar();
                                        } else {
                                            EditorUtility.DisplayDialog("Vearth Error", "Please select a Terrain GameObject first!", "OK", "CANCEL");
                                        }
                                    }
                                } else {
                                    EditorGUILayout.LabelField("Selected terrain has no textures assigned.", EditorStyles.boldLabel);
                                }
                                
                              
                            } else {       
                                EditorGUILayout.LabelField("Please select a Terrain GameObject first!", EditorStyles.boldLabel);
                            }
                        } EditorGUILayout.EndVertical();
                    } EditorGUILayout.EndVertical();
                } EditorGUILayout.EndVertical();
            } EditorGUILayout.EndVertical();
        }   

        private void SplatSetting(int index) {
            EditorGUILayout.BeginHorizontal(); {
                SplatElevationTextureID[index] = EditorGUILayout.Popup(SplatElevationTextureID[index], 
                                                SplatTextureNames, GUILayout.Width(120f));
                SplatElevationHeight[index] = EditorGUILayout.Slider(SplatElevationHeight[index], 0.0f, 1.0f);
                SplatElevationHeight[index] = Mathf.Clamp(SplatElevationHeight[index], SplatElevationHeight[index - 1], 1.0f);
                SplatElevationHeight[index + 1] = Mathf.Clamp(SplatElevationHeight[index + 1], SplatElevationHeight[index], 1.0f);
            } EditorGUILayout.EndHorizontal();
        }

        private void Generate2DNoise (int noise) {
            switch (noise) {
                case 0:
                    previewModule = new Perlin(Frequency, Lacunarity, Persistance, 
                                    Octaves, Seed, QualityMode.High);
                    break;
                case 1:
                    previewModule = new Billow(Frequency, Lacunarity, Persistance, 
                                    Octaves, Seed, QualityMode.High);
                    break;
                case 2:
                    previewModule = new RidgedMultifractal(Frequency, Lacunarity, 
                                    Octaves, Seed, QualityMode.High);
                    break;
            }

            int xoffset = 0; int yoffset = 0;
            float zoom = 1f;
            previewNoise = new Noise2D(resolution, resolution, previewModule);
            float x1 = xoffset * zoom;
            float x2 = (xoffset * zoom) + ((zoom / resolution) * (resolution + 1));
            float y1 = -yoffset * zoom;
            float y2 = (-yoffset * zoom) + ((zoom / resolution) * (resolution + 1));
            previewNoise.GeneratePlanar(x1, x2, y1, y2);
            previewTexture = previewNoise.GetTexture();
            previewTexture.Apply();
        }
    }
}