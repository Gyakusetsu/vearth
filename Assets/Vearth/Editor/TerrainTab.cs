#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;


namespace Vearth {
    public class TerrainTab : VearthTab
    {
        int[] SelectedNoise = new int [20];

        string[] NoiseTypes = {"Perlin Noise", "Billow Noise", "Ridged Multifractal Noise"};

        int[] Seed = new int[20] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1};

        float[] Frequency = new float[20] {2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f};

        float[] Lacunarity = new float[20] {2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f};
        
        float[] Persistance = new float[20] {0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f};

        int[] Octaves = new int[20] {6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6};

        float[] Displacement = new float[20] {1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f};

        float Heights = 0.5f;

        float Alpha = 1.0f;

        Texture2D previewTexture;

        Noise2D previewNoise;

        ModuleBase[] moduleBase = new ModuleBase[20];

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

        bool ElevationShow = false;

        bool SlopeShow = false;

        public static float[] noiseFuncMin = new float[20];
        public static float[] noiseFuncMax = new float[20];
        public static int teNoiseChanIndex = 0;
        public static int[] teNoiseChanTypeIndex = new int[20];
        public static string[] teFunctionTypes = new string[18] {"Add","Subtract","Multiply","Min","Max","Blend","Clamp","Power","Curve","Terrace","Abs","Exponent","Invert","ScaleBias","Turbulence","Select","Warp","WindexWarp"};
        public static string[] teNoiseChannels = new string[20] {"Channel 0","Channel 1","Channel 2","Channel 3","Channel 4","Channel 5","Channel 6","Channel 7","Channel 8","Channel 9","Channel 10","Channel 11","Channel 12","Channel 13","Channel 14","Channel 15","Channel 16","Channel 17","Channel 18","Channel 19"};
        public static string[] teNoiseChannelTypes = new string[3] { "(none)", "Generator", "Function" };
        public static int[] teFunctionTypeIndex = new int[20];
        public static int[] teNoiseTypeIndex = new int[20];
        public static int[] srcChannel1Id = new int[20];
        public static int[] srcChannel2Id = new int[20];
        public static int[] srcChannel3Id = new int[20];

        public static float[] exponent = new float[20];
        public static float[] offset = new float[20];
        public static float[] gain = new float[20];
        public static float[] scale = new float[20];
        public static float[] bias = new float[20];
        public static float[] power = new float[20];
        public static float[] falloff = new float[20];

	    public static float[,] cpval = new float[20,10];
	    public static int[] controlpointcount = new int[20] {4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4};
	    public static bool[] invertTerrace = new bool[20];

        int tempInt = 0;
        double tempDouble = 0;
        float tempFloat2 = 0;
        float tempFloat = 0;
        bool tempBool = false;

        public TerrainTab(string description) : base(description) {
            Generate2DNoise(teNoiseChanIndex);
        }

