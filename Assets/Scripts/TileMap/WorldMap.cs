using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldMap : MonoBehaviour {

    public enum World {
        Earth,
        Lava,
        Moon
    }

    public World world;

    public Tilemap groundMap;
    public Tilemap foliageMap;

    private TerrainSettings m_TerrainSettings;
    private HeightSettings m_HeightSettings;
    private FoliageSettings m_FoliageSettings;

    public bool autoUpdate;

    private System.Random rand = new System.Random();

    public void OnValidate() {
        /*TODO: This feel VERY expensive, consider refactor */
        m_HeightSettings = Resources.Load<HeightSettings>($"Worlds/{world.ToString()}/{world.ToString()}Height");
        m_TerrainSettings = Resources.Load<TerrainSettings>($"Worlds/{world.ToString()}/{world.ToString()}Terrain");
        m_FoliageSettings = Resources.Load<FoliageSettings>($"Worlds/{world.ToString()}/{world.ToString()}Foliage");

        if (m_TerrainSettings != null) {
            m_TerrainSettings.OnValuesUpdated -= OnValuesUpdated;
            m_TerrainSettings.OnValuesUpdated += OnValuesUpdated;
        }

        if (m_HeightSettings != null) {
            m_HeightSettings.OnValuesUpdated -= OnValuesUpdated;
            m_HeightSettings.OnValuesUpdated += OnValuesUpdated;
        }

        if (m_FoliageSettings != null) {
            m_FoliageSettings.OnValuesUpdated -= OnValuesUpdated;
            m_FoliageSettings.OnValuesUpdated += OnValuesUpdated;
        }
    }

    public void OnValuesUpdated() {
        if (!Application.isPlaying) {
            GenerateMap();
        }
    }

    public void GenerateMap() {
        groundMap.ClearAllTiles();
        GenerateIsoMap();
    }

    private void GenerateIsoMap() {
        // Hide the object with the test texture
        groundMap.ClearAllTiles();

        // Generate a height map
        int terrainCount = m_TerrainSettings.terrains.Length;

        float[,] falloffMap = new float[0, 0];
        if (m_HeightSettings.useFalloff) {
            falloffMap = FalloffGenerator.GenerateFalloffMap(m_HeightSettings.dimensionLength * 2,
                m_HeightSettings.falloffMultiplier, m_HeightSettings.falloffModifier1,
                m_HeightSettings.falloffModifier2);
        }

        float[,] noiseMap =
            Noise.GenerateNoiseMapMatrix(m_HeightSettings);

        Tilemap[] tileMaps = new Tilemap[m_TerrainSettings.terrains.Length];

        // Create Tiles
        for (int x = 0; x < m_HeightSettings.dimensionLength; x++) {
            for (int y = 0; y < m_HeightSettings.dimensionLength; y++) {
                // Noise for the current tile
                float noiseHeight = 0f;
                if (m_HeightSettings.useFalloff) {
                    if (m_HeightSettings.onlyFalloff) {
                        noiseHeight = falloffMap[x, y];
                    }
                    else {
                        noiseHeight = Mathf.Clamp01((noiseMap[x, y] - falloffMap[x, y])
                                                    * m_HeightSettings.heightCurve.Evaluate(noiseMap[x, y]));
                    }
                }
                else {
                    noiseHeight = noiseMap[x, y] * m_HeightSettings.heightCurve.Evaluate(noiseMap[x, y]);
                }

                if (m_HeightSettings.invert) {
                    noiseHeight = 1 - noiseHeight;
                }

                SetTileAtPointAndHeight(x, y, noiseHeight, terrainCount);
            }
        }

        Debug.Log("Made it to the end!");
    }

    private void SetTileAtPointAndHeight(int x, int y, float noiseHeight, int terrainLevelCount) {
        // Tile assets allow you to use a height of 2z
        // Therefore, we “stretch” the noise scale more than 2 times
        float tileHeight = noiseHeight * terrainLevelCount * 2;
        int tileHeightIndex = Mathf.FloorToInt(tileHeight);

        int z = tileHeightIndex * m_HeightSettings.heightMultiplier;
        if (m_HeightSettings.flatten) {
            z = 0;
        }

        int terrainIndex = GetTilemapLevelAtHeight(noiseHeight);
        TerrainSettings.Terrain terrain = m_TerrainSettings.GetTerrain(terrainIndex);
        Tile tile = terrain.tile;
        AnimatedTile animatedTile = terrain.animatedTile;

        if (m_HeightSettings.jagged) {
            //if jagged Percent is 1, everything is shifted down 1, giving the illusion that nothing is shifted
            bool shouldApplyJagged = UnityEngine.Random.value >= 1 - m_HeightSettings.jaggedPercent;
            if (shouldApplyJagged) {
                z = Mathf.Max(0, z - terrain.heightDelta);
            }
        }


        FillTileMapGaps(x, y, z, tile);

        Vector3Int tilePosition =
            new Vector3Int(x - m_HeightSettings.dimensionLength / 2, y - m_HeightSettings.dimensionLength / 2, z);

        if (animatedTile != null) {
            groundMap.SetTile(tilePosition, animatedTile);
        }
        else {
            groundMap.SetTile(tilePosition, tile);
        }
    }

    private void FillTileMapGaps(int x, int y, int z, Tile tile) {
        if (m_HeightSettings.fillInGaps && z - 3 > 0) {
            int currentZ = z;
            while (currentZ > 0) {
                Vector3Int pillarVector =
                    new Vector3Int(x - m_HeightSettings.dimensionLength / 2, y - m_HeightSettings.dimensionLength / 2,
                        currentZ - 3);
                groundMap.SetTile(pillarVector, tile);
                currentZ -= 3;
            }
        }
    }

    private int GetTilemapLevelAtHeight(float noiseHeight) {
        for (int i = 0; i < m_TerrainSettings.terrains.Length; i++) {
            if (noiseHeight <= m_TerrainSettings.terrains[i].height / 100) {
                return i;
            }
        }

        return 0;
    }
}