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
        public Tile tile;
        public AnimatedTile animatedTile;
        public int heightDelta;
    }
    
    public Terrain GetTerrain(int terrainIndex) {
        return terrains[terrainIndex];
    }


#if UNITY_EDITOR

    protected override void OnValidate() {
        base.OnValidate();
    }
#endif
}