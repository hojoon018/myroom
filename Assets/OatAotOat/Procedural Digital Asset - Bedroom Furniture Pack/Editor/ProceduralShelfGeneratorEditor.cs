using UnityEditor;
using UnityEngine;

namespace OatAotOat.ProceduralDigitalAsset
{
    [CustomEditor(typeof(ProceduralShelfGenerator))]
    public class ProceduralShelfGeneratorEditor : ProceduralGeneratorEditor
    {
        private int currentTab = 0;
        private int settingsTab = 0;
        private readonly string[] settingsTabNames = { "Base & Frame", "Compartments", "Decorations" };

        private bool showReferences = false;
        private readonly string[] tabNames = { "Main Settings", "Style" };

        public override void OnInspectorGUI()
        {
            ProceduralShelfGenerator script = (ProceduralShelfGenerator)target;
            serializedObject.Update();

            showReferences = EditorGUILayout.Foldout(showReferences, "Model References", true, EditorStyles.foldoutHeader);
            if (showReferences)
            {
                ShowReferencesFoldout();
            }

            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("shelfStyle"));
            EditorGUILayout.Space();

            currentTab = GUILayout.Toolbar(currentTab, tabNames, GUILayout.Height(30));
            EditorGUILayout.Space(10);

            switch (currentTab)
            {
                case 0: ShowSettingsSection(script); break;
                case 1: DrawAppearanceSection(script, "Primary Color", "Secondary Color"); break;
            }

            DrawBakingSection(script, "Shelves");
            serializedObject.ApplyModifiedProperties();
        }

        #region --- GUI DRAWERS ---
        void ShowReferencesFoldout()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("leftPanel"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rightPanel"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("topPanel"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("bottomPanel"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("backPanel"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("outerLeftPanel"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("outerRightPanel"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("outerLeftBackPanel"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("outerRightBackPanel"));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("lowerDivider"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("upperDivider"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("upperCabinetBottomDivider"));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("shelfPrefab"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("shelfContainer"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sideShelfContainer"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("doorPrefab"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("doorContainer"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("upperDoorContainer"));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("bookPrefab"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("bookContainer"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("tvPrefab"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("tvContainer"));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("basePanelMesh"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseBookMesh"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("baseTvMesh"));
            EditorGUI.indentLevel--;
        }

        void ShowSettingsSection(ProceduralShelfGenerator script)
        {
            settingsTab = GUILayout.Toolbar(settingsTab, settingsTabNames, GUILayout.Height(25));
            EditorGUILayout.Space(5);

            EditorGUILayout.BeginVertical("box");
            switch (settingsTab)
            {
                case 0: ShowBaseSpecsSection(script); break;
                case 1: ShowCompartmentsSection(script); break;
                case 2: ShowDecorationsSection(script); break;
            }
            EditorGUILayout.EndVertical();
        }

        void ShowBaseSpecsSection(ProceduralShelfGenerator script)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("mainShelfWidth"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("wholeShelfHeight"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("wholeShelfDepth"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("panelThickness"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("hasBackPanel"));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("hasSideSections"));
            if (script.hasSideSections)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("sideSectionWidth"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("sideShelfCount"));
                EditorGUI.indentLevel--;
            }
        }

        void ShowCompartmentsSection(ProceduralShelfGenerator script)
        {
            if (script.shelfStyle == ShelfStyle.Bookshelf)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("shelfCount"));
            }
            else
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("lowerCabinetHeight"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("doorCount"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("doorSpacing"));
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("tvAreaHeight"));
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("hasUpperCabinet"));
                if (script.hasUpperCabinet)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("upperCabinetHeight"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("upperDoorCount"));
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.Space();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("upperShelfCount"));
            }
        }

        void ShowDecorationsSection(ProceduralShelfGenerator script)
        {
            if (script.shelfStyle == ShelfStyle.Bookshelf)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("hasBooks"));
                if (script.hasBooks)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("randomSeed"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("bookAlignment"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("bookFillPercentage"));

                    DrawMinMaxSlider("bookHeightScale", "Book Height Scale", 0.1f, 1.0f);
                    DrawMinMaxSlider("bookDepthScale", "Book Depth Scale", 0.1f, 1.0f);
                    DrawMinMaxSlider("bookThicknessScale", "Book Thickness", 0.05f, 1.0f);

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("bookLeanChance"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("maxLeanAngle"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("bookColors"));
                    EditorGUI.indentLevel--;
                }
            }
            else if (script.shelfStyle == ShelfStyle.TVCabinet)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("hasTV"));
                if (script.hasTV)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("tvFillScale"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("tvDepthScale"));
                    EditorGUI.indentLevel--;
                }
            }
        }

        protected override void OnCustomBakeModifications(GameObject clone, string folderName)
        {
            ProceduralShelfGenerator clonedScript = clone.GetComponent<ProceduralShelfGenerator>();
            Shader safeShader = ProceduralBakingUtility.GetSafeShader();

            if (clonedScript.tvContainer != null && clonedScript.tvContainer.childCount > 0)
            {
                Material blackMat = new(safeShader);
                blackMat.SetColor("_BaseColor", Color.black);
                blackMat.SetColor("_Color", Color.black);

                string matPath = AssetDatabase.GenerateUniqueAssetPath($"Assets/Prefabs/{folderName}/Materials/{clone.name}_Black.mat");
                AssetDatabase.CreateAsset(blackMat, matPath);

                foreach (Transform tv in clonedScript.tvContainer)
                {
                    ProceduralUtility.SetMaterialToPart(tv, blackMat);
                }
            }

            if (clonedScript.bookContainer != null && clonedScript.bookContainer.childCount > 0)
            {
                System.Collections.Generic.Dictionary<Color, Material> colorToMat = new();
                MaterialPropertyBlock propBlock = new();

                foreach (Transform book in clonedScript.bookContainer)
                {
                    Color bColor = clonedScript.secondaryColor;

                    if (book.TryGetComponent(out Renderer rend))
                    {
                        rend.GetPropertyBlock(propBlock);
                        if (propBlock.HasProperty("_BaseColor")) bColor = propBlock.GetColor("_BaseColor");
                        else if (propBlock.HasProperty("_Color")) bColor = propBlock.GetColor("_Color");
                    }

                    if (!colorToMat.ContainsKey(bColor))
                    {
                        Material bookMat = new(safeShader);
                        bookMat.SetColor("_BaseColor", bColor);
                        bookMat.SetColor("_Color", bColor);

                        string colorHex = ColorUtility.ToHtmlStringRGB(bColor);
                        string bMatPath = AssetDatabase.GenerateUniqueAssetPath($"Assets/Prefabs/{folderName}/Materials/{clone.name}_Book_{colorHex}.mat");
                        AssetDatabase.CreateAsset(bookMat, bMatPath);

                        colorToMat[bColor] = bookMat;
                    }

                    ProceduralUtility.SetMaterialToPart(book, colorToMat[bColor]);
                }
            }
        }
        #endregion
    }
}