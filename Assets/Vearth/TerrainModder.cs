#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using LibNoise;
using LibNoise.Generator;
using LibNoise.Operator;

namespace Vearth3D {
    public class TerrainModder {
        public static void ApplyHeightMap(Terrain terrain, ModuleBase noise, float amplification, float alpha) {
            Vector3 gopos = terrain.gameObject.transform.position;
	        float cwidth = terrain.terrainData.size.x;
	        int resolution = terrain.terrainData.heightmapResolution;
			float[,] hmap = new float[resolution,resolution];
	        double yoffset = 0 - (gopos.x / cwidth);
	        double xoffset = (gopos.z / cwidth);
	        Noise2D tmpNoiseMap = new Noise2D(resolution, resolution, noise);
	        tmpNoiseMap.GeneratePlanar(xoffset, (xoffset) + (1f / resolution) * (resolution + 1), -yoffset, (-yoffset) + (1f / resolution) * (resolution + 1));
	        if (alpha == 1.0f)
	        {
	            for (int hY = 0; hY < resolution; hY++)
	            {
	                for (int hX = 0; hX < resolution; hX++)
	                {
	                    hmap[hX, hY] = ((tmpNoiseMap[hX, hY]*0.5f) + 0.5f) * amplification;
	                }
	            }
	        }
	        else
	        {
	            hmap = terrain.terrainData.GetHeights(0, 0, resolution, resolution);
	            for (int hY = 0; hY < resolution; hY++)
	            {
	                for (int hX = 0; hX < resolution; hX++)
	                {
	                    hmap[hX, hY] = ((1.0f - alpha) * hmap[hX, hY]) 
                                + (alpha * (((tmpNoiseMap[hX, hY]*0.5f) + 0.5f) * amplification));
	                }
	            }
	        }
	        terrain.terrainData.SetHeights(0, 0, hmap);
        }
    
