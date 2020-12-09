using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapPreview : MonoBehaviour {
    public enum DisplayType {
        Iso,
        Noise
    };
    public DisplayType displayType;

    public enum World {
        Earth,
        Lava,
        Moon
    }

    public World world;

    public Tilemap tileMap;

    // List with tiles
    private TerrainSettings terrainSettings;

    private HeightSettings heightSettings;

    public bool autoUpdate;

    

    void OnValidate() {
        /*TODO: This feel VERY expensive, consider refactor */
        heightSettings = Resources.Load<HeightSettings>($"Worlds/{world.ToString()}/{world.ToString()}Height");
        terrainSettings = Resources.Load<TerrainSettings>($"Worlds/{world.ToString()}/{world.ToString()}Terrain");
        
        if (terrainSettings != null) {
            terrainSettings.OnValuesUpdated -= OnValuesUpdated;
            terrainSettings.OnValuesUpdated += OnValuesUpdated;
        }
        
        if (heightSettings != null) {
            heightSettings.OnValuesUpdated -= OnValuesUpdated;
            heightSettings.OnValuesUpdated += OnValuesUpdated;
        }
        
    }

    void OnValuesUpdated() {
        if (!Application.isPlaying) {
            GenerateMap();
        }
    }

    public void GenerateMap() {
        tileMap.ClearAllTiles();
        if (displayType == DisplayType.Iso) {
            GenerateIsoMap();
        }
        else if (displayType == DisplayType.Noise) {
            GenerateNoiseMap();
        }
    }

    public void GenerateIsoMap() {
        // Hide the object with the test texture
        tileMap.ClearAllTiles();

        // Generate a height map
        int terrainLevelCount = terrainSettings.levels.Length;

        float[,] falloffMap = new float[0, 0];
        if (heightSettings.useFalloff) {
            falloffMap = FalloffGenerator.GenerateFalloffMap(heightSettings.dimensionLength * 2,
                heightSettings.falloffMultiplier, heightSettings.falloffModifier1,
                heightSettings.falloffModifier2);
        }

        float[,] noiseMap =
            Noise.GenerateNoiseMapMatrix(heightSettings.dimensionLength, heightSettings.dimensionLength, heightSettings.seed,
                heightSettings.scale, heightSettings.octaves, heightSettings.persistence,
                heightSettings.lacunarity, heightSettings.offset);

        Tilemap[] tileMapLevels = new Tilemap[terrainSettings.levels.Length];

        // Create Tiles
        for (int x = 0; x < heightSettings.dimensionLength; x++) {
            for (int y = 0; y < heightSettings.dimensionLength; y++) {
                // Noise level for the current tile
                float noiseHeight = 0f;
                if (heightSettings.useFalloff) {
                    if (heightSettings.onlyFalloff) {
                        noiseHeight = falloffMap[x, y];
                    }
                    else {
                        noiseHeight = Mathf.Clamp01((noiseMap[x, y] - falloffMap[x, y])
                                                    * heightSettings.heightCurve.Evaluate(noiseMap[x, y]));
                    }
                }
                else {
                    noiseHeight = noiseMap[x, y] * heightSettings.heightCurve.Evaluate(noiseMap[x, y]);
                }

                if (heightSettings.invertHeight) {
                    noiseHeight = 1 - noiseHeight;
                }

                SetTileAtPointAndHeight(x, y, noiseHeight, terrainLevelCount);
            }
        }

        Debug.Log("Made it to the end!");
    }

    private void SetTileAtPointAndHeight(int x, int y, float noiseHeight, int terrainLevelCount) {
        // Tile assets allow you to use a height of 2z
        // Therefore, we “stretch” the noise scale more than 2 times
        float tileHeight = noiseHeight * terrainLevelCount * 2;
        int tileHeightIndex = Mathf.FloorToInt(tileHeight);

        int z = tileHeightIndex * heightSettings.heightMultiplier;
        if (heightSettings.flatten) {
            z = 0;
        }

        Tile tile = GetTile(noiseHeight);

        FillTileMapGaps(x, y, z, tile);

        Vector3Int tilePosition =
            new Vector3Int(x - heightSettings.dimensionLength / 2, y - heightSettings.dimensionLength / 2, z);
        tileMap.SetTile(tilePosition, tile);
    }

    private Tile GetTile(float noiseHeight) {
        int tilemalLevelAtHeight = GetTilemapLevelAtHeight(noiseHeight);
        return terrainSettings.levels[tilemalLevelAtHeight].tile;
    }

    private void FillTileMapGaps(int x, int y, int z, Tile tile) {
        if (heightSettings.fillInGaps && z - 3 > 0) {
            int currentZ = z;
            while (currentZ > 0) {
                Vector3Int pillarVector =
                    new Vector3Int(x - heightSettings.dimensionLength / 2, y - heightSettings.dimensionLength / 2,
                        currentZ - 3);
                tileMap.SetTile(pillarVector, tile);
                currentZ -= 3;
            }
        }
    }

    private int GetTilemapLevelAtHeight(float noiseHeight) {
        for (int i = 0; i < terrainSettings.levels.Length; i++) {
            if (noiseHeight <= terrainSettings.levels[i].height / 100) {
                return i;
            }
        }

        return 0;
    }

    // Function for generating a test texture with the parameters specified for the noise generator
    // Used from the editor extension NoiseMapEditorGenerate
    public void GenerateNoiseMap() {
        // Generate a heightmap
        float[] noiseMap =
            Noise.GenerateNoiseMapOneDimension(heightSettings.dimensionLength, heightSettings.dimensionLength,
                heightSettings.seed, heightSettings.scale, heightSettings.octaves, heightSettings.persistence,
                heightSettings.lacunarity, heightSettings.offset);

        // Depending on the filling of the array with tile assets, we generate a uniformly distributed color dependence on the noise height
        List<TerrainSettings.TerrainLevel> tl = new List<TerrainSettings.TerrainLevel>();
        // The upper border of the range determines the color, so divide the scale into equal segments and shift it up
        int terrainLevelCount = terrainSettings.levels.Length;
        float heightOffset = 1.0f / terrainLevelCount;
        for (int i = 0; i < terrainLevelCount; i++) {
            // Take the color from the texture of the asset tile
            Color color = terrainSettings.levels[i].tile.sprite.texture
                .GetPixel(terrainSettings.levels[i].tile.sprite.texture.width / 2,
                    terrainSettings.levels[i].tile.sprite.texture.height / 2);
            // Create a new color-noise level
            TerrainSettings.TerrainLevel lev = new TerrainSettings.TerrainLevel();
            lev.color = color;
            // Convert the index to a position on the scale with the range [0,1] and move up
            lev.height = Mathf.InverseLerp(0, terrainLevelCount, i) + heightOffset;
            // Save a new color-noise level
            tl.Add(lev);
        }

        // Apply a new color-noise scale and generate a texture based on it from the specified parameters
        TileMapRenderer mapRenderer = FindObjectOfType<TileMapRenderer>();
        mapRenderer.RenderMap(heightSettings.dimensionLength, heightSettings.dimensionLength, noiseMap);
    }
}