using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorTools
{
	public const float Default_TextField_H = 20f;


	public static void DrawTitle(string _title, Color _color)
	{
		Rect progressRect = GUILayoutUtility.GetRect(50, 20);
		EditorGUI.DrawRect(progressRect, _color);
		progressRect.x += 3;
		EditorGUI.LabelField(progressRect, _title);

		EditorGUILayout.Space();
	}

	public static Rect DrawAera(float _width, float _height, Color _color)
	{
		Rect progressRect = GUILayoutUtility.GetRect(_width, _height);
		EditorGUI.DrawRect(progressRect, _color);
		return progressRect;
	}

	public static string TextField(Rect root, Rect mine, string _name, string _value)
    {
		Rect now = new Rect(mine);
		now.x += root.x;
		now.y += root.y;
		return EditorGUI.TextField(now, _name, _value);
	}
}