		public static void ApplyTextures(Terrain terrain,
										int[] SplatElevationTextureID, float[] SplatElevationHeight, 
										int[] SplatSlopeTextureID, float[] SplatSlopeSteepness, 
										bool UseFlowTexture, int FlowTextureID, float[] FlowTextureParams){

			TerrainData terdata = terrain.terrainData;
			float[,] heights = terdata.GetHeights(0,0,terdata.heightmapResolution,terdata.heightmapResolution);
			int alphares = terdata.alphamapResolution;
			float[,,] alphadata = terdata.GetAlphamaps(0,0,alphares,alphares);
			int splatSlot = 0;
			int splatSlotSteep = 0;
			float mult = 1.0f;
			float slopemult = 1.0f;
			float tmpSlope = 0.0f;
			for(int y=0;y<alphares;y++){
				for(int x=0;x<alphares;x++){
					float tmpHeight = (terdata.GetHeight(y,x)/terdata.size.y);
					tmpSlope = terdata.GetSteepness(((1.0f/alphares)*y)+(0.5f/alphares),((1.0f/alphares)*x)+(0.5f/alphares));
					splatSlotSteep = 0;
					if(tmpHeight< SplatElevationHeight[0]){
						splatSlot = 0;	
					}
					for(int i=0;i<11;i++){
						if(tmpHeight>=SplatElevationHeight[i]){
							splatSlot = i;	
						}
						if(i<4){
							if(tmpSlope>=SplatSlopeSteepness[i]){
								splatSlotSteep = i;	
							}
						}	
					}
					if(tmpHeight>SplatElevationHeight[10]){
						splatSlot = 10;	
					}
					if(tmpSlope>SplatSlopeSteepness[3]){
						splatSlotSteep = 3;	
					}
					for(int i=0;i<terdata.splatPrototypes.Length;i++){
						alphadata[x,y,i]=0.0f;
					}
					mult=1.0f / (SplatElevationHeight[splatSlot+1]-SplatElevationHeight[splatSlot]);
					slopemult=1.0f / (SplatSlopeSteepness[splatSlotSteep+1]-SplatSlopeSteepness[splatSlotSteep]);
					int texFrom = SplatElevationTextureID[splatSlot];
					int texTo = SplatElevationTextureID[splatSlot+1];
					int slopeTexFrom = SplatSlopeTextureID[splatSlotSteep];
					int slopeTexTo = SplatSlopeTextureID[splatSlotSteep+1];
					if(splatSlotSteep>0){
						slopemult=1.0f / (SplatSlopeSteepness[splatSlotSteep+1]-SplatSlopeSteepness[splatSlotSteep]);
						float texFromAmount = (slopemult*(tmpSlope-SplatSlopeSteepness[splatSlotSteep]));
						float texToAmount = 1.0f-texFromAmount;
						if(slopeTexFrom==slopeTexTo){texToAmount=1.0f;}
						alphadata[x,y,slopeTexTo] = texFromAmount;
						alphadata[x,y,slopeTexFrom] = texToAmount;
					} else {
						
						if(tmpSlope<SplatSlopeSteepness[0]){
							// skip slope splatting
							if(texFrom==texTo){
								alphadata[x,y,texFrom] = 1.0f;
							} else {
								alphadata[x,y,texFrom] = 1.0f-(mult*(tmpHeight-SplatElevationHeight[splatSlot]));
								alphadata[x,y,texTo] = mult*(tmpHeight-SplatElevationHeight[splatSlot]);
							}
						} else {
							int slopeTex = SplatSlopeTextureID[splatSlotSteep+1];
							float slopeTexAmount = slopemult*(tmpSlope-SplatSlopeSteepness[splatSlotSteep]);
							float texFromAmount = 1.0f-slopeTexAmount;
							float texToAmount = 1.0f-slopeTexAmount;
							if(texFrom!=texTo){
								texToAmount = mult*(tmpHeight-SplatElevationHeight[splatSlot]);
								texFromAmount = 1.0f-texToAmount;
								texToAmount = texToAmount * (1.0f-slopeTexAmount);
								texFromAmount = texFromAmount * (1.0f-slopeTexAmount);
								if(texTo==slopeTex&&texTo!=texFrom){
									texToAmount = slopeTexAmount+texToAmount;
									slopeTexAmount = texToAmount;
								}
								if(texFrom==slopeTex&&texTo!=texFrom){
									texFromAmount = slopeTexAmount+texFromAmount;
									slopeTexAmount = texFromAmount;
								}
								
							}
							if(texFrom==slopeTex&&texTo==texFrom){
								texFromAmount = 1.0f;
								slopeTexAmount = 1.0f;
								texToAmount = 1.0f;
							}
							if(terdata.splatPrototypes.Length>texFrom){
								alphadata[x,y,texFrom]=texFromAmount;
							} else {
								Debug.Log("Alpha/Splat layer "+texFrom+" on "+ terrain.gameObject.name+" was skipped because terrain does not have that many prototypes.");
							}
							if(terdata.splatPrototypes.Length>texFrom){
								alphadata[x,y,texFrom]=texFromAmount;
							} else {
								Debug.Log("Alpha/Splat layer "+texFrom+" on "+terrain.gameObject.name+" was skipped because terrain does not have that many prototypes.");
							}
							if(terdata.splatPrototypes.Length>texFrom){
								alphadata[x,y,texFrom]=texFromAmount;
							} else {
								Debug.Log("Alpha/Splat layer "+texFrom+" on "+terrain.gameObject.name+" was skipped because terrain does not have that many prototypes.");
							}
							alphadata[x,y,texTo]=texToAmount;
							alphadata[x,y,slopeTex]=slopeTexAmount;
						}
					}
				}
			}
			
			if(UseFlowTexture){
				int heightres = terdata.heightmapResolution;
				float[,] flows = new float[terdata.heightmapResolution,terdata.heightmapResolution];
				// Flow-based splatmapping :)
				for(int hY=1;hY<terdata.heightmapResolution-1;hY++){
					for(int hX=1;hX<terdata.heightmapResolution-1;hX++){
						if(terdata.GetSteepness(hX*(1f/terdata.heightmapResolution),hY*(1f/terdata.heightmapResolution))>FlowTextureParams[4]){
							flows[hX,hY]=FlowTextureParams[3];
						}
					}
				}
				
				for(int i = 0;i<(int)FlowTextureParams[0];i++){ //Iterations
					EditorUtility.DisplayProgressBar("Flowmap","Flowmap generation "+i+" / "+FlowTextureParams[0].ToString("N0"),(float)i*(1f/FlowTextureParams[0]));
					for(int hY=1;hY<terdata.heightmapResolution-1;hY++){
						for(int hX=1;hX<terdata.heightmapResolution-1;hX++){
							if(terdata.GetSteepness(hX*(1f/terdata.heightmapResolution),hY*(1f/terdata.heightmapResolution))>FlowTextureParams[4]){
								float cumulativedrop = 0f; 
								float nextHighest = 0f;
								float overflow = 0f;
								if(flows[hX,hY]>1f){
									overflow = (flows[hX,hY]-1f)/100f;	
								}
								float thisCellHeight = heights[hX,hY] + overflow;
								
								for(int nY = hY-1; nY < hY+2; nY++){
									for(int nX = hX-1; nX < hX+2; nX++){
										if(nY>0&&nX>0&&nX<heightres&&nY<heightres){
											if(heights[nX,nY]<thisCellHeight){
												cumulativedrop += thisCellHeight-heights[nX,nY];
											}
											if(heights[nX,nY]>thisCellHeight&&heights[nX,nY]<nextHighest){
												nextHighest = heights[nX,nY];
											}
										}
									}	
								}
								if(cumulativedrop>0f){
									for(int nY = hY-1; nY < hY+2; nY++){
										for(int nX = hX-1; nX < hX+2; nX++){
											if(nY>0&&nX>0&&nX<heightres&&nY<heightres){
												if(heights[nX,nY]<thisCellHeight){
													flows[nX,nY] += (thisCellHeight-heights[nX,nY]) * (flows[hX,hY]*(FlowTextureParams[1] / cumulativedrop));
													flows[hX,hY] -= (thisCellHeight-heights[nX,nY]) * (flows[hX,hY]*(FlowTextureParams[2] / cumulativedrop));
												}
											}
										}	
									}
								}
							}
						}
					}
				}			
				
				EditorUtility.ClearProgressBar();
					
				for(int hY=0;hY<terdata.alphamapResolution;hY++){
					for(int hX=0;hX<terdata.alphamapResolution;hX++){
						for(int splat=0;splat<terdata.splatPrototypes.Length; splat++){
							
							int convX = (int)(((float)hX/(float)terdata.alphamapResolution) * (float)terdata.heightmapResolution);
							int convY = (int)(((float)hY/(float)terdata.alphamapResolution) * (float)terdata.heightmapResolution);
							
							float addAmount = flows[convY,convX];
						//	addAmount = TerEdge.teFunc.clampVal(addAmount);
							addAmount = Mathf.Clamp01(addAmount);
							
							float remainder = 1.0f-addAmount;
							// Now we tally up the total value shared by other splat channels...
							float cumulativeAmount = 0f;
							for(int i2=0;i2<terdata.splatPrototypes.Length;i2++){
								if(i2!=FlowTextureID){
									cumulativeAmount += alphadata[hY,hX,i2];
								}
							}
							if(cumulativeAmount>0.0f){
								float fixLayerMult = remainder / cumulativeAmount; // we multiple the other layer's splat values by this
								// Now we re-apply the splat values..
								for(int i2=0;i2<terdata.splatPrototypes.Length;i2++){
									if(i2!=FlowTextureID){
										alphadata[hY,hX,i2] = fixLayerMult*alphadata[hY,hX,i2];
									} else {
										alphadata[hY,hX,i2] = addAmount;
									}
								}
							} else {
								alphadata[hY,hX,FlowTextureID] = 1.0f;
							}
						}
					}
				}
			}

			terdata.SetAlphamaps(0,0,alphadata);
			terrain.Flush();		
		}
	
