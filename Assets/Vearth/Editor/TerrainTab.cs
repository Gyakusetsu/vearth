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
            
        int resolution = 155;

        public TerrainTab(string description) : base(description) {
            Generate2DNoise(SelectedNoise);
        }

        public override void ShowTabContent()
        {
            EditorGUI.BeginChangeCheck ();

            EditorGUILayout.LabelField("Noise Settings", EditorStyles.toolbarButton);

            EditorGUILayout.BeginVertical(EditorStyles.textField); {
                    
                GUIStyle SectionStyle = new GUIStyle();
                SectionStyle.padding = new RectOffset(13, 13, 13, 13);

                EditorGUILayout.BeginVertical(SectionStyle); {
                    
                    EditorGUILayout.BeginHorizontal(); {

                        EditorGUILayout.BeginVertical(); {

                            EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
                            EditorGUILayout.Space();

                            SelectedNoise = EditorGUILayout.Popup("Type / Algorithm", SelectedNoise, NoiseTypes);

                            Seed = EditorGUILayout.IntSlider("Seed", Seed, 0, 65535);

                            Frequency = EditorGUILayout.Slider("Frequency", Frequency, 0.01f, 4f);

                            Lacunarity = EditorGUILayout.Slider("Lacunarity", Lacunarity, 0.1f, 4f);

                            if (!NoiseTypes[SelectedNoise].Equals("Ridged Multifractal Noise")) {
                                EditorGUILayout.BeginHorizontal();
                                Persistance = EditorGUILayout.Slider("Persistance", Persistance, 0.1f, 1.0f);
                                EditorGUILayout.EndHorizontal();
                            }

                            Octaves = EditorGUILayout.IntSlider("Octaves", Octaves, 1, 10);

                            // check if changed
                            if (EditorGUI.EndChangeCheck ()) {
                                Generate2DNoise(SelectedNoise);
                            }
                            
                            Heights = EditorGUILayout.Slider("Heights", Heights, 0.1f, 1.0f);
                            
                            Alpha = EditorGUILayout.Slider("Alpha", Alpha, 0.1f, 1.0f);
                        } EditorGUILayout.EndVertical();
                        
                        GUILayout.Space(13f);
                        GUILayout.Box(previewTexture);
                    } EditorGUILayout.EndHorizontal();
                } EditorGUILayout.EndVertical();
            } EditorGUILayout.EndVertical();
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