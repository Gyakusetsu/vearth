
using UnityEngine;
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
    }
}


