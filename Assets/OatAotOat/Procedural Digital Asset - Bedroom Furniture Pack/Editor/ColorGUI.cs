using UnityEngine;
using UnityEditor;

namespace OatAotOat.ProceduralDigitalAsset
{
    public static class ColorGUI
    {
        public static bool DrawColorPaletteButtons(ColorPalette palette, string label, out Color selectedColor)
        {
            selectedColor = Color.clear;
            bool colorWasClicked = false;

            if (palette == null || palette.colors == null || palette.colors.Length == 0) { return false; }

            GUILayout.Space(5);
            GUILayout.Label(label, EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            foreach (Color color in palette.colors)
            {
                Color previousBgColor = GUI.backgroundColor;
                GUI.backgroundColor = color;

                if (GUILayout.Button("", GUILayout.Width(30), GUILayout.Height(30)))
                {
                    selectedColor = color;
                    colorWasClicked = true;
                }

                GUI.backgroundColor = previousBgColor;
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            return colorWasClicked;
        }
    }
}