        public override void ShowTabContent()
        {
            EditorGUILayout.BeginVertical(); {

                Vearth.BeginVearthBox("Noise Settings"); {
                        
                    EditorGUILayout.BeginHorizontal(); {

                        EditorGUILayout.BeginVertical(); {
                            
                            EditorGUI.BeginChangeCheck ();
                            EditorGUILayout.BeginHorizontal(); {
                                tempInt = teNoiseChanIndex;
                                teNoiseChanIndex = EditorGUILayout.Popup(teNoiseChanIndex, teNoiseChannels);

                                GUILayout.Space(6f);

                                tempInt = teNoiseChanTypeIndex[teNoiseChanIndex];
                                teNoiseChanTypeIndex[teNoiseChanIndex] = EditorGUILayout.Popup(teNoiseChanTypeIndex[teNoiseChanIndex], teNoiseChannelTypes);
                            } EditorGUILayout.EndHorizontal();

                            EditorGUILayout.Space();

                            #region Generatorsss
                            if (teNoiseChanTypeIndex[teNoiseChanIndex] == 1) {
                                GUILayout.Label("Generator Settings", EditorStyles.miniBoldLabel);
                                
                                SelectedNoise[teNoiseChanIndex] = EditorGUILayout.Popup("Type / Algorithm", 
                                    SelectedNoise[teNoiseChanIndex], NoiseTypes);

                                    GUILayout.Space(3);
                                Seed[teNoiseChanIndex] = EditorGUILayout.IntSlider("Seed", Seed[teNoiseChanIndex], 0, 65535);

                                    GUILayout.Space(3);
                                Frequency[teNoiseChanIndex] = EditorGUILayout.Slider("Frequency", 
                                    Frequency[teNoiseChanIndex], 0.01f, 4f);

                                    GUILayout.Space(3);
                                Lacunarity[teNoiseChanIndex] = EditorGUILayout.Slider("Lacunarity", 
                                    Lacunarity[teNoiseChanIndex], 0.1f, 4f);

                                    GUILayout.Space(3);

                                if (!NoiseTypes[SelectedNoise[teNoiseChanIndex]].Equals("Ridged Multifractal Noise")) {
                                    Persistance[teNoiseChanIndex] = EditorGUILayout.Slider("Persistance", 
                                        Persistance[teNoiseChanIndex], 0.1f, 1.0f);
                                }

                                    GUILayout.Space(3);
                                Octaves[teNoiseChanIndex] = EditorGUILayout.IntSlider("Octaves", 
                                    Octaves[teNoiseChanIndex], 1, 10);
                                
                            }
                            #endregion

                            #region Functionsss
                            if (teNoiseChanTypeIndex[teNoiseChanIndex] == 2)
                            {
                                GUILayout.BeginVertical();
                                GUILayout.Label("Function Settings", EditorStyles.miniBoldLabel);
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Type", GUILayout.Width(80));
                                tempInt = teFunctionTypeIndex[teNoiseChanIndex];
                                teFunctionTypeIndex[teNoiseChanIndex] = EditorGUILayout.Popup(teFunctionTypeIndex[teNoiseChanIndex], teFunctionTypes);
                                GUILayout.EndHorizontal();
                                GUILayout.BeginHorizontal();
                                GUILayout.Label("Source A", GUILayout.Width(80));
                                tempInt = srcChannel1Id[teNoiseChanIndex];
                                srcChannel1Id[teNoiseChanIndex] = EditorGUILayout.Popup(srcChannel1Id[teNoiseChanIndex], teNoiseChannels);
                                GUILayout.EndHorizontal();
                                if (teFunctionTypeIndex[teNoiseChanIndex] < 6)
                                {
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("Source B", GUILayout.Width(80));
                                    tempInt = srcChannel2Id[teNoiseChanIndex];
                                    srcChannel2Id[teNoiseChanIndex] = EditorGUILayout.Popup(srcChannel2Id[teNoiseChanIndex], teNoiseChannels);
                                    GUILayout.EndHorizontal();
                                }
                                
                                if (teFunctionTypeIndex[teNoiseChanIndex] == 5||teFunctionTypeIndex[teNoiseChanIndex] == 15)
                                {
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label("Controller", GUILayout.Width(80));
                                    tempInt = srcChannel3Id[teNoiseChanIndex];
                                    srcChannel3Id[teNoiseChanIndex] = EditorGUILayout.Popup(srcChannel3Id[teNoiseChanIndex], teNoiseChannels);
                                    GUILayout.EndHorizontal();
                                }
                                if(teFunctionTypeIndex[teNoiseChanIndex]==11){
                                    tempDouble = exponent[teNoiseChanIndex];
                                    exponent[teNoiseChanIndex] = EditorGUILayout.Slider("Exponent:",exponent[teNoiseChanIndex],0.0f,1.0f);
                               }
                                if(teFunctionTypeIndex[teNoiseChanIndex]==13){ // ScaleBias
                                    tempDouble = scale[teNoiseChanIndex];
                                    scale[teNoiseChanIndex] = EditorGUILayout.Slider("Scale:",scale[teNoiseChanIndex],0.0f,1.0f);
                                    tempDouble = bias[teNoiseChanIndex];
                                }
                                if(teFunctionTypeIndex[teNoiseChanIndex]==14){
                                    tempDouble = power[teNoiseChanIndex];
                                    power[teNoiseChanIndex] = EditorGUILayout.Slider("Power:",power[teNoiseChanIndex],0.0f,1.0f);
                                }
                                if(teFunctionTypeIndex[teNoiseChanIndex]==15){
                                    tempDouble = falloff[teNoiseChanIndex];
                                    falloff[teNoiseChanIndex] = EditorGUILayout.Slider("Falloff:",falloff[teNoiseChanIndex],0.0f,1.0f);
                                }
                                if(teFunctionTypeIndex[teNoiseChanIndex]==6||teFunctionTypeIndex[teNoiseChanIndex] == 15)
                                {
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label ("Limits", GUILayout.Width(80));
                                    GUILayout.Label (noiseFuncMin[teNoiseChanIndex].ToString("N2"), GUILayout.Width(40));
                                    tempFloat = noiseFuncMin[teNoiseChanIndex];
                                    tempFloat2 = noiseFuncMax[teNoiseChanIndex];
                                    EditorGUILayout.MinMaxSlider(ref noiseFuncMin[teNoiseChanIndex],ref noiseFuncMax[teNoiseChanIndex],0.0f,1.0f,GUILayout.MinWidth(40));
                                    GUILayout.Label (noiseFuncMax[teNoiseChanIndex].ToString("N2"), GUILayout.Width(40));
                                    GUILayout.EndHorizontal();
                                }
                                if(teFunctionTypeIndex[teNoiseChanIndex]>7&&teFunctionTypeIndex[teNoiseChanIndex]<10)
                                {
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Label ("Control Points", GUILayout.Width(80));
                                    tempInt = controlpointcount[teNoiseChanIndex];
                                    controlpointcount[teNoiseChanIndex] = (int)GUILayout.HorizontalSlider(controlpointcount[teNoiseChanIndex],4.0f,10.0f);
                                    GUILayout.Label (controlpointcount[teNoiseChanIndex].ToString("N0"));
                                    GUILayout.EndHorizontal();
                                    GUILayout.BeginHorizontal(GUILayout.MaxHeight(80));
                                        GUILayout.Label ("\nCurve\nControl\nPoints", GUILayout.Width(80),GUILayout.Height(70));
                                        for(int i=0;i<controlpointcount[teNoiseChanIndex];i++){
                                            GUILayout.BeginVertical(GUILayout.Width(16));
                                            tempFloat = cpval[teNoiseChanIndex,i];
                                            cpval[teNoiseChanIndex,i] = GUILayout.VerticalSlider(cpval[teNoiseChanIndex,i],-1.0f,1.0f);
                                            GUI.enabled = true;
                                            GUILayout.EndVertical();
                                        }
                                    GUILayout.EndHorizontal();
                                    if(teFunctionTypeIndex[teNoiseChanIndex]==9)
                                    {
                                        tempBool = invertTerrace[teNoiseChanIndex];
                                        invertTerrace[teNoiseChanIndex] = GUILayout.Toggle(invertTerrace[teNoiseChanIndex],"Invert Terraces");	
                                   }
                                    EditorGUILayout.Space();
                                }
                                GUILayout.EndVertical();
                            }
                            #endregion

                            // check if changed
                            if (EditorGUI.EndChangeCheck ()) {
                                Generate2DNoise(teNoiseChanIndex);
                            }

                            if (teNoiseChanTypeIndex[teNoiseChanIndex] != 0) {
                                GUILayout.Space(3);
                                Heights = EditorGUILayout.Slider("Heights", Heights, 0.1f, 1.0f);
                            
                                GUILayout.Space(3);
                                Alpha = EditorGUILayout.Slider("Alpha", Alpha, 0.1f, 1.0f);
                            }

                        } EditorGUILayout.EndVertical();
                        
                        GUILayout.Space(13f);

                        
                        if (teNoiseChanTypeIndex[teNoiseChanIndex] != 0) {
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
                                                                    moduleBase[teNoiseChanIndex], Heights, Alpha);
                                        EditorUtility.ClearProgressBar();
                                    } else {
                                        EditorUtility.DisplayDialog("Vearth Error", "Please select a Terrain GameObject first!", "OK", "CANCEL");
                                    }
                                }
                            } EditorGUILayout.EndVertical(); 
                        }

                 

                    } EditorGUILayout.EndHorizontal();

                } Vearth.EndVearthBox();
                
                EditorGUILayout.Space();

                Vearth.BeginVearthBox("SplatMap Texture Settings"); {

                    EditorGUILayout.BeginVertical(); {
                        
                        if (Vearth.SelectedTerrain != null) {
                            SplatPrototype[] splatdata = Vearth.SelectedTerrain.GetComponent<Terrain>().terrainData.splatPrototypes;
                            SplatTextureNames = new string[splatdata.Length];

                            if (splatdata.Length > 0) {
                
                                for (int i = 0; i < splatdata.Length; i++)
                                {
                                    SplatTextureNames[i] = splatdata[i].texture.name;
                                }

                                ElevationShow = EditorGUILayout.Foldout(ElevationShow, "Elevations per Scale: ");    
                                if (ElevationShow) {
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
                                }


                                EditorGUILayout.Space();
                                
                                SlopeShow = EditorGUILayout.Foldout(SlopeShow, "Slope per Scale: ");
                                if (SlopeShow) {
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
                                }

                                EditorGUILayout.Space();
                                
                                UseFlowTexture = EditorGUILayout.ToggleLeft("Flow-Map Texturing", UseFlowTexture); 
                                
                                if (UseFlowTexture) {
                                    
                                    EditorGUILayout.Space();
                                    FlowTextureID = EditorGUILayout.Popup("Flow Texture", FlowTextureID, SplatTextureNames); 
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
                                } 

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
                } Vearth.EndVearthBox();
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

        private void Generate2DNoise (int channelId) {
            moduleBase[channelId] = new Perlin();
            
            if (teNoiseChanTypeIndex[channelId] == 1) {
                switch (SelectedNoise[channelId]) {
                    case 0:
                        moduleBase[channelId] = new Perlin(Frequency[channelId], Lacunarity[channelId], Persistance[channelId], 
                                        Octaves[channelId], Seed[channelId], QualityMode.High);
                        break;
                    case 1:
                        moduleBase[channelId] = new Billow(Frequency[channelId], Lacunarity[channelId], Persistance[channelId], 
                                        Octaves[channelId], Seed[channelId], QualityMode.High);
                        break;
                    case 2:
                        moduleBase[channelId] = new RidgedMultifractal(Frequency[channelId], Lacunarity[channelId], 
                                        Octaves[channelId], Seed[channelId], QualityMode.High);
                        break;
                }
            }

            if (teNoiseChanTypeIndex[channelId] == 2)
            {
                int fIdx = teFunctionTypeIndex[channelId];
                if (fIdx == 0) { moduleBase[channelId] = new Add(moduleBase[srcChannel1Id[channelId]], moduleBase[srcChannel2Id[channelId]]); }
                if (fIdx == 1) { moduleBase[channelId] = new Subtract(moduleBase[srcChannel1Id[channelId]], moduleBase[srcChannel2Id[channelId]]); }
                if (fIdx == 2) { moduleBase[channelId] = new Multiply(moduleBase[srcChannel1Id[channelId]], moduleBase[srcChannel2Id[channelId]]); }
                if (fIdx == 3) { moduleBase[channelId] = new Min(moduleBase[srcChannel1Id[channelId]], moduleBase[srcChannel2Id[channelId]]); }
                if (fIdx == 4) { moduleBase[channelId] = new Max(moduleBase[srcChannel1Id[channelId]], moduleBase[srcChannel2Id[channelId]]); }
                if (fIdx == 5) { moduleBase[channelId] = new Blend(moduleBase[srcChannel1Id[channelId]], moduleBase[srcChannel2Id[channelId]], moduleBase[srcChannel3Id[channelId]]); }
                if (fIdx == 6) { moduleBase[channelId] = new Clamp((double)noiseFuncMin[channelId], (double)noiseFuncMax[channelId], moduleBase[srcChannel1Id[channelId]]); }
                if (fIdx == 7) { moduleBase[channelId] = new Power(moduleBase[srcChannel1Id[channelId]],moduleBase[srcChannel2Id[channelId]]);}
                if (fIdx == 8) { Curve tmpCurve = new Curve(moduleBase[srcChannel1Id[channelId]]);
                    double adjust = double.Parse((controlpointcount[channelId]-1).ToString())*0.5;
                    for(int i=0;i<controlpointcount[channelId];i++){
                        tmpCurve.Add(double.Parse(i.ToString())-adjust,(double)cpval[channelId,i]);
                        moduleBase[channelId] = tmpCurve;
                    }
                }
                if(fIdx==9){Terrace tmpTerrace = new Terrace(invertTerrace[channelId],moduleBase[srcChannel1Id[channelId]]);
                    for(int i=0;i<controlpointcount[channelId];i++){
                        tmpTerrace.Add((double)cpval[channelId,i]-0.5);
                        moduleBase[channelId] = tmpTerrace;
                    }
                }
                if (fIdx == 17) { moduleBase[channelId] = new WindexWarp(moduleBase[srcChannel1Id[channelId]]); }
                if (fIdx == 16) { moduleBase[channelId] = new TEWarp(moduleBase[srcChannel1Id[channelId]]); }
                if (fIdx == 15) { moduleBase[channelId] = new Select((double)noiseFuncMin[channelId], (double)noiseFuncMax[channelId], falloff[channelId],moduleBase[srcChannel1Id[channelId]],moduleBase[srcChannel3Id[channelId]]); }
                if (fIdx == 14) { moduleBase[channelId] = new Turbulence(power[channelId],moduleBase[srcChannel1Id[channelId]]); }
                if (fIdx == 13) { moduleBase[channelId] = new ScaleBias(scale[channelId],bias[channelId],moduleBase[srcChannel1Id[channelId]]); }
                if (fIdx == 12) { moduleBase[channelId] = new Invert(moduleBase[srcChannel1Id[channelId]]);}
                if (fIdx == 11) { moduleBase[channelId] = new Exponent(exponent[channelId],moduleBase[srcChannel1Id[channelId]]); }
                if (fIdx == 10) { moduleBase[channelId] = new Abs(moduleBase[srcChannel1Id[channelId]]);}
            }

            int xoffset = 0; int yoffset = 0;
            float zoom = 1f;
            previewNoise = new Noise2D(resolution, resolution, moduleBase[channelId]);
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
#endif