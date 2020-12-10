﻿using UnityEngine;
using System.Collections;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor (typeof (WorldMap))]
public class MapGeneratorEditor : Editor {

	public override void OnInspectorGUI() {
		WorldMap worldMapGen = (WorldMap)target;

		if (DrawDefaultInspector ()) {
			if (worldMapGen.autoUpdate) {
				worldMapGen.GenerateMap ();
			}
		}

		if (GUILayout.Button ("Generate")) {
			worldMapGen.GenerateMap ();
		}
	}
}
#endif