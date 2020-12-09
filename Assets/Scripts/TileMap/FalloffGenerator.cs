﻿using UnityEngine;
using System.Collections;

public static class FalloffGenerator {

	public static float[,] GenerateFalloffMap(int size, float falloffMultiplier, float falloffModifier1, float falloffModifier2) {
		float[,] map = new float[size,size];

		for (int i = 0; i < size; i++) {
			for (int j = 0; j < size; j++) {
				float x = i / (float)size * falloffMultiplier - 1;
				float y = j / (float)size * falloffMultiplier - 1;

				float value = 0.0f;


				value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
				
				//} else {
					//value = Mathf.Max (Mathf.Abs (x), Mathf.Abs (y)) - Mathf.Min (Mathf.Abs (x), Mathf.Abs (y));
				//}
				
				float falloffValue = Evaluate(value, falloffModifier1, falloffModifier2);
				map [i, j] = falloffValue;
			}
		}

		return map;
	}

	static float Evaluate(float value, float a, float b) {
		//float a = 3;
		//float b = 2.2f;
		
		return Mathf.Pow (value, a) / (Mathf.Pow (value, a) + Mathf.Pow (b - b * value, a));
	}
}
