using UnityEngine;
using System.Collections;

public static class Noise {
    public static float[] GenerateNoiseMapOneDimension(int width, int height, int seed, float scale, int octaves,
        float persistence,
        float lacunarity, Vector2 offset) {
        // An array of vertex data, a one-dimensional view will help get rid of unnecessary cycles later
        float[] noiseMap = new float[width * height];

        // Seed element
        System.Random rand = new System.Random(seed);

        // Octave shift to get a more interesting picture with overlapping
        Vector2[] octavesOffset = new Vector2[octaves];
        for (int i = 0; i < octaves; i++) {
            // Also use external position shift
            float xOffset = rand.Next(-100000, 100000) + offset.x;
            float yOffset = rand.Next(-100000, 100000) + offset.y;
            octavesOffset[i] = new Vector2(xOffset / width, yOffset / height);
        }

        if (scale < 0) {
            scale = 0.0001f;
        }

        // For a more visually pleasant zoom shift our view to the center
        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        // Generate points for a heightmap
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                // Set the values for the first octave
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                float superpositionCompensation = 0;

                // Octave Overlay Processing
                for (int i = 0; i < octaves; i++) {
                    // Calculate the coordinates to get from Noise Perlin
                    float xResult = (x - halfWidth) / scale * frequency + octavesOffset[i].x * frequency;
                    float yResult = (y - halfHeight) / scale * frequency + octavesOffset[i].y * frequency;

                    // Obtaining Altitude from the PRNG
                    float generatedValue = Mathf.PerlinNoise(xResult, yResult);
                    // Octave overlay
                    noiseHeight += generatedValue * amplitude;
                    // Compensate octave overlay to stay in a range [0,1]
                    noiseHeight -= superpositionCompensation;

                    // Calculation of amplitude, frequency and superposition compensation for the next octave
                    amplitude *= persistence;
                    frequency *= lacunarity;
                    superpositionCompensation = amplitude / 2;
                }

                // Save heightmap point
                // Due to the superposition of octaves, there is a chance of going out of the range [0,1]
                noiseMap[y * width + x] = Mathf.Clamp01(noiseHeight);
            }
        }

        return noiseMap;
    }

    public static float[,] GenerateNoiseMapMatrix(HeightSettings heightSettings) {
        (int dimensionLength,
            int seed,
            float scale,
            int octaves,
            float persistence,
            float lacunarity,
            Vector2 offset) = heightSettings.ToTuple();

        float[,] noiseMap = new float[dimensionLength, dimensionLength];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < octaves; i++) {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistence;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = dimensionLength / 2f;
        float halfHeight = dimensionLength / 2f;

        for (int y = 0; y < dimensionLength; y++) {
            for (int x = 0; x < dimensionLength; x++) {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++) {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxLocalNoiseHeight) {
                    maxLocalNoiseHeight = noiseHeight;
                }

                if (noiseHeight < minLocalNoiseHeight) {
                    minLocalNoiseHeight = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;

                /*if (settings.normalizeMode == NormalizeMode.Global) {
                    float normalizedHeight = (noiseMap [x, y] + 1) / (maxPossibleHeight / 0.9f);
                    noiseMap [x, y] = Mathf.Clamp (normalizedHeight, 0, int.MaxValue);
                }*/
            }
        }

        //if (settings.normalizeMode == NormalizeMode.Local) {
        for (int y = 0; y < dimensionLength; y++) {
            for (int x = 0; x < dimensionLength; x++) {
                noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
            }
        }
        //}

        return noiseMap;
    }
}

[System.Serializable]
public class NoiseSettings : MonoBehaviour {
    //public Noise.NormalizeMode normalizeMode;

    public float scale = 50;

    public int octaves = 6;
    [Range(0, 1)] public float persistance = .6f;
    public float lacunarity = 2;

    public int seed;
    public Vector2 offset;

    public void ValidateValues() {
        scale = Mathf.Max(scale, 0.01f);
        octaves = Mathf.Max(octaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1);
        persistance = Mathf.Clamp01(persistance);
    }
}