		public static void ApplyDetails(Terrain terrain,
										int[] DetailRulePrototypeID, float[,] DetailRuleParams,
										bool[] DetailSplatPrototypeEnable, int[] DetailSplatPrototypeMatch,
										float[] DetailSplatPrototypeAmount) {

			TerrainData terdata = terrain.terrainData;
			int res = terdata.detailResolution;
			int[,] detaildata = new int[res,res];
			float tmpHeight = 0.0f;
			float tmpSlope = 0.0f;
			float strengthmult = 1.0f;
			for(int layer=0; layer<terdata.detailPrototypes.Length; layer++){
				terdata.SetDetailLayer(0,0,layer,detaildata);
			}
			float addAmount = 0.0f;
			float tmpSplatAmount = 0f;
			for(int ruleId=0;ruleId<10;ruleId++){
				if(DetailRulePrototypeID[ruleId]>0){
					detaildata = terdata.GetDetailLayer(0,0,res,res,DetailRulePrototypeID[ruleId]);
					float[,,] splatMaps = terdata.GetAlphamaps(0,0,terdata.alphamapResolution,terdata.alphamapResolution);
					float resmult = (1.0f / (float)res)*(float)terdata.alphamapResolution;
					for(int y=0;y<res;y++){
						for(int x=0;x<res;x++){
							strengthmult = DetailRuleParams[ruleId,6];
							tmpHeight = (terdata.GetHeight(y,x)/(terdata.size.y));		
							tmpSlope = terdata.GetSteepness(((1.0f/(float)res)*y)+(0.5f/(float)res),((1.0f/(float)res)*x)+(0.5f/(float)res));
							if(DetailSplatPrototypeEnable[ruleId]==true){
								tmpSplatAmount = splatMaps[(int)(resmult*(float)x),(int)(resmult*(float)y),DetailSplatPrototypeMatch[ruleId]]; 
								if(tmpSplatAmount<DetailSplatPrototypeAmount[ruleId]){strengthmult = 0f;}
							}
							if(tmpHeight<DetailRuleParams[ruleId,0]){strengthmult = 0.0f;}
							if(tmpHeight>DetailRuleParams[ruleId,1]){strengthmult = 0.0f;}
							if(tmpSlope<DetailRuleParams[ruleId,2]){strengthmult = 0.0f;}
							if(tmpSlope>DetailRuleParams[ruleId,3]){strengthmult = 0.0f;}
							if(UnityEngine.Random.value<DetailRuleParams[ruleId,7]){
								addAmount = strengthmult;
								int tmpval = detaildata[x,y]+(int)addAmount;
								if(tmpval>15){tmpval=15;}
								detaildata[x,y]=tmpval;
							}
						}
					}

					if(DetailRulePrototypeID[ruleId]-1<terdata.detailPrototypes.Length){
						terdata.SetDetailLayer(0,0,DetailRulePrototypeID[ruleId]-1,detaildata);
					}
				}	
			}
		}
	}


}
#endif