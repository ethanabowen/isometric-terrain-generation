using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu()]
public class FoliageSettings : UpdatableData
{
    public Foliage[] levels;
    
    [System.Serializable]
    public class Foliage {
        [Range(0,100)]
        public float height;

        public Tile tile;
    }
    
#if UNITY_EDITOR

    protected override void OnValidate() {
        base.OnValidate();
    }
#endif
}
