﻿using UnityEngine;
using System.Collections;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor (typeof (TileMapPreview))]
public class MapGeneratorEditor : Editor {

	public override void OnInspectorGUI() {
		TileMapPreview tileMapGen = (TileMapPreview)target;

		if (DrawDefaultInspector ()) {
			if (tileMapGen.autoUpdate) {
				tileMapGen.GenerateMap ();
			}
		}

		if (GUILayout.Button ("Generate")) {
			tileMapGen.GenerateMap ();
		}
	}
}
#endif