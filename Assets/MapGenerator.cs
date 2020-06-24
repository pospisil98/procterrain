using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode{NoiseMap, ColorMap};

    public int mapWidth;
    public int mapHeight;
    public float mapScale;

    public int octaves;

    [Range(0, 1)]
    public float persistance;
    public float lacunarity; 

    public int seed;
    public Vector2 offset;
    public bool autoUpdate;

    public DrawMode drawMode;

    public TerrainType[] terrains;

    public void GenerateMap() {
        float[,] noiseMap = Noise.Generate(mapWidth, mapHeight, seed, mapScale, octaves, persistance, lacunarity, offset);
        

        MapRenderer mr = FindObjectOfType<MapRenderer>();

        if (drawMode == DrawMode.NoiseMap) {
            mr.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        } else if (drawMode == DrawMode.ColorMap) {
            Color[] colorMap = this.ConvertNoiseToRegions(noiseMap);
            mr.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, noiseMap.GetLength(0), noiseMap.GetLength(1)));
        }
        
    }

    public void OnValidate() {
        if (mapWidth < 1) {
            mapWidth = 1;
        }

        if (mapHeight < 1) {
            mapHeight = 1;
        }

        if (octaves < 0) {
            octaves = 0;
        }

        if (lacunarity < 1) {
            lacunarity = 1;
        }

    }

    private Color[] ConvertNoiseToRegions(float[,] noiseMap) {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        
        Color[] coloredArray = new Color[width * height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int arrayIndex = y * width + x;
                float currentHeight = noiseMap[x, y];
                
                for (int i = 0; i < terrains.Length; i++)
                {
                    if (currentHeight <= terrains[i].height) {
                        coloredArray[arrayIndex] = terrains[i].color;
                        break;
                    }
                }
            }
        }

        return coloredArray;
    }
}

[System.Serializable]
public struct TerrainType {
    public string name;
    public float height;
    public Color color;
}
