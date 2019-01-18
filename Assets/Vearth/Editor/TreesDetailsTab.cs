#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Vearth {
    public class TreesDetailsTab : VearthTab
    {
        private static List<TreeInstance> TreeInstances;
        public static int[] teTreeRuleProtoId = new int[10];
        public static float[,] teTreeRuleParams = new float[10, 8];
        public static string[] teTreeRules = new string[10] {"Tree Rule 1","Tree Rule 2","Tree Rule 3","Tree Rule 4","Tree Rule 5","Tree Rule 6","Tree Rule 7","Tree Rule 8","Tree Rule 9","Tree Rule 10"};
        public static int teTreeRuleIndex = 0;
        public static Color[] teTreeColor = new Color[10] {Color.white,Color.white,Color.white,Color.white,Color.white,Color.white,Color.white,Color.white,Color.white,Color.white};
        public static Color[] teTreeLightmapColor = new Color[10] {Color.white,Color.white,Color.white,Color.white,Color.white,Color.white,Color.white,Color.white,Color.white,Color.white};
        public static float[] teTreeHeight = new float[10] {1f,1f,1f,1f,1f,1f,1f,1f,1f,1f};
        public static float[] teTreeWidth = new float[10] {1f,1f,1f,1f,1f,1f,1f,1f,1f,1f};	
        public static float[] teTreeHeightVariation = new float[10];
        public static float[] teTreeWidthVariation = new float[10];
        public static string[,,] tfRuleParams = new string[10,10,12];
   	    public static string[] tfRules = new string[10] {"Rule 0","Rule 1","Rule 2","Rule 3","Rule 4","Rule 5","Rule 6","Rule 7","Rule 8","Rule 9"};
        public static int tfRuleIndex = 0;
        public static string[] tfRuleTypes = new string[4] {"None","Detail","Splat","Tree"};


        public TreesDetailsTab(string description) : base(description) {
            for (int i = 0; i < 10; i++){
                teTreeRuleParams[i, 1] = 1.0f;
                teTreeRuleParams[i, 3] = 90.0f;
                teTreeRuleParams[i, 6] = 0.5f;
                for ( int ii = 0; ii < 10; ii++ ) {
                    tfRuleParams[i,ii,2] = "0.0";
                    tfRuleParams[i,ii,3] = "0.0";
                    tfRuleParams[i,ii,4] = Color.white.ToString();
                    tfRuleParams[i,ii,5] = Color.white.ToString();
                    tfRuleParams[i,ii,6] = "0.0";
                    tfRuleParams[i,ii,7] = "0.0";
                    tfRuleParams[i,ii,8] = "0.0";
                    tfRuleParams[i,ii,9] = "0.0";
                    tfRuleParams[i,ii,10] = "0.0";
                }
            }
        }

        
		public static string[] GetPrototypeList(string prototypeType, TerrainData terdat){
			string[] protoNames = new string[16];
				switch(prototypeType){
				case "1":
					DetailPrototype[] protos = terdat.detailPrototypes;
					if (protos.Length > 0)
			        {
			            for (int i = 0; i < protos.Length; i++)
			            {
			                if (protos[i].prototypeTexture)
			                {
			                    protoNames[i] = protos[i].prototypeTexture.name;
			                }
			                else
			                {
			                    protoNames[i] = protos[i].prototype.name;
			                }
			            }
			        }
					break;
				case "2":
					SplatPrototype[] sprotos = terdat.splatPrototypes;
					if (sprotos.Length > 0)
			        {
			            for (int i = 0; i < sprotos.Length; i++)
			            {
			               protoNames[i] = sprotos[i].texture.name;
			            }
			        }
					break;
				case "3":
					TreePrototype[] tprotos = terdat.treePrototypes;
					if (tprotos.Length > 0)
			        {
			            for (int i = 0; i < tprotos.Length; i++)
			            {
			                protoNames[i] = tprotos[i].prefab.name;
			            }
			        }
					break;
				}
			return protoNames;
		}		


		public static Color ColorParse(string colorText){
			if(colorText.IndexOf("RGBA")<0){return Color.white;}
			string colorTextStripped = colorText.Replace("RGBA(","").Replace(")","").Replace(" ","");
			string[] colorTextSegs = colorTextStripped.Split(",".ToCharArray());
			return new Color(float.Parse(colorTextSegs[0]),float.Parse(colorTextSegs[1]),float.Parse(colorTextSegs[2]),float.Parse(colorTextSegs[3]));
		}

        public override void ShowTabContent()
        { 
            Vearth.BeginVearthBox("Tree Rules / Details"); {
            if (Vearth.SelectedTerrain != null)
            {
                GUILayout.BeginHorizontal();
                int prevTreeRuleIndex = teTreeRuleIndex;
                teTreeRuleIndex = EditorGUILayout.Popup(teTreeRuleIndex,teTreeRules);
                if(prevTreeRuleIndex!=teTreeRuleIndex){
                    tfRuleIndex = 0;	
                }
                TreePrototype[] treedata = Vearth.SelectedTerrain.GetComponent<Terrain>().terrainData.treePrototypes;
                string[] teTreeOptions = new string[treedata.Length+1];
                teTreeOptions[0]="(none)";
                if(treedata.Length>0){
                    for(int i=0;i<treedata.Length;i++){
                        teTreeOptions[i+1] = treedata[i].prefab.name;
                    }
                }   
                GUILayout.Space(6);
                teTreeRuleProtoId[teTreeRuleIndex] = EditorGUILayout.Popup(teTreeRuleProtoId[teTreeRuleIndex],teTreeOptions);
                GUILayout.Space(3);
                GUILayout.EndHorizontal();
                if(teTreeRuleProtoId[teTreeRuleIndex]>0){
                    GUILayout.BeginHorizontal();
                        GUILayout.Label ("Height", GUILayout.Width(140));
                        GUILayout.Label (teTreeRuleParams[teTreeRuleIndex,0].ToString("N2"), GUILayout.Width(40));
                        EditorGUILayout.MinMaxSlider(ref teTreeRuleParams[teTreeRuleIndex,0],ref teTreeRuleParams[teTreeRuleIndex,1],0.0f,1.0f,GUILayout.MinWidth(40));
                        GUILayout.Label (teTreeRuleParams[teTreeRuleIndex,1].ToString("N2"), GUILayout.Width(40));
                    GUILayout.EndHorizontal();
                    GUILayout.Space(3);
                    GUILayout.BeginHorizontal();
                        GUILayout.Label ("Slope", GUILayout.Width(140));
                        GUILayout.Label (teTreeRuleParams[teTreeRuleIndex,2].ToString("N2"), GUILayout.Width(40));
                        EditorGUILayout.MinMaxSlider(ref teTreeRuleParams[teTreeRuleIndex,2],ref teTreeRuleParams[teTreeRuleIndex,3],0.0f,90.0f,GUILayout.MinWidth(40));
                        GUILayout.Label (teTreeRuleParams[teTreeRuleIndex,3].ToString("N2"), GUILayout.Width(40));
                    GUILayout.EndHorizontal();
                    GUILayout.Space(3);
                    teTreeRuleParams[teTreeRuleIndex,6] = EditorGUILayout.Slider("Trees", teTreeRuleParams[teTreeRuleIndex,6], 1.0f, 2000f);
                    GUILayout.Space(3);
                    teTreeColor[teTreeRuleIndex] = EditorGUILayout.ColorField("Color", teTreeColor[teTreeRuleIndex]);
                    GUILayout.Space(3);
                    teTreeLightmapColor[teTreeRuleIndex] = EditorGUILayout.ColorField("Lightmap",teTreeLightmapColor[teTreeRuleIndex]);
                    GUILayout.Space(3);
                    teTreeWidth[teTreeRuleIndex] = EditorGUILayout.Slider("Width", teTreeWidth[teTreeRuleIndex], 0.25f, 4.0f);
                    GUILayout.Space(3);
                    teTreeWidthVariation[teTreeRuleIndex] = EditorGUILayout.Slider("Width Variation", teTreeWidthVariation[teTreeRuleIndex], 0.0f, 0.9f);
                    GUILayout.Space(3);
                    teTreeHeight[teTreeRuleIndex] = EditorGUILayout.Slider("Height", teTreeHeight[teTreeRuleIndex], 0.25f, 4.0f);
                    GUILayout.Space(3);
                    teTreeHeightVariation[teTreeRuleIndex] = EditorGUILayout.Slider("Height Variation", teTreeHeightVariation[teTreeRuleIndex], 0.0f, 0.9f);
                    GUILayout.Space(3);
            
                    GUI.backgroundColor = new Color(0.9f,0.9f,1f,1f);
                    GUILayout.BeginVertical(EditorStyles.textField);
                    GUILayout.Space(3);
                    GUILayout.Label("Tree Surround Foliage");
                    GUILayout.BeginHorizontal();
                    tfRuleIndex = EditorGUILayout.Popup(tfRuleIndex,tfRules);
                    GUILayout.Space(6);
                    tfRuleParams[teTreeRuleIndex,tfRuleIndex,0] = EditorGUILayout.Popup(int.Parse("0"+tfRuleParams[teTreeRuleIndex,tfRuleIndex,0]),tfRuleTypes).ToString();
                    GUILayout.EndHorizontal();
                    GUILayout.Space(3);
                    
                    if(int.Parse("0"+tfRuleParams[teTreeRuleIndex,tfRuleIndex,0])>0){
                        string[] tfProtos = GetPrototypeList(tfRuleParams[teTreeRuleIndex,tfRuleIndex,0],Vearth.SelectedTerrain.GetComponent<Terrain>().terrainData);		
                        if(tfProtos.Length==0){
                            tfProtos = new string[1]{"Terrain has no "+tfRuleTypes[int.Parse(tfRuleParams[teTreeRuleIndex,tfRuleIndex,0])]+" prototypes"};
                            GUI.enabled=false;
                            tfRuleParams[teTreeRuleIndex,tfRuleIndex,1] = EditorGUILayout.Popup(int.Parse(tfRuleParams[teTreeRuleIndex,tfRuleIndex,1]),tfProtos).ToString();
                            GUI.enabled=true;
                        } else {
                            tfRuleParams[teTreeRuleIndex,tfRuleIndex,1] = EditorGUILayout.Popup(int.Parse(tfRuleParams[teTreeRuleIndex,tfRuleIndex,1]),tfProtos).ToString();
                            string folTypeId = tfRuleParams[teTreeRuleIndex,tfRuleIndex,0];	
                            if(folTypeId!="3"){
                                tfRuleParams[teTreeRuleIndex,tfRuleIndex,2] = EditorGUILayout.Slider("Strength",float.Parse(tfRuleParams[teTreeRuleIndex,tfRuleIndex,2]),0f,1f).ToString();	
                                GUILayout.Space(3);
                            }
                            tfRuleParams[teTreeRuleIndex,tfRuleIndex,3] = EditorGUILayout.Slider("Density",float.Parse(tfRuleParams[teTreeRuleIndex,tfRuleIndex,3]),0f,1f).ToString();		
                            GUILayout.Space(3);		
                            tfRuleParams[teTreeRuleIndex,tfRuleIndex,10] = EditorGUILayout.Slider("Radius",float.Parse(tfRuleParams[teTreeRuleIndex,tfRuleIndex,10]),0f,10f).ToString();
                            GUILayout.Space(3);
                            if(folTypeId=="3"){
                                tfRuleParams[teTreeRuleIndex,tfRuleIndex,4] = EditorGUILayout.ColorField("Color", ColorParse(tfRuleParams[teTreeRuleIndex,tfRuleIndex,4])).ToString();
                                GUILayout.Space(3);
                                tfRuleParams[teTreeRuleIndex,tfRuleIndex,5] = EditorGUILayout.ColorField("Light",ColorParse(tfRuleParams[teTreeRuleIndex,tfRuleIndex,5])).ToString();
                                GUILayout.Space(3);
                                tfRuleParams[teTreeRuleIndex,tfRuleIndex,6] = EditorGUILayout.Slider("Width",float.Parse(tfRuleParams[teTreeRuleIndex,tfRuleIndex,6]),1f,4f).ToString();
                                GUILayout.Space(3);
                                tfRuleParams[teTreeRuleIndex,tfRuleIndex,7] = EditorGUILayout.Slider("Width Variation",float.Parse(tfRuleParams[teTreeRuleIndex,tfRuleIndex,7]),0.0f,0.9f).ToString();
                                GUILayout.Space(3);
                                tfRuleParams[teTreeRuleIndex,tfRuleIndex,8] = EditorGUILayout.Slider("Height",float.Parse(tfRuleParams[teTreeRuleIndex,tfRuleIndex,8]),1f,4f).ToString();
                                GUILayout.Space(3);
                                tfRuleParams[teTreeRuleIndex,tfRuleIndex,9] = EditorGUILayout.Slider("Height Variation",float.Parse(tfRuleParams[teTreeRuleIndex,tfRuleIndex,9]),0.0f,0.9f).ToString();
                                GUILayout.Space(3);		
                            }	
                        }
                    } else {
                        string[] tfProtos = new string[1]{"No type selected"};
                        GUI.enabled=false;
                        tfRuleParams[teTreeRuleIndex,tfRuleIndex,1] = EditorGUILayout.Popup(int.Parse("0"+tfRuleParams[teTreeRuleIndex,tfRuleIndex,1]),tfProtos).ToString();
                        GUI.enabled=true;
                    }
                    GUILayout.EndVertical();
                    GUI.backgroundColor = new Color(1f,1f,1f,1f);			
                }
                
				EditorGUILayout.Space();
                GUILayout.BeginHorizontal();
                if(GUILayout.Button("Apply Tree Details", GUILayout.MinHeight(30))){
                    Undo.RegisterCompleteObjectUndo(Vearth.SelectedTerrain.GetComponent<Terrain>().terrainData,"te:Generate Trees");
                    ApplyTreeDetails(Vearth.SelectedTerrain.GetComponent<Terrain>());
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.Label("Select a terrain object.", EditorStyles.miniBoldLabel);
            }
            } Vearth.EndVearthBox();
        }

    public static void ApplyTreeDetails(Terrain terrain){
		TerrainData terdata = terrain.terrainData;
		TreeInstance[] newtrees = new TreeInstance[0];
		int res = terdata.heightmapResolution;
		float tmpHeight = 0.0f;
		float tmpSlope = 0.0f;
		bool[,] posTaken = new bool[res+1,res+1];
		terdata.treeInstances = newtrees;
		for(int ruleId=0;ruleId<10;ruleId++){
			if(teTreeRuleProtoId[ruleId]>0){
				int treecount = 0;
				int attempts = 0;
				while(treecount<(int)teTreeRuleParams[ruleId,6]&&attempts<2000){
					EditorUtility.DisplayProgressBar("Generate Trees","Terrain GameObject:"+terrain.name+"\nTree Rule: "+ruleId.ToString()+"\nGenerating: "+treecount+" of "+(int)teTreeRuleParams[ruleId,6]+"...",(float)treecount*(1f/(float)teTreeRuleParams[ruleId,6]));
				
					float strengthmult = 1f;
					int x=(int)((float)res*UnityEngine.Random.value);
					int y=(int)((float)res*UnityEngine.Random.value);
					tmpHeight = (terdata.GetHeight(y,x)/(terdata.size.y));		
					tmpSlope = terdata.GetSteepness(((1.0f/res)*y)+(0.2f/res),((1.0f/res)*x)+(0.2f/res));
					if(tmpHeight<teTreeRuleParams[ruleId,0]){strengthmult = 0f;}
					if(tmpHeight>teTreeRuleParams[ruleId,1]){strengthmult = 0f;}
					if(tmpSlope<teTreeRuleParams[ruleId,2]){strengthmult = 0f;}
					if(tmpSlope>teTreeRuleParams[ruleId,3]){strengthmult = 0f;}		
					if(posTaken[x,y]!=true&&strengthmult>0f){
						treecount++;
						attempts = 0;
						posTaken[x,y]=true;
						TreeInstance newTree = new TreeInstance();
						newTree.prototypeIndex = teTreeRuleProtoId[ruleId]-1;
						float treeX = (float)x / (float)res;
						float treeY = (float)y / (float)res;
						newTree.position = new Vector3(treeY,0,treeX);
						newTree.widthScale = teTreeWidth[ruleId] + (teTreeWidthVariation[ruleId]*(UnityEngine.Random.value-0.75f));
						newTree.heightScale = teTreeHeight[ruleId] + (teTreeHeightVariation[ruleId]*(UnityEngine.Random.value-0.75f));
						newTree.color = teTreeColor[ruleId];
						newTree.lightmapColor = teTreeLightmapColor[ruleId];
						terrain.AddTreeInstance(newTree);
						TreeSurroundFoliage(ruleId,treeX,treeY);
					}
					attempts++;
				}
				EditorUtility.ClearProgressBar();
			}
		}
		terrain.Flush();
	}
    
    public static float FallOffMult(int calcX,int centerX,int calcZ,int centerZ,int maxVal){
        int offX = calcX - centerX;
        int offZ = calcZ - centerZ;
        if(offX<0){offX=0-offX;}
        if(offZ<0){offZ=0-offZ;}
        if(offX+offZ==0){return 1f;}
        return 1f/(float)(offX+offZ);
    }		

	public static Color colorParse(string colorText){
		if(colorText.IndexOf("RGBA")<0){return Color.white;}
		string colorTextStripped = colorText.Replace("RGBA(","").Replace(")","").Replace(" ","");
		string[] colorTextSegs = colorTextStripped.Split(",".ToCharArray());
		return new Color(float.Parse(colorTextSegs[0]),float.Parse(colorTextSegs[1]),float.Parse(colorTextSegs[2]),float.Parse(colorTextSegs[3]));
	}



	
	public static void TreeSurroundFoliage(int treeRuleId,float inY, float inX){
		GameObject go = Vearth.SelectedTerrain;
		TerrainData td = go.GetComponent<Terrain>().terrainData;
		int x = (int)(inX*td.size.x);
		int z = (int)(inY*td.size.z);
		int detailres = td.detailResolution;
		int alphares = td.alphamapResolution;
		float[,,] alphadata = td.GetAlphamaps(0,0,alphares,alphares);
		float detailMult = (1.0f / td.size.x)*(float)detailres;
		float alphaMult = (1.0f / td.size.x)*(float)alphares;
		int detX = (int)(detailMult*(float)x); //x:z coords for other map resolutions
		int detZ = (int)(detailMult*(float)z);
		int alphaX = (int)(alphaMult*(float)x);
		int alphaZ = (int)(alphaMult*(float)z); 
		int[,] detaildata = new int[detailres,detailres]; 

		for(int tfRuleId=0;tfRuleId<10;tfRuleId++){	
			int pasteregionoffset = 0;
			float strengthmult =1.0f;
			bool[,] posTaken = new bool[(int)td.size.x,(int)td.size.x];
			switch(tfRuleParams[treeRuleId,tfRuleId,0]){
			case "1":
					//Debug.Log("TF-Detail @"+x+":"+z);
					// Detail -------------------------------------------
					detaildata = td.GetDetailLayer(0,0,detailres,detailres,int.Parse(tfRuleParams[treeRuleId,tfRuleId,1]));
					pasteregionoffset = (int)float.Parse("0"+tfRuleParams[treeRuleId,tfRuleId,10]);
					for(int tZ=(detZ)-pasteregionoffset;tZ<(detZ)+(pasteregionoffset+1);tZ++){
						for(int tX=(detX)-pasteregionoffset;tX<(detX)+(pasteregionoffset+1);tX++){
							strengthmult = (float.Parse(tfRuleParams[treeRuleId,tfRuleId,2])*15f)*FallOffMult(tX,detX,tZ,detZ,pasteregionoffset);
							if(tZ>=0&&tZ<=detailres-1&&tX>=0&&tX<=detailres-1){
								if(0f<float.Parse(tfRuleParams[treeRuleId,tfRuleId,3])){;
									int tmpval = detaildata[tZ,tX]+(int)strengthmult;
									if(tmpval>15){tmpval=15;}
									detaildata[tZ,tX]=tmpval;
								}
							}
						}
					}
					td.SetDetailLayer(0,0,int.Parse(tfRuleParams[treeRuleId,tfRuleId,1]),detaildata);
					break;
			case "2":
					//Debug.Log("TF-Splat @"+x+":"+z);
					// Splat/Texture  -------------------------------------------
					pasteregionoffset = (int)float.Parse("0"+tfRuleParams[treeRuleId,tfRuleId,10]);
					for(int tZ=(alphaZ)-pasteregionoffset;tZ<(alphaZ)+(pasteregionoffset+1);tZ++){
						for(int tX=(alphaX)-pasteregionoffset;tX<(alphaX)+(pasteregionoffset+1);tX++){
							strengthmult = (int)float.Parse(tfRuleParams[treeRuleId,tfRuleId,3])*FallOffMult(tX,alphaX,tZ,alphaZ,pasteregionoffset);
							if(tZ>=0&&tZ<=alphares-1&&tX>=0&&tX<=alphares-1){
								if(UnityEngine.Random.value<float.Parse(tfRuleParams[treeRuleId,tfRuleId,2])){
									float addAmount = strengthmult;
									float remainder = 1.0f-addAmount;
									// Now we tally up the total value shared by other splat channels...
									float cumulativeAmount = 0.0f;
									for(int i2=0;i2<td.splatPrototypes.Length;i2++){
										if(i2!=int.Parse(tfRuleParams[treeRuleId,tfRuleId,1])){
											cumulativeAmount=cumulativeAmount+alphadata[tZ,tX,i2];
										}
									}
									if(cumulativeAmount>0.0f){
										float fixLayerMult = remainder / cumulativeAmount; // we multiple the other layer's splat values by this
										// Now we re-apply the splat values..
										for(int i2=0;i2<td.splatPrototypes.Length;i2++){
											if(i2!=int.Parse(tfRuleParams[treeRuleId,tfRuleId,1])){
												alphadata[tZ,tX,i2] = fixLayerMult*alphadata[tZ,tX,i2];
											} else {
												alphadata[tZ,tX,i2] = addAmount;
											}
										}
									} else {
										alphadata[tZ,tX,int.Parse(tfRuleParams[treeRuleId,tfRuleId,1])] = 1.0f;
									}
								}
							}
						}
					}
					break;
			case "3":
					// Trees -------------------------------------------------------
					strengthmult = float.Parse("0"+tfRuleParams[treeRuleId,tfRuleId,2]);
					pasteregionoffset = (int)float.Parse("0"+tfRuleParams[treeRuleId,tfRuleId,10]);
					for(int tZ=z-pasteregionoffset;tZ<z+(pasteregionoffset+1);tZ++){
						for(int tX=x-pasteregionoffset;tX<x+(pasteregionoffset+1);tX++){
							if(tZ>=0&&tZ<=td.size.z-1&&tX>=0&&tX<=td.size.x-1){
								
								if(posTaken[tX,tZ]!=true&&UnityEngine.Random.value<(0.01f*strengthmult*FallOffMult(tX,x,tZ,z,10))){
									posTaken[tX,tZ]=true;
									TreeInstance newTree = new TreeInstance();
									newTree.prototypeIndex = int.Parse(tfRuleParams[treeRuleId,tfRuleId,1]);
									float treeX = (float)tX/td.size.x;
									float treeZ = (float)tZ/td.size.z;
									newTree.position = new Vector3(treeX,0,treeZ);
									newTree.widthScale = float.Parse(tfRuleParams[treeRuleId,tfRuleId,6]) + (float.Parse(tfRuleParams[treeRuleId,tfRuleId,7])*(UnityEngine.Random.value-0.75f));
									newTree.heightScale = float.Parse(tfRuleParams[treeRuleId,tfRuleId,8]) + (float.Parse(tfRuleParams[treeRuleId,tfRuleId,9])*(UnityEngine.Random.value-0.75f));
									newTree.color = colorParse(tfRuleParams[treeRuleId,tfRuleId,4]);
									newTree.lightmapColor = colorParse(tfRuleParams[treeRuleId,tfRuleId,5]);
									go.GetComponent<Terrain>().AddTreeInstance(newTree);
								}
							}
						}
					}
					// Get rid of tree colliders..
					float[,] tmpheights = td.GetHeights(0, 0, 0, 0);
        			td.SetHeights(0, 0, tmpheights);
                    break;
                }
            td.SetAlphamaps(0,0,alphadata);			
            }	
        }
    }
}
#endif