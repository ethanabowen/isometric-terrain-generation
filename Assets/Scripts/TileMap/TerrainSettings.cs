using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu()]
public class TerrainSettings : UpdatableData {
    public Terrain[] terrains;
    
    [System.Serializable]
    public class Terrain {
        [Range(0,100)]
        public float height;
        public Color color;
        public Tile tile;
    }
    
#if UNITY_EDITOR

    protected override void OnValidate() {
        base.OnValidate();
    }
#endif
}