using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class HeightSettings : UpdatableData {
    // Input data for our noise generator
    [Range(1, 200)] public int dimensionLength;
    [Range(1, 10)] public int heightMultiplier = 3;
    public float scale;

    [Range(1, 6)] public int octaves;
    [Range(0, 1)] public float persistence;
    [Range(0, 1)] public float lacunarity;

    public int seed;
    public Vector2 offset;

    public AnimationCurve heightCurve;

    public bool useFalloff;
    public bool onlyFalloff;
    [Range(1, 4)] public float falloffMultiplier = 4;
    [Range(1, 10)] public float falloffModifier1 = 3;
    [Range(1, 10)] public float falloffModifier2 = 2.2f;
    public bool flatten;

    public bool fillInGaps;
    public bool invert;
    public bool jagged;
    [Range(0, 1)] public float jaggedPercent;
    
    public (int, int, float, int, float, float, Vector2) ToTuple() {
        return (dimensionLength, seed, scale, octaves, persistence, lacunarity, offset);
    }
    
#if UNITY_EDITOR

    protected override void OnValidate() {
            base.OnValidate();
    }
#endif
}