using UnityEditor;
using UnityEngine;

namespace OatAotOat.ProceduralDigitalAsset
{
    public abstract class ProceduralGeneratorEditor : Editor
    {
        private FurnitureTheme[] availableThemes;

        protected virtual void OnEnable()
        {
            RefreshThemes();
        }

        private void RefreshThemes()
        {
            string[] guids = AssetDatabase.FindAssets("t:FurnitureTheme");
            availableThemes = new FurnitureTheme[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                availableThemes[i] = AssetDatabase.LoadAssetAtPath<FurnitureTheme>(path);
            }
        }

        protected void DrawAppearanceSection(ProceduralGenerator script, string primaryLabel, string secondaryLabel)
        {
            EditorGUILayout.LabelField("Theme Presets", EditorStyles.boldLabel);

            if (availableThemes == null || availableThemes.Length == 0) { RefreshThemes(); }

            if (availableThemes != null && availableThemes.Length > 0)
            {
                GUILayout.BeginHorizontal();
                foreach (var theme in availableThemes)
                {
                    if (theme == null) continue;

                    Color previousBgColor = GUI.backgroundColor;
                    GUI.backgroundColor = theme.primaryColor;

                    GUIContent buttonContent = new("", $"Apply {theme.name}");

                    if (GUILayout.Button(buttonContent, GUILayout.Width(30), GUILayout.Height(30)))
                    {
                        theme.ApplyThemeTo(script);
                        serializedObject.Update();
                    }

                    GUI.backgroundColor = previousBgColor;
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(5);
                if (GUILayout.Button("Refresh Theme List", GUILayout.Width(130)))
                {
                    RefreshThemes();
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No Furniture Themes found in project. Right-click in your project window to create some!", MessageType.Info);
            }

            EditorGUILayout.Space(15);

            EditorGUILayout.LabelField("Appearance", EditorStyles.boldLabel);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("primaryColorPalette"), new GUIContent($"{primaryLabel} Palette"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("primaryColor"), new GUIContent(primaryLabel));

            if (ColorGUI.DrawColorPaletteButtons(script.primaryColorPalette, $"{primaryLabel} Presets", out Color newPrimaryColor))
            {
                Undo.RecordObject(script, $"Change {primaryLabel}");
                script.primaryColor = newPrimaryColor;
                script.ForceGenerate();
                EditorUtility.SetDirty(script);
            }

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("secondaryColorPalette"), new GUIContent($"{secondaryLabel} Palette"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("secondaryColor"), new GUIContent(secondaryLabel));

            if (ColorGUI.DrawColorPaletteButtons(script.secondaryColorPalette, $"{secondaryLabel} Presets", out Color newSecondaryColor))
            {
                Undo.RecordObject(script, $"Change {secondaryLabel}");
                script.secondaryColor = newSecondaryColor;
                script.ForceGenerate();
                EditorUtility.SetDirty(script);
            }
        }

        protected void DrawBakingSection(ProceduralGenerator script, string folderName)
        {
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Hierarchy Management", EditorStyles.boldLabel);

            if (GUILayout.Button("Organize Hierarchy in Scene", GUILayout.Height(25)))
            {
                script.OrganizeHierarchy();
                EditorUtility.SetDirty(script);
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("prefabName"));

            string pName = script.prefabName;
            if (string.IsNullOrEmpty(pName))
            {
                pName = "GeneratedAsset";
                script.prefabName = pName;
            }

            if (AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Prefabs/{folderName}/{pName}.prefab") != null)
            {
                EditorGUILayout.HelpBox($"A prefab named '{pName}' already exists. Baking will automatically number the new asset.", MessageType.Warning);
            }

            EditorGUILayout.Space(5);
            GUI.backgroundColor = new Color(0.2f, 0.8f, 0.2f);

            if (GUILayout.Button("Bake to Prefab", GUILayout.Height(40)))
            {
                if (EditorUtility.DisplayDialog("Bake Procedural Asset",
                    $"Are you sure you want to bake '{script.prefabName}' to Assets/Prefabs/{folderName}?\n\nThis will generate a Prefab and Material assets which cannot be undone.",
                    "Bake Asset", "Cancel"))
                {
                    ProceduralBakingUtility.BakeAsset(
                        folderName,
                        script.gameObject,
                        script.prefabName,
                        script.primaryColor,
                        script.secondaryColor,
                        (clone, primaryMat, secondaryMat) =>
                        {
                            clone.GetComponent<ProceduralGenerator>().ApplyBakeMaterials(primaryMat, secondaryMat);
                            OnCustomBakeModifications(clone, folderName);
                        },
                        (clone) => { clone.GetComponent<ProceduralGenerator>().OrganizeHierarchy(); }
                    );
                }
            }

            GUI.backgroundColor = Color.white;
        }

        protected virtual void OnCustomBakeModifications(GameObject clone, string folderName) { }

        protected void DrawMinMaxSlider(string propertyName, string label, float minLimit, float maxLimit)
        {
            SerializedProperty prop = serializedObject.FindProperty(propertyName);
            if (prop != null)
            {
                Vector2 val = prop.vector2Value;
                float min = val.x;
                float max = val.y;

                Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
                EditorGUI.BeginProperty(rect, new GUIContent(label), prop);

                Rect prefixRect = EditorGUI.PrefixLabel(rect, new GUIContent(label));

                float floatWidth = 40f;
                float spacing = 5f;

                Rect minRect = new(prefixRect.x, prefixRect.y, floatWidth, prefixRect.height);
                Rect sliderRect = new(minRect.xMax + spacing, prefixRect.y, prefixRect.width - (floatWidth * 2) - (spacing * 2), prefixRect.height);
                Rect maxRect = new(sliderRect.xMax + spacing, prefixRect.y, floatWidth, prefixRect.height);

                min = EditorGUI.FloatField(minRect, (float)System.Math.Round(min, 3));
                EditorGUI.MinMaxSlider(sliderRect, ref min, ref max, minLimit, maxLimit);
                max = EditorGUI.FloatField(maxRect, (float)System.Math.Round(max, 3));

                min = Mathf.Clamp(min, minLimit, max);
                max = Mathf.Clamp(max, min, maxLimit);

                prop.vector2Value = new Vector2(min, max);
                EditorGUI.EndProperty();
            }
        }
    }
}
