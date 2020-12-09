using System;
using System.Collections.Generic;
using UnityEngine;

public class TileMapRenderer : MonoBehaviour
{
    [SerializeField] public SpriteRenderer spriteRenderer = null;

    public TerrainSettings terrainSettings;

    public void RenderMap(int width, int height, float[] noiseMap)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Point;
        texture.SetPixels(GenerateColorMap(noiseMap));
        texture.Apply();

        spriteRenderer.sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),
            new Vector2(0.5f, 0.5f), 100.0f);
    }

    private Color[] GenerateColorMap(float[] noiseMap)
    {
        Color[] colorMap = new Color[noiseMap.Length];
        for (int i = 0; i < noiseMap.Length; i++)
        {
            colorMap[i] = terrainSettings.terrains[terrainSettings.terrains.Length - 1].color;
            foreach (var terrainLevel in terrainSettings.terrains)
            {
                if (noiseMap[i] < terrainLevel.height / 100)
                {
                    colorMap[i] = terrainLevel.color;
                    break;
                }
            }
        }

        return colorMap;
    }
}