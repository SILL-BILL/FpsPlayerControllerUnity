using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SetCanvasBounds : MonoBehaviour {

#if UNITY_IOS
	[DllImport("__Internal")]
    private extern static void GetSafeAreaImpl(out float x, out float y, out float w, out float h);
#endif

	private Rect GetSafeArea()
	{
		float x, y, w, h;
#if UNITY_IOS
		GetSafeAreaImpl(out x, out y, out w, out h);
#else
		x = 0;
		y = 0;
		w = Screen.width;
		h = Screen.height;
#endif
		return new Rect(x, y, w, h);
	}

	public RectTransform canvas;
	public RectTransform panel;
	Rect lastSafeArea = new Rect(0, 0, 0, 0);

	// Use this for initialization
	void Start () {
		
	}

	void ApplySafeArea(Rect area)
	{
		var anchorMin = area.position;
		var anchorMax = area.position + area.size;
		anchorMin.x /= Screen.width;
		anchorMin.y /= Screen.height;
		anchorMax.x /= Screen.width;
		anchorMax.y /= Screen.height;
		panel.anchorMin = anchorMin;
		panel.anchorMax = anchorMax;

		lastSafeArea = area;
	}

	// Update is called once per frame
	void Update () 
	{
		Rect safeArea = GetSafeArea(); // or Screen.safeArea if you use a version of Unity that supports it

		if (safeArea != lastSafeArea)
			ApplySafeArea(safeArea);
	}
}
